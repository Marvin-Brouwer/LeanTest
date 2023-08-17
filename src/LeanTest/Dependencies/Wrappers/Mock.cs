using LeanTest.Dependencies.Definitions;
using LeanTest.Dependencies.Providers;
using LeanTest.Dynamic.Invocation;

using System.Linq.Expressions;

namespace LeanTest.Dependencies.Wrappers;

internal class Mock<TService> : IMock<TService>
{
	private readonly ISpy<TService> _spy;
	private readonly ConfiguredMethodSet _configuredMethods;

	internal Mock(ConfiguredMethodSet configuredMethods, ISpy<TService> spy)
	{
		_configuredMethods = configuredMethods;
		_spy = spy;
	}

	public TService Instance { get; } = default!;

	public IMemberSetup<IMock<TService>> Setup(Expression<Action<TService>> member) =>
		new MemberSetup<IMock<TService>>(this, member, _configuredMethods);

	public IMemberSetup<IMock<TService>, TReturn> Setup<TReturn>(Expression<Func<TService, TReturn>> member) =>
		new MemberSetup<IMock<TService>, TReturn>(this, member, _configuredMethods);

	public IMock<TService> Verify(Expression<Func<TService>> member, ITimesConstraint times)
	{
		_spy.Verify(member, times);
		return this;
	}

	public IMock<TService> Verify<TValue>(Expression<Func<TService, TValue>> member, ITimesConstraint times)
	{
		_spy.Verify(member, times);
		return this;
	}

	public IMock<TService> Verify(Expression<Func<TService, Task>> member, ITimesConstraint times)
	{
		_spy.Verify(member, times);
		return this;
	}

	public IMock<TService> Verify<TValue>(Expression<Func<TService, Task<TValue>>> member, ITimesConstraint times)
	{
		_spy.Verify(member, times);
		return this;
	}

	public IMock<TService> VerifyOnce(Expression<Func<TService>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Once);

	public IMock<TService> VerifyOnce<TValue>(Expression<Func<TService, TValue>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Once);

	public IMock<TService> VerifyOnce(Expression<Func<TService, Task>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Once);

	public IMock<TService> VerifyOnce<TValue>(Expression<Func<TService, Task<TValue>>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Once);

	public IMock<TService> VerifyNever(Expression<Func<TService>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Never);

	public IMock<TService> VerifyNever<TValue>(Expression<Func<TService, TValue>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Never);

	public IMock<TService> VerifyNever(Expression<Func<TService, Task>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Never);

	public IMock<TService> VerifyNever<TValue>(Expression<Func<TService, Task<TValue>>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Never);

	public void VerifyNoOtherCalls() =>
		_spy.VerifyNoOtherCalls();
}
