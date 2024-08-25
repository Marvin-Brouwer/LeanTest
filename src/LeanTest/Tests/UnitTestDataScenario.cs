using System.Collections.Immutable;

namespace LeanTest.Tests;

public sealed record class UnitTestDataScenario : Test
{
	public IReadOnlyList<object?[]> TestData { get; }

	public UnitTestDataScenario(
		IEnumerable<object?[]> testData,
		Delegate testBody
	)
		: base(testBody)
	{
		TestData = testData.ToImmutableArray();
	}
}
