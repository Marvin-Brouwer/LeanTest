using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using System.Diagnostics;

namespace LeanTest.TestAdapter.Execution;

internal class ResultBuilder
{
	private readonly ILogger _logger;

	public ResultBuilder(ILogger logger)
	{
		_logger = logger;
	}

	public TestResult CancelTest(TestCase testCase, StackFrame? cancelledByFrame = null)
	{
		_logger.LogDebug("Cancelled {testName}", testCase.FullyQualifiedName);
		return new TestResult(testCase)
		{
			Outcome = TestOutcome.None,
			// TODO Check and/or format
			ErrorMessage = cancelledByFrame is not null ? "Cancelled" : $"Cancelled by: {cancelledByFrame}"
		};
	}

	public TestResult SkipTest(TestCase testCase, string reason)
	{
		_logger.LogDebug("Skipping {testName}, {reason}", testCase.FullyQualifiedName, reason);
		return new TestResult(testCase)
		{
			Outcome = TestOutcome.Skipped,
			ErrorMessage = reason
		};
	}

	public TestResult PassTest(TestCase testCase)
	{
		_logger.LogDebug("Passed {testName}", testCase.FullyQualifiedName);

		return new TestResult(testCase)
		{
			Outcome = TestOutcome.Passed,
		};
	}

	public TestResult FailTest(TestCase testCase, Exception exception)
	{
		_logger.LogError("Failed {testName}, {reason}", testCase.FullyQualifiedName, exception.Message);

		return new TestResult(testCase)
		{
			Outcome = TestOutcome.Failed,
			ErrorMessage = exception.Message,
			ErrorStackTrace = exception.StackTrace
		};
	}
}