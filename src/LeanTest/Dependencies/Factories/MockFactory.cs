using LeanTest.Dependencies.Wrappers;

namespace LeanTest.Dependencies.Factories
{
    internal readonly record struct MockFactory : IMockFactory
    {
        internal static readonly IMockFactory Instance = new MockFactory();

        IMock<TService> IMockFactory.Of<TService>() => new Mock<TService>();
    }
}
