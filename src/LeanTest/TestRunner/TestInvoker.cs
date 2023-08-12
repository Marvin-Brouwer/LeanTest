using LeanTest.Tests;

namespace LeanTest.TestRunner;

internal class TestInvoker
{
	public async Task RunTests(IEnumerable<ITestScenario> scenarios, CancellationToken cancellationToken)
	{
		// TODO Randomize
		var testTasks = InvokeTests(scenarios, cancellationToken);
		await Task.WhenAll(testTasks);
	}

	private static IEnumerable<Task> InvokeTests(IEnumerable<ITestScenario> scenarios, CancellationToken cancellationToken)
	{
		// TODO batch threading
		foreach (var scenario in scenarios)
		{
			if (cancellationToken.IsCancellationRequested) yield break;
			yield return scenario.Run(cancellationToken);
		}
	}
}
