namespace LeanTest.Tests;

internal class TestScenario : ITestScenario
{
	private readonly ITestArangement? _arrange;
	private readonly ITestAction? _act;
	private readonly ITestAssertion _assert;

	private readonly Type _suiteType;

	internal string Name { get; }
	public Type SuiteType { get; }

	public TestScenario(Type suiteType,  string scenarioName, ITestArangement? arrange, ITestAction? act, ITestAssertion assert)
	{
		Name = scenarioName;
		SuiteType = suiteType;

		_arrange = arrange;
		_act = act;
		_assert = assert;
	}

	public async Task Run(CancellationToken cancellationToken)
	{
		IDictionary<string, (Type, object?)> parameters = new Dictionary<string, (Type, object?)>();
		ITestSuite suite;

		try
		{
			// TODO see if this is threadsafe, otherwise make it so
			suite = (ITestSuite)Activator.CreateInstance(_suiteType)!;
		}
		catch (Exception ex)
		{
			// TODO Wrap and give context, should be rare
			throw;
		}

		try
		{
			if (_arrange is not null)
				parameters = await _arrange.CallArrange(suite, _act?.GetParameters(), cancellationToken);
		}
		catch (Exception ex)
		{
			// TODO Wrap and give context
			throw;
		}
		try
		{
			if (_act is not null)
				await _act.CallAct(suite, parameters, cancellationToken);
		}
		catch (Exception ex)
		{
			// TODO Wrap and give context
			throw;
		}
		try
		{
			if (_assert is not null)
				await _assert.CallAssert(suite, parameters, cancellationToken);
		}
		catch (Exception ex)
		{
			// TODO Wrap and give context
			throw;
		}
	}
}