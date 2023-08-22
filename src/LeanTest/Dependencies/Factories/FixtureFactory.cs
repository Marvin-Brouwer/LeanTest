namespace LeanTest.Dependencies.Factories;

internal readonly record struct FixtureFactory : IFixtureFactory
{
	internal static readonly IFixtureFactory Instance = new FixtureFactory();

	public Fixture<TClass> For<TClass>()
		where TClass : notnull, new() =>
		new Fixture<TClass>(() => new TClass());

	public Fixture<TClass> For<TClass>(Func<TClass> defaultInstance)
		where TClass : notnull =>
		new Fixture<TClass>(defaultInstance);
}
