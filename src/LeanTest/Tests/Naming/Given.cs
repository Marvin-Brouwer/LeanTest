namespace LeanTest.Tests.Naming;

public readonly record struct Given(For? For, string Value) : ITestNamePart
{
	public string Name => String.Join("_", For?.Name, Value);

	public When When(string value) => new(this, value);
	public Then Then(string value) => new(this, value);
}
