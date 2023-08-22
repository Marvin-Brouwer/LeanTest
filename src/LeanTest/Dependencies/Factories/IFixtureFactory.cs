namespace LeanTest.Dependencies.Factories;

public interface IFixtureFactory
{
	Fixture<TClass> For<TClass>() where TClass : notnull, new();
	Fixture<TClass> For<TClass>(Func<TClass> defaultInstance) where TClass : notnull;
}