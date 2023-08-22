namespace LeanTest.Dependencies.Factories;

public interface ISpyFactory
{
	Spy<TService> On<TService>(TService service)
		where TService : class;
}