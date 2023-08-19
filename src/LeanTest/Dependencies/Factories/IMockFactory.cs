namespace LeanTest.Dependencies.Factories;

public interface IMockFactory
{
	IMock<TService> Of<TService>() where TService : class;
}