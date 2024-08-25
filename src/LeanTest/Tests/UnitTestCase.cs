namespace LeanTest.Tests;

public sealed record class UnitTestCase : Test
{
	public UnitTestCase(Delegate testBody) : base(testBody) { }
}
