namespace LeanTest.Dependencies.Providers;

internal readonly record struct CancellationTokenProvider : ICancellationTokenProvider
{
	internal static readonly ICancellationTokenProvider Instance = new CancellationTokenProvider();

	internal CancellationTokenProvider(CancellationToken testCancellationToken)
	{
		_cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(testCancellationToken);
		ForTest = _cancellationTokenSource.Token;
	}

	public CancellationToken None => CancellationToken.None;
	public CancellationToken Cancelled => new CancellationToken(true);

	private readonly CancellationTokenSource _cancellationTokenSource;

	public CancellationToken ForTest { get;  }

	public void CancelTestRun()
	{
		_cancellationTokenSource.Cancel();
	}
}