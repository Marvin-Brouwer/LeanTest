using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using System.Diagnostics;

namespace LeanTest.Hosting.TestAdapter;

internal class TestResultBuilder
{
	private readonly ILogger<TestResultBuilder> _logger;

	public TestResultBuilder(ILogger<TestResultBuilder> logger)
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

	public TestResult PassTest(TestCase testCase, IReadOnlyList<TestResultMessage> testLogs)
	{
		_logger.LogDebug("Passed {testName}", testCase.FullyQualifiedName);

		var result = new TestResult(testCase)
		{
			Outcome = TestOutcome.Passed,
			ComputerName = Environment.MachineName
		};

		// TODO figure out why messages don't work;
		foreach (var log in testLogs)
		{
			result.Messages.Add(log);
		}


		return result;
	}

	public TestResult FailTest(TestCase testCase, Exception exception, IReadOnlyList<TestResultMessage> testLogs)
	{
		_logger.LogError("Failed {testName}, {reason}", testCase.FullyQualifiedName, exception.Message);

		var result = new TestResult(testCase)
		{
			Outcome = TestOutcome.Failed,
			ComputerName = Environment.MachineName,
			ErrorMessage = exception.Message,
			ErrorStackTrace = exception.StackTrace
		};

		// TODO figure out why messages don't work;
		foreach (var log in testLogs)
		{
			result.Messages.Add(log);
		}

		return result;
	}
}