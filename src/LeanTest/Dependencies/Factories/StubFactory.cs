using LeanTest.Dependencies.Wrappers;

namespace LeanTest.Dependencies.Factories
{
    internal readonly record struct StubFacotry : IStubFactory
    {
        internal static readonly IStubFactory Instance = new StubFacotry();

        IStub<TService> IStubFactory.Of<TService>() => new Stub<TService>();
    }
}
