namespace LeanTest.Dependencies.Providers;

public interface ICancellationTokenProvider
{
	CancellationToken None { get; }
	CancellationToken Cancelled { get; }
	CancellationToken ForTest { get; }
}