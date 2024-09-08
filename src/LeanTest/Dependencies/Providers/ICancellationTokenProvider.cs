namespace LeanTest.Dependencies.Providers;

public interface ICancellationTokenProvider
{
	CancellationToken None { get; }
	CancellationToken Cancelled { get; }
	CancellationToken ForTest { get; }

	/// <summary>
	/// Cancels the <see cref="ForTest"/> token
	/// </summary>
	void CancelTestRun();
}