namespace LeanTest.Tests.TestBody;

internal record TestDelegate(Delegate Delegate)
{

	public Task<object?> ExecuteAsync(object owner, CancellationToken cancellationToken)
	{
		return ExecuteAsync(owner, null, cancellationToken);
	}
	public async Task<object?> ExecuteAsync(object owner, IDictionary<string, (Type, object?)>? availableParams, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		// TODO Handle params
		var result = Delegate.Method.Invoke(owner, null);
		cancellationToken.ThrowIfCancellationRequested();

		if (result is Task<object?> objectTask) return await objectTask;
		if (result is Task voidTask)
		{
			await voidTask;
			return null;
		}

		return result;
	}
}
