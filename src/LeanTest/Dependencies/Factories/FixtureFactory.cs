using LeanTest.Dependencies.Wrappers;

namespace LeanTest.Dependencies.Factories;

internal readonly record struct FixtureFactory : IFixtureFactory
{
	internal static readonly IFixtureFactory Instance = new FixtureFactory();

	public IFixture<TClass> For<TClass>()
		where TClass : notnull, new() =>
		new Fixture<TClass>(() => new TClass());

	public IFixture<TClass> For<TClass>(Func<TClass> defaultInstance)
		where TClass : notnull =>
		new Fixture<TClass>(defaultInstance);
}
