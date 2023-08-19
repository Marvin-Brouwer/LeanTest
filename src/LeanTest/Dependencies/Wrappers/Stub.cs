using LeanTest.Dependencies.Configuration;

using System.Linq.Expressions;

namespace LeanTest.Dependencies.Wrappers;

internal class Stub<TService> : IStub<TService>
{
	private readonly ConfiguredMethodSet _configuredMethods;

	public Stub(ConfiguredMethodSet configuredMethods, TService instance)
	{
		_configuredMethods = configuredMethods;
		Instance = instance;
	}

	public TService Instance { get; }

	public IMemberSetup<IStub<TService>> Setup(Expression<Action<TService>> member) =>
		new MemberSetup<IStub<TService>>(this, member, _configuredMethods);

	public IMemberSetup<IStub<TService>, TReturn> Setup<TReturn>(Expression<Func<TService, TReturn>> member) =>
		new MemberSetup<IStub<TService>, TReturn>(this, member, _configuredMethods);
}