using LeanTest.Hosting;
using LeanTest.Hosting.TestAdapter;
using LeanTest.Hosting.TestAdapter.Constants;
using LeanTest.Tests;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using System.Diagnostics;
using System.Reflection;

namespace LeanTest.TestInvokers;

internal class UnitTestInvoker : ITestInvoker
{
	private readonly ILogger<UnitTestInvoker> _hostLogger;
	private readonly TestResultBuilder _resultBuilder;

	public UnitTestInvoker(ILogger<UnitTestInvoker> hostLogger, TestResultBuilder resultBuilder)
	{
		_hostLogger = hostLogger;
		_resultBuilder = resultBuilder;
	}

	public bool SupportsSuite(ITestSuite test) => test is TestSuite.UnitTests;

	// TODO skipped test situation
	public async Task<TestResult> Invoke(TestCase testCase, ITest test, TestCaseLoggerFactory loggerFactory, CancellationToken cancellationToken)
	{
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
				return _resultBuilder
					.CancelTest(testCase);
			}

			// Record start time
			var startTime = DateTime.UtcNow;
			var stopwatch = Stopwatch.StartNew();

			try
			{
				if (dt.TestBody.Method.ReturnType == typeof(ValueTask))
					await(ValueTask)dt.TestBody.DynamicInvoke(testParameters)!;
				else
					dt.TestBody.DynamicInvoke(testParameters);

				return _resultBuilder
					.PassTest(testCase, loggerFactory.Logs, startTime, stopwatch);
			}
			catch (OperationCanceledException ex)
			{
				return _resultBuilder
					.CancelTest(testCase, ex, loggerFactory.Logs, startTime, stopwatch);
			}
			catch (TargetInvocationException invocationException) when (invocationException.InnerException is OperationCanceledException ex)
			{
				return _resultBuilder
					.CancelTest(testCase, ex, loggerFactory.Logs, startTime, stopwatch);
			}
			catch (Exception ex)
			{
				var testException = ex is TargetInvocationException invocationException && invocationException.InnerException is not null
					? invocationException.InnerException
					: ex;

				return _resultBuilder
					.FailTest(testCase, testException, loggerFactory.Logs, startTime, stopwatch);
			}
		}

		if (test is UnitTestCase tc)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return _resultBuilder
					.CancelTest(testCase);
			}

			// Record start time
			var startTime = DateTime.UtcNow;
			var stopwatch = Stopwatch.StartNew();

			try
			{
				if (tc.TestBody.Method.ReturnType == typeof(ValueTask))
					await(ValueTask)tc.TestBody.DynamicInvoke()!;
				else
					tc.TestBody.DynamicInvoke();

				return _resultBuilder
					.PassTest(testCase, loggerFactory.Logs, startTime, stopwatch);
			}
			catch (OperationCanceledException ex)
			{
				return _resultBuilder
					.CancelTest(testCase, ex, loggerFactory.Logs, startTime, stopwatch);
			}
			catch (TargetInvocationException invocationException) when (invocationException.InnerException is OperationCanceledException ex)
			{
				return _resultBuilder
					.CancelTest(testCase, ex, loggerFactory.Logs, startTime, stopwatch);
			}
			catch (Exception ex)
			{
				var testException = ex is TargetInvocationException invocationException && invocationException.InnerException is not null
					? invocationException.InnerException
					: ex;

				return _resultBuilder
					.FailTest(testCase, testException, loggerFactory.Logs, startTime, stopwatch);
			}
		}

		throw new UnreachableException();
	}
}
