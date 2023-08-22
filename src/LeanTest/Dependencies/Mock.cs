using LeanTest.Dependencies.Configuration;
using LeanTest.Dependencies.Definitions;
using LeanTest.Dependencies.Providers;
using LeanTest.Dependencies.Verification;

using System.Linq.Expressions;

namespace LeanTest.Dependencies;

public sealed class Mock<TService> :
	IConfigurableDependency<TService, Mock<TService>>,
	IVerifyableDependency<TService, Mock<TService>>
{
	private readonly InvocationRecordList _invocationRecordList;
	private readonly ConfiguredMethodSet _configuredMethods;

	/// <summary>
	/// This is merely a shorthand for <see cref="Mock{TService}.Instance"/>
	/// </summary>
	public static TService operator ~(Mock<TService> mock) => mock.Instance;

	internal Mock(ConfiguredMethodSet configuredMethods, InvocationRecordList invocationRecordList, TService instance)
	{
		_configuredMethods = configuredMethods;
		_invocationRecordList = invocationRecordList;
		Instance = instance;
	}

	/// <inheritdoc />
	public TService Instance { get; }

	public IMemberSetup<Mock<TService>> Setup(Expression<Action<TService>> member) =>
		new MemberSetup<Mock<TService>>(this, member, _configuredMethods);

	public IMemberSetup<Mock<TService>, TReturn> Setup<TReturn>(Expression<Func<TService, TReturn>> member) =>
		new MemberSetup<Mock<TService>, TReturn>(this, member, _configuredMethods);

	public Mock<TService> Verify(Expression<Func<TService>> member, ITimesConstraint times)
	{
		// TODO
		return this;
	}

	public Mock<TService> Verify<TValue>(Expression<Func<TService, TValue>> member, ITimesConstraint times)
	{
		// TODO
		return this;
	}

	public Mock<TService> Verify(Expression<Func<TService, Task>> member, ITimesConstraint times)
	{
		// TODO
		return this;
	}

	public Mock<TService> Verify<TValue>(Expression<Func<TService, Task<TValue>>> member, ITimesConstraint times)
	{
		// TODO
		return this;
	}

	public Mock<TService> VerifyOnce(Expression<Func<TService>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Once);

	public Mock<TService> VerifyOnce<TValue>(Expression<Func<TService, TValue>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Once);

	public Mock<TService> VerifyOnce(Expression<Func<TService, Task>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Once);

	public Mock<TService> VerifyOnce<TValue>(Expression<Func<TService, Task<TValue>>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Once);

	public Mock<TService> VerifyNever(Expression<Func<TService>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Never);

	public Mock<TService> VerifyNever<TValue>(Expression<Func<TService, TValue>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Never);

	public Mock<TService> VerifyNever(Expression<Func<TService, Task>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Never);

	public Mock<TService> VerifyNever<TValue>(Expression<Func<TService, Task<TValue>>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Never);

	public void VerifyNoOtherCalls()
	{
		// TODO
	}
}
