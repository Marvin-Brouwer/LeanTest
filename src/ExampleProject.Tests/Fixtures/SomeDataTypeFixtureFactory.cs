using ExampleProject.Models;

using LeanTest.Dependencies;
using LeanTest.Dependencies.Factories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleProject.Tests.Fixtures;

internal static class SomeDataTypeFixtureFactory
{
	private static SomeDataType CreateDefault() => new()
	{
		Id = 1337,
		Name = "Foo",
	};

	public static IFixture<SomeDataType> ForSomeDataType(this IFixtureFactory factory) => factory.For(CreateDefault);

	public static IFixture<SomeDataType> WithName(this IFixture<SomeDataType> fixture, string name) =>
		fixture.AddMutation(data => data.Name = name);
	public static IFixture<SomeDataType> WithRealName(this IFixture<SomeDataType> fixture) =>
		fixture.AddMutation(data => data.Name = "Alice");
}
