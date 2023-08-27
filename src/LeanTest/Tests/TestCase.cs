namespace LeanTest.Tests;

public readonly record struct TestCase(Func<ValueTask> TestBody): ITest
{
	// TODO this should be taskCompletionSource
	private static Func<ValueTask> Wrap(Action testAction) => async () =>
	{
		testAction();
		await Task.CompletedTask;
	};

	public TestCase(Action testBody) : this(Wrap(testBody)) { }
}
