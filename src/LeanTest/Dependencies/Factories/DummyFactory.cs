namespace LeanTest.Dependencies.Factories
{
    // TODO DummyWrapperProxy
    internal readonly record struct DummyFactory : IDummyFactory
    {
        internal static readonly IDummyFactory Instance = new DummyFactory();

        TService IDummyFactory.Of<TService>() => default!;
    }
}
