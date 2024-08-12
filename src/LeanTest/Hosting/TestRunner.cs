using LeanTest.Dependencies.Providers;
using LeanTest.Dynamic.Generating;
using LeanTest.Hosting.TestAdapter;
using LeanTest.Hosting.TestAdapter.Constants;
using LeanTest.Tests;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

using System.Diagnostics;

namespace LeanTest.Hosting;

internal class TestRunner
{
	private readonly ILogger<TestRunner> _hostLogger;
	private readonly IOptions<LoggerFilterOptions> _logOptions;

	private readonly ITestExecutionRecorder _executionRecorder;
	private readonly TestResultBuilder _resultBuilder;

	public TestRunner(ILogger<TestRunner> hostLogger, IOptions<LoggerFilterOptions> logOptions, ITestExecutionRecorder executionRecorder, TestResultBuilder resultBuilder)
	{
		_hostLogger = hostLogger;
		_logOptions = logOptions;

		_executionRecorder = executionRecorder;
		_resultBuilder = resultBuilder;
	}
	public async Task RunTests(IReadOnlyList<TestCase> tests, CancellationToken cancellationToken)
	{
		_hostLogger.LogDebug("Running {0} tests", tests.Count);

		var shuffled = tests.Shuffle(cancellationToken);
		var testTasks = InvokeTests(shuffled, cancellationToken);

		// TODO Semaphore?
		await Task.WhenAll(testTasks);
	}

	private IEnumerable<Task> InvokeTests(IReadOnlyList<TestCase> tests, CancellationToken cancellationToken)
	{
		foreach (var test in tests)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				EndTest(test, _resultBuilder.CancelTest(test));
				continue;
			}
			yield return InvokeTest(test, cancellationToken);
		}
	}

	private async Task InvokeTest(TestCase testCase, CancellationToken cancellationToken)
	{
		_hostLogger.LogTrace("Running {testName}", testCase.DisplayName);

		// Start the spinner but don't record the start time yet
		_executionRecorder.RecordStart(testCase);

		if (cancellationToken.IsCancellationRequested)
		{
			EndTest(testCase, _resultBuilder.CancelTest(testCase));
			return;
		}

		var testPropertyName = testCase.GetPropertyValue<string>(TestProperties.PropertyName, null);
		if (testPropertyName is null || string.IsNullOrEmpty(testCase.CodeFilePath))
		{
			throw new Exception("TODO: NotFound result");
		}

		if (cancellationToken.IsCancellationRequested)
		{
			EndTest(testCase, _resultBuilder.CancelTest(testCase));
			return;
		}

		var testAssemblyType = testCase.GetPropertyValue<string>(TestProperties.SuiteTypeName, null);
		if (string.IsNullOrEmpty(testAssemblyType))
		{
			throw new Exception("TODO: NotFound result");
		}

		var testSuiteType = Type.GetType(testAssemblyType);
		if (testSuiteType is null)
		{
			throw new Exception("TODO: NotFound result");
		}
		if (cancellationToken.IsCancellationRequested)
		{
			EndTest(testCase, _resultBuilder.CancelTest(testCase));
			return;
		}

		// We can assume the suite succeeds instantiation because we already need this during discovery phase.
		// If this fails, the test should break horribly too.
		var suite = (TestSuite.UnitTests)Activator.CreateInstance(testSuiteType)!;
		if (cancellationToken.IsCancellationRequested)
		{
			EndTest(testCase, _resultBuilder.CancelTest(testCase));
			return;
		}

		var testAssembly = suite.GetType().Assembly;
		var resultStreamingLoggerFactory = new TestCaseLoggerFactory(_logOptions);

		TestContext.Current.TestCancellationToken = new CancellationTokenProvider(cancellationToken);
		TestContext.Current.AssemblyContext = new RuntimeAssemblyContext(testAssembly);
		TestContext.Current.TestLoggerFactory = resultStreamingLoggerFactory;

		var test = testSuiteType.GetProperty(testPropertyName)?.GetValue(suite);
		if (test is UnitTestDataScenario dt)
		{
			var testParametersIndex = testCase.GetPropertyValue(TestProperties.DataParametersIndex, -1);
			if (testParametersIndex == -1)
			{
				_hostLogger.LogCritical("TODO Fail");
				throw new Exception("TODO fail");
			}

			var testParameters = dt.TestData[testParametersIndex];
			if (cancellationToken.IsCancellationRequested)
			{
				EndTest(testCase, _resultBuilder.CancelTest(testCase));
				return;
			}

			// Record start time
			var startTime = DateTime.UtcNow;
			var stopwatch = Stopwatch.StartNew();

			try
			{
				await dt.TestBody(testParameters);
				var testResult = _resultBuilder.PassTest(testCase, resultStreamingLoggerFactory.Logs);
				EndTest(testCase, testResult, startTime, stopwatch);
			}
			catch(Exception ex)
			{
				var testResult = _resultBuilder.FailTest(testCase, ex, resultStreamingLoggerFactory.Logs);
				EndTest(testCase, testResult, startTime, stopwatch);
			}

			return;
		}
		if (test is UnitTestCase tc)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				EndTest(testCase, _resultBuilder.CancelTest(testCase));
				return;
			}

			// Record start time
			var startTime = DateTime.UtcNow;
			var stopwatch = Stopwatch.StartNew();

			try
			{
				await tc.TestBody();

				var testResult = _resultBuilder.PassTest(testCase, resultStreamingLoggerFactory.Logs);
				EndTest(testCase, testResult, startTime, stopwatch);
			}
			catch (Exception ex)
			{
				var testResult = _resultBuilder.FailTest(testCase, ex, resultStreamingLoggerFactory.Logs);
				EndTest(testCase, testResult, startTime, stopwatch);
			}
			return;
		}

		throw new UnreachableException();

		// TODO add Test.Skip situation?
	}

	private void EndTest(TestCase testCase, TestResult testResult)
	{
		_executionRecorder.RecordResult(testResult);
		_executionRecorder.RecordEnd(testCase, testResult.Outcome);
	}

	private void EndTest(TestCase testCase, TestResult testResult, DateTime startTime, Stopwatch stopwatch)
	{
		testResult.StartTime = startTime;
		testResult.EndTime = DateTime.UtcNow;
		stopwatch.Stop();
		testResult.Duration = stopwatch.Elapsed;

		EndTest(testCase, testResult);
	}
}
