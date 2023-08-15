namespace LeanTest.Dependencies;

public interface IFixture<TClass> : IDependency<TClass>
{
	IFixture<TClass> AddMutation(Action<TClass> mutation);
	IFixture<TClass> AddMutation(Func<TClass, TClass> mutation);
}
