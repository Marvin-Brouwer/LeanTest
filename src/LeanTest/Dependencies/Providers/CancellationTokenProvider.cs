using LeanTest.Dependencies.Definitions;

namespace LeanTest.Dependencies.Providers;

internal readonly record struct CancellationTokenProvider : ICancellationTokenProvider
{
	internal static readonly ICancellationTokenProvider Instance = new CancellationTokenProvider();

	internal CancellationTokenProvider(CancellationToken testCancellationToken)
	{
		ForTest = testCancellationToken;
	}

	public CancellationToken None => CancellationToken.None;
	public CancellationToken Cancelled => new CancellationToken(true);
	public CancellationToken ForTest { get;  }
}