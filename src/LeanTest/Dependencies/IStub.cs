namespace LeanTest.Dependencies
{
    public interface IStub<TService> : IConfigurableDependency<TService, IStub<TService>>
    {

    }
}
