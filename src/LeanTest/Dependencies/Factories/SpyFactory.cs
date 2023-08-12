using LeanTest.Dependencies.Wrappers;

namespace LeanTest.Dependencies.Factories;

internal readonly record struct SpyFactory : ISpyFactory
{
	internal static readonly ISpyFactory Instance = new SpyFactory();

	ISpy<TService> ISpyFactory.On<TService>(TService service) => new Spy<TService>(service);
}
