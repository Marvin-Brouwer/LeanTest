using LeanTest.Tests;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;

using TestCase = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase;

namespace LeanTest.Hosting.TestAdapter;

[DefaultExecutorUri(TestExecutor.Id)]
[FileExtension(".exe")]
[FileExtension(".dll")]
internal class TestDiscoverer : ITestDiscoverer
{
	private ILogger _logger = null!;
	// TODO should come from settings
	private readonly bool _shouldAttach = true;

	public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
	{
		_logger = logger.Wrap();
		if (_shouldAttach && !Debugger.IsAttached) Debugger.Launch();

		foreach (var assemblyPath in sources) DiscoverTests(assemblyPath, discoverySink);
	}

	private void DiscoverTests(string assemblyPath, ITestCaseDiscoverySink discoverySink)
	{
		if (!assemblyPath.IsTestAssembly())
		{
			_logger.LogInformation("Skipping {assemblyPath} because it is not a test assembly.", assemblyPath);
			return;
		}

		var testCases = GetTestsForAssembly(assemblyPath).ToArray();
		if (testCases.Length > 0)
		{
			_logger.LogInformation("Skipping {assemblyPath} because no tests were found with testCases", assemblyPath);
			return;
		}

		_logger.LogDebug("Found {testCount} test(s) in {assemblyPath}", testCases.Length, assemblyPath);

		foreach (var testCase in testCases)
			discoverySink.SendTestCase(testCase);
	}

	private IEnumerable<TestCase> GetTestsForAssembly(string assemblyPath)
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
				foreach (var testProperty in testSuite.GetProperties())
				{
					if (testProperty.GetGetMethod() is null) yield break;
					if (testProperty.PropertyType != typeof(ITest)) yield break;

					yield return new TestCase($"{testSuite.Namespace}.{testSuite.Name}.{testProperty.Name}", TestExecutor.Uri, assemblyPath);
				}
			}
		}
		finally
		{
			assemblyLoadContext.Unload();
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
}
