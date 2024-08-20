using LeanTest.Tests;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;

using MsTestCase = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase;
using LeanTest.Hosting.TestAdapter.Constants;

namespace LeanTest.Hosting.TestAdapter.VsTest;

[FileExtension(".exe"), FileExtension(".dll")]
public abstract class LeanTestDiscoverer : ITestDiscoverer
{
	private ILogger _logger = null!;

	public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
	{
#if DEBUG
		_logger = logger.Wrap(LogLevel.Debug);
#else
		_logger = logger.Wrap(LogLevel.Information);
#endif

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
			_logger.LogDebug("Skipping {assemblyPath} because it is not a test assembly.", assemblyPath);
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

			_logger.LogInformation("Found {testCount} test(s) in {assemblyPath}", testCases.Length, assemblyPath);

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
		var assemblyLoadContext = new AssemblyLoadContext($"{nameof(LeanTestDiscoverer)}.{assemblyPath}", true);
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
					var msTestCase = new MsTestCase(testSuite.FullName!, LeanTestExecutor.Uri, assemblyPath)
					{
						Id = StringToGUID(testSuite.FullName!),
						DisplayName =
							testSuite.Name + " [FAILED]" + Environment.NewLine +
							instantiationException,
						// TODO: Perhaps it's best to actually parse the stacktrace and fish out the fileName
						// Otherwise: https://stackoverflow.com/questions/10960071/how-to-find-path-to-cs-file-by-its-type-in-c-sharp
						CodeFilePath = testSuite.Name,
						LineNumber = instantiationException.StackTrace is not null
							? int.Parse(instantiationException.StackTrace?
								.Split(Environment.NewLine)
								.Reverse()
								.Skip(1)
								.First()!
								.Split(":line ")
								.ElementAtOrDefault(1) ?? "0")
							: -1
					};

					yield return msTestCase;
					yield break;
				}
				foreach (var testProperty in testSuite.GetProperties(BindingFlags.Instance | BindingFlags.Public))
				{
					var testPropertyGetter = testProperty.GetGetMethod(true);

					if (testPropertyGetter is null) yield break;
					if (testProperty.PropertyType != typeof(ITest)) yield break;

					var test = (ITest)testProperty.GetValue(suite)!;

					if (test is UnitTestDataScenario dt)
					{
						for (var index = 0; index < dt.TestData.Count; index++)
						{
							var testData = dt.TestData[index];
							var (fullyQualifiedName, displayName) = testProperty.GetTestNames(testSuite.Name, testData);

							var msTestCase = new MsTestCase(fullyQualifiedName, LeanTestExecutor.Uri, assemblyPath)
							{
								Id = StringToGUID(displayName),
								DisplayName = displayName,
								CodeFilePath = dt.FilePath,
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
								msTestCase.SetPropertyValue(TestProperties.SuiteTypeName, testSuite.AssemblyQualifiedName);
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

						var msTestCase = new MsTestCase(fullyQualifiedName, LeanTestExecutor.Uri, assemblyPath)
						{
							Id = StringToGUID(displayName),
							DisplayName = displayName,
							CodeFilePath = tc.FilePath,
							LineNumber = tc.LineNumber,
							Traits = {
								{  "Type" , "Unit" },
								{  "Input" , "Singular" }
							}
						};
						try
						{
							msTestCase.SetPropertyValue(TestProperties.PropertyName, testProperty.Name);
							msTestCase.SetPropertyValue(TestProperties.SuiteTypeName, testSuite.AssemblyQualifiedName);
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
		catch (TargetInvocationException ex) when (ex.InnerException is not null) {
			return (null, ex.InnerException);
		}
		catch (Exception ex)
		{
			if (!Debugger.IsAttached) Debugger.Launch();
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
