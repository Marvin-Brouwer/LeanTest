using LeanTest.Tests;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LeanTest.Hosting.TestAdapter.Standalone;

// TODO Implement discovery protocol
internal class DiscoveringTestFactory : ITestFactory
{
	private readonly ILoggerFactory _loggerFactory;
	private readonly ILogger<DiscoveringTestFactory> _logger;

	public DiscoveringTestFactory(ILoggerFactory loggerFactory)
	{
		_loggerFactory = loggerFactory;
		_logger = loggerFactory.CreateLogger<DiscoveringTestFactory>();
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async IAsyncEnumerable<TestCase> InitializeTests(
		Assembly assembly, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		yield break;
		throw new NotImplementedException("TODO Parity fix");
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

		//TestContext.Current.TestLoggerFactory = _loggerFactory;
		//TestContext.Current.TestCancellationToken = new CancellationTokenProvider(cancellationToken);
		//TestContext.Current.AssemblyContext = new RuntimeAssemblyContext(assembly);

		//if (cancellationToken.IsCancellationRequested) yield break;
		//var assemblyTestCases = InitializeScenariosForAssembly(assembly, cancellationToken);
		//if (cancellationToken.IsCancellationRequested) yield break;

		//await foreach (var cases in assemblyTestCases)
		//{
		//	if (cancellationToken.IsCancellationRequested) yield break;
		//	yield return cases;
		//}
	}

	// TODO, move this out and create an alternative that uses the TestHostContext
	private async IAsyncEnumerable<TestRun> InitializeScenariosForAssembly(
		Assembly assembly, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		var suiteTypes = IndexTestSuites(assembly, cancellationToken)
			.ToArray()
			.Shuffle(cancellationToken);

		_logger.LogDebug("Found {amount} testSuites in {assemblyName}", suiteTypes.Count, assembly.FullName);
		if (cancellationToken.IsCancellationRequested) yield break;

		// Enumerate tests after logger's been inserted
		foreach (var suiteType in suiteTypes)
		{
			if (cancellationToken.IsCancellationRequested) yield break;

			_logger.LogTrace("Initializing {testSuiteName}", suiteType.FullName);
			var suite = await InitializeSuite(suiteType);

			foreach (var testProperty in suiteType.GetProperties())
			{
				if (cancellationToken.IsCancellationRequested) yield break;
				if (testProperty.GetGetMethod() is null) yield break;
				if (testProperty.PropertyType != typeof(ITest)) yield break;

				var test = testProperty.GetValue(suite) ?? throw new NotSupportedException($"Type null is not supported");
				if (test is UnitTestCase testCase)
				{
					yield return new TestRun(testCase.TestBody, testProperty.Name!, suiteType.FullName ?? suiteType.Name);
					continue;
				}

				if (test is UnitTestDataScenario testScenario)
				{
					foreach (var dataRecord in testScenario.TestData)
					{
						yield return new TestRun(() => testScenario.TestBody(dataRecord), testProperty.Name!, suiteType.FullName ?? suiteType.Name);
					}
					continue;
				}

				throw new NotSupportedException($"Type {test.GetType()} is currently not supported");
			}
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

	private Task<TestSuite.UnitTests> InitializeSuite(Type suiteType)
	{
		// Use a task completion source, this ensures it's wrapped in a task properly
		// This means the parent task doesn't block when someone buils a huge testsuite constructor
		var completion = new TaskCompletionSource<TestSuite.UnitTests>();

		TestSuite.UnitTests instance = default!;
		try
		{
			instance = (TestSuite.UnitTests)Activator.CreateInstance(suiteType)!;
			completion.SetResult(instance);
		}
		catch (Exception ex)
		{
			// TODO handle constructor exceptions
			_ = ex;
		}

		Debug.Assert(instance is not null,
			$"Types passed to {nameof(InitializeSuite)} are known to be {typeof(TestSuite.UnitTests).Name}");

		return completion.Task;
	}
}

