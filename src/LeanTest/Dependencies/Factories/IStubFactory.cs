namespace LeanTest.Dependencies.Factories;

public interface IStubFactory
{
	Stub<TService> Of<TService>() where TService : class;
}