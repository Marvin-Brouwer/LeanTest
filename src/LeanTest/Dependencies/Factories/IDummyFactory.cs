namespace LeanTest.Dependencies.Factories;

public interface IDummyFactory
{
	TService Of<TService>() where TService : notnull;
}