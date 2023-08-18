using LeanTest.Dependencies;
using LeanTest.Dependencies.Configuration;

using System.Linq.Expressions;


internal class Stub<TService> : IStub<TService>
{
	private readonly ConfiguredMethodSet _configuredMethods;

	public Stub(ConfiguredMethodSet configuredMethods)
	{
		_configuredMethods = configuredMethods;
	}

	public TService Instance { get; internal init; } = default!;

	public IMemberSetup<IStub<TService>> Setup(Expression<Action<TService>> member) =>
		new MemberSetup<IStub<TService>>(this, member, _configuredMethods);

	public IMemberSetup<IStub<TService>, TReturn> Setup<TReturn>(Expression<Func<TService, TReturn>> member) =>
		new MemberSetup<IStub<TService>, TReturn>(this, member, _configuredMethods);
}
