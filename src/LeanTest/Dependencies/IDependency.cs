namespace LeanTest.Dependencies;

public interface IDependency
{

}
public interface IDependency<out TService> : IDependency
{
	TService Instance { get; }
}
