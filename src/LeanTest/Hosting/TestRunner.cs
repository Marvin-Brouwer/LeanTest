using LeanTest.Tests;

using System.Security.Cryptography;

namespace LeanTest.Hosting;

internal static class TestRunner
{
	public static async Task RunTests(IReadOnlyList<ITestScenario> scenarios, CancellationToken cancellationToken)
	{
		var testTasks = InvokeTests(Shuffle(scenarios, cancellationToken), cancellationToken);
		await Task.WhenAll(testTasks);
	}

	private static IReadOnlyList<ITestScenario> Shuffle(IReadOnlyList<ITestScenario> scenarios, CancellationToken cancellationToken)
	{
		// https://stackoverflow.com/a/1262619/2319865
		var provider = RandomNumberGenerator.Create();
		var shuffledScenarios = new ITestScenario[scenarios.Count];

		int cursor = shuffledScenarios.Length;
		while (cursor > 1)
		{
			if (cancellationToken.IsCancellationRequested) return Array.Empty<ITestScenario>();

			var box = new byte[1];
			do provider.GetBytes(box);
			while (box[0] >= cursor * (byte.MaxValue / cursor));

			var selector = box[0] % cursor; cursor--;

			shuffledScenarios[selector] = scenarios[cursor];
			shuffledScenarios[cursor] = scenarios[selector];
		}

		return shuffledScenarios;
	}

	private static IEnumerable<Task> InvokeTests(IReadOnlyCollection<ITestScenario> scenarios, CancellationToken cancellationToken)
	{
		foreach (var scenario in scenarios)
		{
			if (cancellationToken.IsCancellationRequested) yield break;
			yield return scenario.Run(cancellationToken);
		}
	}
}
