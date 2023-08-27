using LeanTest.Tests;

using Microsoft.Extensions.Logging;

namespace LeanTest.Hosting;

internal class TestRunner
{
	private readonly ILogger<TestRunner> _logger;

	public TestRunner(ILogger<TestRunner> logger)
	{
		_logger = logger;
	}
	public async Task RunTests(IReadOnlyList<TestRun> tests, CancellationToken cancellationToken)
	{
		_logger.LogDebug("Running {0} tests", tests.Count);
		var testTasks = InvokeTests(tests.Shuffle(cancellationToken), cancellationToken);
		await Task.WhenAll(testTasks);
	}


	private IEnumerable<Task> InvokeTests(IReadOnlyCollection<TestRun> tests, CancellationToken cancellationToken)
	{
		foreach (var test in tests)
		{
			if (cancellationToken.IsCancellationRequested) yield break;
			_logger.LogInformation("Running {0} {1}", test.SuiteName, test.TestName);
			yield return test.Run(cancellationToken);
		}
	}
}
