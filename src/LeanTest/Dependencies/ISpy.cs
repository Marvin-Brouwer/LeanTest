namespace LeanTest.Dependencies;

public interface ISpy<TService> : IVerifyableDependency<TService, ISpy<TService>>
{
}
