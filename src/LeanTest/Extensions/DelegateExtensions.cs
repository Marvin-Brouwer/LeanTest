namespace LeanTest.Extensions;

public static class DelegateExtensions
{
	public static Task<object?> ExecuteAsync(
		this Delegate delegateFunction, object owner, CancellationToken cancellationToken)
	{
		return delegateFunction.ExecuteAsync(owner, null, cancellationToken);
	}
	public static async Task<object?> ExecuteAsync(
		this Delegate delegateFunction, object owner, IDictionary<string, (Type, object?)>? availableParams, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		// TODO Handle params
		var result = delegateFunction.Method.Invoke(owner, null);
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
