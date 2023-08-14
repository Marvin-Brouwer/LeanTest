namespace LeanTest.Dependencies.Factories;

public interface IStubFactory
{
	IStub<TService> Of<TService>() where TService : class;
}