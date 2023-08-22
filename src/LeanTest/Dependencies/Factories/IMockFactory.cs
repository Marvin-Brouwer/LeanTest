namespace LeanTest.Dependencies.Factories;

public interface IMockFactory
{
	Mock<TService> Of<TService>() where TService : class;
}