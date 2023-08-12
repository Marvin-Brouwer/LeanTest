namespace LeanTest.Dependencies.Factories
{
    public interface ISpyFactory
    {
        ISpy<TService> On<TService>(TService service) where TService : notnull;
    }
}