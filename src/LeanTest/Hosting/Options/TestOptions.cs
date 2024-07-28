namespace LeanTest.Hosting.Options;

public sealed class TestOptions
{
	public int Port { get; set; }
	// TODO test with values
	public IReadOnlyList<string>? TestPatterns { get; set; }
}
