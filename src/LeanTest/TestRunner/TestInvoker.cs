using LeanTest.Tests;

using System.Security.Cryptography;

namespace LeanTest.TestRunner;

internal class TestInvoker
{
	public async Task RunTests(IEnumerable<ITestScenario> scenarios, CancellationToken cancellationToken)
	{
		var testTasks = InvokeTests(Shuffle(scenarios, cancellationToken), cancellationToken);
		await Task.WhenAll(testTasks);
	}

	private static IReadOnlyCollection<ITestScenario> Shuffle(IEnumerable<ITestScenario> scenarios, CancellationToken cancellationToken)
	{
		// https://stackoverflow.com/a/1262619/2319865
		var provider = RandomNumberGenerator.Create();
		var shuffledScenarios = scenarios.ToArray();

		int cursor = shuffledScenarios.Length;
		while (cursor > 1)
		{
			if (cancellationToken.IsCancellationRequested) return Array.Empty<ITestScenario>();

			var box = new byte[1];
			do provider.GetBytes(box);
			while (box[0] >= cursor * (byte.MaxValue / cursor));

			var selector = box[0] % cursor; cursor--;

			var value = shuffledScenarios[selector];
			shuffledScenarios[selector] = shuffledScenarios[cursor];
			shuffledScenarios[cursor] = value;
		}

		return shuffledScenarios;
	}

	private static IEnumerable<Task> InvokeTests(IReadOnlyCollection<ITestScenario> scenarios, CancellationToken cancellationToken)
	{
		// TODO batch threading
		foreach (var scenario in scenarios)
		{
			if (cancellationToken.IsCancellationRequested) yield break;
			yield return scenario.Run(cancellationToken);
		}
	}
}
