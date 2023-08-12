namespace LeanTest.Dependencies;

public interface IDependency<out TService>
{
	TService Instance { get; }
}
