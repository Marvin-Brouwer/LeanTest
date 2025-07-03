namespace LeanTest.Tests;

internal readonly record struct TestRun(
	Delegate TestBody,
	string TestName,
	string SuiteName
)
{
	//public async Task Run(CancellationToken cancellationToken)
	//{
	//	IDictionary<string, (Type, object?)> parameters = new Dictionary<string, (Type, object?)>();
	//	ITestSuite suite;

	//	try
	//	{
	//		// TODO see if this is threadsafe, otherwise make it so
	//		suite = (ITestSuite)Activator.CreateInstance(_suiteType)!;
	//	}
	//	catch (Exception ex)
	//	{
	//		// TODO Wrap and give context, should be rare
	//		throw;
	//	}

	//	try
	//	{
	//		if (_arrange is not null)
	//			parameters = await _arrange.CallArrange(suite, _act?.GetParameters(), cancellationToken);
	//	}
	//	catch (Exception ex)
	//	{
	//		// TODO Wrap and give context
	//		throw;
	//	}
	//	try
	//	{
	//		if (_act is not null)
	//			await _act.CallAct(suite, parameters, cancellationToken);
	//	}
	//	catch (Exception ex)
	//	{
	//		// TODO Wrap and give context
	//		throw;
	//	}
	//	try
	//	{
	//		if (_assert is not null)
	//			await _assert.CallAssert(suite, parameters, cancellationToken);
	//	}
	//	catch (Exception ex)
	//	{
	//		// TODO Wrap and give context
	//		throw;
	//	}
	//}
	public async Task Run(CancellationToken cancellationToken)
	{
		try
		{
			await (Task)TestBody.DynamicInvoke()!;
			_ = cancellationToken;
		}
		catch (Exception ex)
		{
			// TODO
			//throw;
			_ = ex;
		} 
	}
}