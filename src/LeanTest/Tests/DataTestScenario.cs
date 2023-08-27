namespace LeanTest.Tests;

// TODO typed parameters
public readonly record struct DataTestScenario(
	IEnumerable<object?[]> TestData,
	Func<object?[], ValueTask> TestBody
): ITest
{

	// TODO this should be taskCompletionSource
	private static Func<object?[], ValueTask> Wrap(Action<object?[]> testAction) => async (data) =>
	{
		testAction(data);
		await Task.CompletedTask;
	};

	public DataTestScenario(IEnumerable<object?[]> testData, Action<object?[]> testAction) : this(testData, Wrap(testAction)) { }
}
