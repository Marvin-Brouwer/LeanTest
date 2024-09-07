using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using System.Diagnostics;
using System.Text;

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
		var messageBuilder = new StringBuilder();

		var result = new TestResult(testCase)
		{
			Outcome = TestOutcome.Passed,
			ComputerName = Environment.MachineName
		};

		foreach (var log in testLogs)
		{
			result.Messages.Add(log);
			messageBuilder.AppendLine(log.Text);
		}

		// Because regular messages don't show up in the VS Test panel, we misuse the ErrorMessage for that
		result.ErrorMessage = messageBuilder.ToString();

		return result;
	}

	public TestResult FailTest(TestCase testCase, Exception exception, IReadOnlyList<TestResultMessage> testLogs)
	{
		_logger.LogError("Failed {testName}, {reason}", testCase.FullyQualifiedName, exception.Message);
		var messageBuilder = new StringBuilder();

		var result = new TestResult(testCase)
		{
			Outcome = TestOutcome.Failed,
			ComputerName = Environment.MachineName,
			// Because regular messages don't show up in the VS Test panel, we misuse the ErrorMessage for that
			// Because we don't have that, we prepend the Message to the stacktrace.
			ErrorStackTrace = exception.Message + Environment.NewLine + exception.StackTrace
		};

		foreach (var log in testLogs)
		{
			result.Messages.Add(log);
			messageBuilder.AppendLine(log.Text);
		}

		// Because regular messages don't show up in the VS Test panel, we misuse the ErrorMessage for that
		result.ErrorMessage = messageBuilder.ToString();

		return result;
	}
}