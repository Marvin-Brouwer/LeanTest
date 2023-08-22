using LeanTest.Dependencies.Configuration;

using System.Linq.Expressions;

namespace LeanTest.Dependencies;

public sealed class Stub<TService> :
	IConfigurableDependency<TService, Stub<TService>>
{
	private readonly ConfiguredMethodSet _configuredMethods;

	/// <inheritdoc />
	public TService Instance { get; }

	/// <summary>
	/// This is merely a shorthand for <see cref="Stub{TService}.Instance"/>
	/// </summary>
	public static TService operator ~(Stub<TService> stub) => stub.Instance;

	internal Stub(ConfiguredMethodSet configuredMethods, TService instance)
	{
		_configuredMethods = configuredMethods;
		Instance = instance;
	}

	public IMemberSetup<Stub<TService>> Setup(Expression<Action<TService>> member) =>
		new MemberSetup<Stub<TService>>(this, member, _configuredMethods);

	public IMemberSetup<Stub<TService>, TReturn> Setup<TReturn>(Expression<Func<TService, TReturn>> member) =>
		new MemberSetup<Stub<TService>, TReturn>(this, member, _configuredMethods);
}