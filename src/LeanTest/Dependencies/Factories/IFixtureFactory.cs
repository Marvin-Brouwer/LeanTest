namespace LeanTest.Dependencies.Factories;

public interface IFixtureFactory
{
	IFixture<TClass> For<TClass>() where TClass : notnull, new();
	IFixture<TClass> For<TClass>(Func<TClass> defaultInstance) where TClass : notnull;
}