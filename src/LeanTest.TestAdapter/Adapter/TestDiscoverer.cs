using LeanTest.TestAdapter.Constants;
using LeanTest.Tests;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

using Newtonsoft.Json;

using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;

using static System.Net.Mime.MediaTypeNames;

using MsTestCase = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase;

namespace LeanTest.TestAdapter.Adapter;

[DefaultExecutorUri(TestExecutor.Id)]
[FileExtension(".exe"), FileExtension(".dll")]
internal class TestDiscoverer : ITestDiscoverer
{
	private ILogger _logger = null!;

	public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
	{
		_logger = logger.Wrap();

#if (ATTACH_DISCOVERY)
		if (sources.All(source => source.EndsWith("LeanTest.TestAdapter.dll"))) return;
		if (!Debugger.IsAttached) Debugger.Launch();
#endif

		foreach (var assemblyPath in sources) DiscoverTests(assemblyPath, discoverySink);
	}

	private void DiscoverTests(string assemblyPath, ITestCaseDiscoverySink discoverySink)
	{
		if (!assemblyPath.IsTestAssembly())
		{
			_logger.LogInformation("Skipping {assemblyPath} because it is not a test assembly.", assemblyPath);
			return;
		}
		try
		{
			var testCases = GetTestsForAssembly(assemblyPath).ToArray();
			if (testCases.Length < 0)
			{
				_logger.LogInformation("Skipping {assemblyPath} because no tests were found with testCases", assemblyPath);
				return;
			}

			_logger.LogDebug("Found {testCount} test(s) in {assemblyPath}", testCases.Length, assemblyPath);

			foreach (var testCase in testCases)
				discoverySink.SendTestCase(testCase);
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, "Failed discovery");
			if (!Debugger.IsAttached) Debugger.Launch();
		}
	}

	private IEnumerable<MsTestCase> GetTestsForAssembly(string assemblyPath)
	{
		var assemblyLoadContext = new AssemblyLoadContext($"{nameof(TestDiscoverer)}.{assemblyPath}", true);
		try
		{
			var assembly = assemblyLoadContext.LoadFromAssemblyName(AssemblyName.GetAssemblyName(assemblyPath));
			if (assembly is null) throw new NotSupportedException("TODO Custom exception");

			var testSuites = IndexTestSuites(assembly, CancellationToken.None).ToArray();
			_logger.LogDebug("Found {suiteCount} test suites in {assemblyPath}", testSuites.Length, assemblyPath);

			foreach (var testSuite in testSuites)
			{
				var (suite, instantiationException) = InstantiateUnitTestSuite(testSuite);
				if (instantiationException is not null)
				{
					// TODO register as instantly failing testcase
					throw instantiationException;
				}
				foreach (var testProperty in testSuite.GetProperties(BindingFlags.Instance | BindingFlags.Public))
				{
					var testPropertyGetter = testProperty.GetGetMethod(true);

					if (testPropertyGetter is null) yield break;
					if (testProperty.PropertyType != typeof(ITest)) yield break;

					var test = (ITest)testProperty.GetValue(suite)!;

					if (test is UnitTestDataScenario dt)
					{
						for (var index = 0; index < dt.TestData.Count; index ++)
						{
							var testData = dt.TestData[index];
							var (fullyQualifiedName, displayName) = testProperty.GetTestNames(testSuite.Name, testData);

							var msTestCase = new MsTestCase(fullyQualifiedName, TestExecutor.Uri, assemblyPath)
							{
								Id = StringToGUID(displayName),
								DisplayName = displayName,
								CodeFilePath = testSuite.AssemblyQualifiedName,
								LineNumber = dt.LineNumber,
								Traits = {
									{  "Type" , "Unit" },
									{  "Input" , "Data" }
								}
							};

							try
							{
								msTestCase.SetPropertyValue(TestProperties.PropertyName, testProperty.Name);
								msTestCase.SetPropertyValue(TestProperties.DataParametersIndex, index);
							}
							catch (Exception ex)
							{
								_logger.LogError(ex, "Failed setting properties");
								throw;
							}

							yield return msTestCase;
						}
					}
					else if (test is UnitTestCase tc)
					{
						var (fullyQualifiedName, displayName) = testProperty.GetTestNames(testSuite.Name);

						var msTestCase = new MsTestCase(fullyQualifiedName, TestExecutor.Uri, assemblyPath)
						{
							Id = StringToGUID(displayName),
							DisplayName = displayName,
							CodeFilePath = testSuite.AssemblyQualifiedName,
							LineNumber = tc.LineNumber,
							Traits = {
								{  "Type" , "Unit" },
								{  "Input" , "Singular" }
							}
						};
						try
						{
							msTestCase.SetPropertyValue(TestProperties.PropertyName, testProperty.Name);
						}
						catch (Exception ex)
						{
							_logger.LogError(ex, "Failed setting properties");
							throw;
						}

						yield return msTestCase;
					}
				}
			}
		}
		finally
		{
			assemblyLoadContext.Unload();
		}
	}

	// TODO Extract and use in Executor too.
	private static (TestSuite.UnitTests? suite, Exception? exception) InstantiateUnitTestSuite(Type testSuite)
	{
		try
		{
			var suite = (TestSuite.UnitTests)Activator.CreateInstance(testSuite)!;
			return (suite, null);
		}
		catch (Exception ex)
		{
			return (null, ex);
		}
	}

	internal static IEnumerable<Type> IndexTestSuites(Assembly assembly, CancellationToken cancellationToken)
	{
		foreach (var assemblyScannedType in assembly.GetExportedTypes())
		{
			if (cancellationToken.IsCancellationRequested) yield break;
			if (!assemblyScannedType.IsAssignableTo(typeof(TestSuite.UnitTests))) continue;

			yield return assemblyScannedType;
		}
	}

	/// <summary>
	/// Generate a consistent GUID based on test names. <br />
	/// <see href="https://weblogs.asp.net/haithamkhedre/generate-guid-from-any-string-using-c"/>
	/// </summary>
	private static Guid StringToGUID(string value)
	{
		// Create a new instance of the MD5CryptoServiceProvider object.
		MD5 md5Hasher = MD5.Create();
		// Convert the input string to a byte array and compute the hash.
		byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(value));
		return new Guid(data);
	}
}
