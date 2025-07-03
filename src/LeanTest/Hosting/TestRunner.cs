using LeanTest.Dependencies.Providers;
using LeanTest.Dynamic.Generating;
using LeanTest.Hosting.TestAdapter;
using LeanTest.Hosting.TestAdapter.Constants;
using LeanTest.TestInvokers;
using LeanTest.Tests;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

using System.Diagnostics;
using System.Reflection;

namespace LeanTest.Hosting;

internal class TestRunner
{
	private readonly ILogger<TestRunner> _hostLogger;
	private readonly IOptions<LoggerFilterOptions> _logOptions;

	private readonly ITestExecutionRecorder _executionRecorder;
	private readonly TestResultBuilder _resultBuilder;
	private readonly IReadOnlyList<ITestInvoker> _testInvokers;

	public TestRunner(
		ILogger<TestRunner> hostLogger, IOptions<LoggerFilterOptions> logOptions, ITestExecutionRecorder executionRecorder, TestResultBuilder resultBuilder,
		IEnumerable<ITestInvoker> testInvokers)
	{
		_hostLogger = hostLogger;
		_logOptions = logOptions;

		_executionRecorder = executionRecorder;
		_resultBuilder = resultBuilder;
		_testInvokers = testInvokers.ToArray();

	}
	public async Task RunTests(IReadOnlyList<TestCase> tests, CancellationToken cancellationToken)
	{
		_hostLogger.LogDebug("Running {0} tests", tests.Count);

		var shuffled = tests.Shuffle(cancellationToken);
		var testTasks = InvokeTests(shuffled, cancellationToken);

		await Task.WhenAll(testTasks);
	}

	private IEnumerable<Task> InvokeTests(IReadOnlyList<TestCase> tests, CancellationToken cancellationToken)
	{
		// https://github.com/Marvin-Brouwer/LeanTest/issues/10
		var minCount = ThreadPool.ThreadCount;
		ThreadPool.GetMaxThreads(out var maxThreads, out _);
		var maxCount = maxThreads * 2;

		var semaphore = new SemaphoreSlim(minCount, maxCount);

		foreach (var test in tests)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				EndTest(test, _resultBuilder.CancelTest(test), null);
				continue;
			}
			yield return InvokeTest(test, semaphore, cancellationToken);
		}
	}

	private async Task InvokeTest(TestCase testCase, SemaphoreSlim semaphore, CancellationToken cancellationToken)
	{
		await semaphore.WaitAsync(cancellationToken);
		_hostLogger.LogTrace("Running {testName}", testCase.DisplayName);

		// Start the spinner but don't record the start time yet
		_executionRecorder.RecordStart(testCase);

		if (cancellationToken.IsCancellationRequested)
		{
			EndTest(testCase, _resultBuilder.CancelTest(testCase), semaphore);
			return;
		}

		var testPropertyName = testCase.GetPropertyValue<string>(TestProperties.PropertyName, null);
		if (testPropertyName is null || string.IsNullOrEmpty(testCase.CodeFilePath))
		{
			semaphore.Release();
			throw new Exception("TODO: NotFound result");
		}

		if (cancellationToken.IsCancellationRequested)
		{
			EndTest(testCase, _resultBuilder.CancelTest(testCase), semaphore);
			return;
		}

		var testAssemblyType = testCase.GetPropertyValue<string>(TestProperties.SuiteTypeName, null);
		if (string.IsNullOrEmpty(testAssemblyType))
		{
			semaphore.Release();
			throw new Exception("TODO: NotFound result");
		}

		var testSuiteType = Type.GetType(testAssemblyType);
		if (testSuiteType is null)
		{
			semaphore.Release();
			throw new Exception("TODO: NotFound result");
		}
		if (cancellationToken.IsCancellationRequested)
		{
			EndTest(testCase, _resultBuilder.CancelTest(testCase), semaphore);
			return;
		}

		var testAssembly = testSuiteType.Assembly;
		var resultStreamingLoggerFactory = new TestCaseLoggerFactory(_logOptions);

		TestContext.Current.TestCancellationToken = new CancellationTokenProvider(cancellationToken);
		TestContext.Current.AssemblyContext = new RuntimeAssemblyContext(testAssembly);
		TestContext.Current.TestLoggerFactory = resultStreamingLoggerFactory;

		// We can assume the suite succeeds instantiation because we already need this during discovery phase.
		// If this fails, the test should break horribly too.
		var suite = (ITestSuite)Activator.CreateInstance(testSuiteType)!;
		if (cancellationToken.IsCancellationRequested)
		{
			EndTest(testCase, _resultBuilder.CancelTest(testCase), semaphore);
			return;
		}
		EndTest(testCase, _resultBuilder.SkipTest(testCase, $"No {nameof(ITestInvoker)} was found for type ${suite.GetType().Name}"), semaphore);

		var testInvoker = _testInvokers.FirstOrDefault(invoker => invoker.SupportsSuite(suite));
		if (testInvoker is null)
		{
			EndTest(testCase, _resultBuilder.SkipTest(testCase, $"No {nameof(ITestInvoker)} was found for type ${suite.GetType().Name}"), semaphore);
			return;
		}

		var testProperty = testSuiteType
			.GetProperty(testPropertyName)?
			.GetValue(suite);

		if (testProperty is not ITest test)
		{
			semaphore.Release();
			throw new UnreachableException();
		}

		try
		{
			var testResult = await testInvoker.Invoke(testCase, test, resultStreamingLoggerFactory, cancellationToken);
			EndTest(testCase, testResult, semaphore);
		}
		catch (Exception ex)
		{
			EndTest(testCase, _resultBuilder.FailTest(testCase, ex, resultStreamingLoggerFactory.Logs), semaphore);
		}

		// TODO add Test.Skip situation?
	}

	private void EndTest(TestCase testCase, TestResult testResult, SemaphoreSlim? semaphore)
	{
		_executionRecorder.RecordResult(testResult);
		_executionRecorder.RecordEnd(testCase, testResult.Outcome);
		semaphore?.Release();
	}
}
