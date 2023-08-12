namespace LeanTest.Dependencies
{
    public interface IMock<TService> : 
        IConfigurableDependency<TService, IMock<TService>>, 
        IVerifyableDependency<TService, IMock<TService>>
    {

    }
}
