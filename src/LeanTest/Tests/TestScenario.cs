namespace LeanTest.Tests;

internal class TestScenario : ITestScenario
{
	private readonly ITestArangement _arrange;
	private readonly ITestAction _act;
	private readonly ITestAssertion _assert;

	private readonly Type _suiteType;

	public Type ServiceType { get; }
	internal string Name { get; }

	public TestScenario(Type suiteType, Type serviceType, string scenarioName, ITestArangement arrange, ITestAction act, ITestAssertion assert)
	{
		ServiceType = serviceType;
		Name = scenarioName;

		_suiteType = suiteType;

		_arrange = arrange;
		_act = act;
		_assert = assert;
	}

	public async Task Run(CancellationToken cancellationToken)
	{
		IDictionary<string, (Type, object?)> parameters;
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
			parameters = await _arrange.CallArrange(suite, _act.GetParameters(), cancellationToken);
		}
		catch (Exception ex)
		{
			// TODO Wrap and give context
			throw;
		}
		try
		{
			await _act.CallAct(suite, parameters, cancellationToken);
		}
		catch (Exception ex)
		{
			// TODO Wrap and give context
			throw;
		}
		try
		{
			await _assert.CallAssert(suite, parameters, cancellationToken);
		}
		catch (Exception ex)
		{
			// TODO Wrap and give context
			throw;
		}
	}
}