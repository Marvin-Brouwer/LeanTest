using ExampleProject.Models;

using LeanTest.Dependencies;
using LeanTest.Dependencies.Factories;

namespace ExampleProject.Tests.Fixtures;

internal static class SomeDataTypeFixtureFactory
{
	private static SomeDataType CreateDefault() => new()
	{
		Id = 1337,
		Name = "Foo",
	};

	public static Fixture<SomeDataType> ForSomeDataType(this IFixtureFactory factory) => factory.For(CreateDefault);

	public static Fixture<SomeDataType> WithName(this Fixture<SomeDataType> fixture, string name) =>
		fixture.AddMutation(data => data.Name = name);
	public static Fixture<SomeDataType> WithRealName(this Fixture<SomeDataType> fixture) =>
		fixture.AddMutation(data => data.Name = "Alice");
}
