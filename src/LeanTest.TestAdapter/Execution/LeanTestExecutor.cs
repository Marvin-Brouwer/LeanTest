using LeanTest.TestAdapter.Adapter;
using LeanTest.TestAdapter.Constants;
using LeanTest.Tests;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

using System.Diagnostics;

namespace LeanTest.TestAdapter.Execution;

/// <summary>
/// Basically the <see cref="TestExecutor"/>. Except, without the boilerplate.
/// </summary>
internal sealed class LeanTestExecutor
{
	private readonly ILogger _logger;
	private readonly IFrameworkHandle _frameworkHandle;
	private readonly CancellationToken _cancellationToken;
	private readonly ResultBuilder _resultBuilder;

	public LeanTestExecutor(ILogger logger, IFrameworkHandle frameworkHandle, CancellationToken cancellationToken)
	{
		_logger = logger;
		_frameworkHandle = frameworkHandle;
		_cancellationToken = cancellationToken;
		_resultBuilder = new ResultBuilder(logger);
	}

	public void ExecuteTests(IEnumerable<TestCase> tests)
	{
		foreach (var testCase in tests)
		{
			if (_cancellationToken.IsCancellationRequested)
			{
				EndTest(testCase, _resultBuilder.CancelTest(testCase));
				continue;
			}
			ExecuteTestCase(testCase);
		}
	}

	private void ExecuteTestCase(TestCase testCase)
	{
		_logger.LogDebug("Starting {testName}", testCase.FullyQualifiedName);

		var startTime = DateTime.UtcNow;
		_frameworkHandle.RecordStart(testCase);

		try
		{
			if (_cancellationToken.IsCancellationRequested)
			{
				EndTest(testCase, _resultBuilder.CancelTest(testCase));
				return;
			}

			var testPropertyName = testCase.GetPropertyValue<string>(TestProperties.PropertyName, null);
			if (testPropertyName is null || string.IsNullOrEmpty(testCase.CodeFilePath))
			{
				throw new Exception("TODO: NotFound result");
			}
			if (_cancellationToken.IsCancellationRequested)
			{
				EndTest(testCase, _resultBuilder.CancelTest(testCase));
				return;
			}

			var testSuiteType = Type.GetType(testCase.CodeFilePath);
			if (testSuiteType is null)
			{
				throw new Exception("TODO: NotFound result");
			}
			if (_cancellationToken.IsCancellationRequested)
			{
				EndTest(testCase, _resultBuilder.CancelTest(testCase));
				return;
			}

			// We can assume the suite succeeds instantiation because we already need this during discovery phase.
			// If this fails, the test should break horribly too.
			var suite = (TestSuite.UnitTests)Activator.CreateInstance(testSuiteType)!;
			if (_cancellationToken.IsCancellationRequested)
			{
				EndTest(testCase, _resultBuilder.CancelTest(testCase));
				return;
			}
			var test = testSuiteType.GetProperty(testPropertyName)?.GetValue(suite);

			if (test is UnitTestDataScenario dt)
			{
				var testParametersIndex = testCase.GetPropertyValue(TestProperties.DataParametersIndex,  -1);
				if (testParametersIndex == -1)
				{
					_logger.LogCritical("TODO Fail");
					throw new Exception("TODO fail");
				}

				var testParameters = dt.TestData[testParametersIndex];
				if (_cancellationToken.IsCancellationRequested)
				{
					EndTest(testCase, _resultBuilder.CancelTest(testCase));
					return;
				}
				// TODO handle valueTask
				dt.TestBody(testParameters).AsTask().Wait();
				var testResult = _resultBuilder.PassTest(testCase);
				EndTest(testCase, testResult, startTime);
				return;
			}
			if (test is UnitTestCase tc)
			{
				if (_cancellationToken.IsCancellationRequested)
				{
					EndTest(testCase, _resultBuilder.CancelTest(testCase));
					return;
				}
				// TODO handle valueTask
				tc.TestBody().AsTask().Wait();
				var testResult = _resultBuilder.PassTest(testCase);
				EndTest(testCase, testResult, startTime);
				return;
			}

			throw new UnreachableException();

			// TODO Semaphore?
			// TODO add Test.Skip situation?
		}
		catch (OperationCanceledException)
		{
			var testResult = _resultBuilder.CancelTest(testCase);
			EndTest(testCase, testResult);
		}
		catch (Exception ex)
		{
			var testResult = _resultBuilder.FailTest(testCase, ex);
			EndTest(testCase, testResult, startTime);
		}
	}

	private void EndTest(TestCase testCase, TestResult testResult)
	{
		_frameworkHandle.RecordEnd(testCase, testResult.Outcome);
		_frameworkHandle.RecordResult(testResult);
	}

	private void EndTest(TestCase testCase, TestResult testResult, DateTime startTime)
	{
		testResult.StartTime = startTime;
		testResult.EndTime = DateTime.UtcNow;

		EndTest(testCase, testResult);
	}
}