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
	private readonly InvocationChecker _invocationChecker;
	private readonly ConfiguredMethodSet _configuredMethods;

	/// <inheritdoc />
	public TService Instance { get; }

	/// <summary>
	/// This is merely a shorthand for <see cref="Mock{TService}.Instance"/>
	/// </summary>
	public static TService operator ~(Mock<TService> mock) => mock.Instance;

	internal Mock(ConfiguredMethodSet configuredMethods, InvocationRecordList invocationRecordList, TService instance)
	{
		// TODO, this probably isn't threadsafe, perhaps it's cool to have a context associated with the current thread,
		// or in other words the current Test(() => ....)
		_configuredMethods = configuredMethods;
		// TODO, this probably isn't threadsafe, perhaps it's cool to have a context associated with the current thread,
		// or in other words the current Test(() => ....)
		_invocationChecker = new InvocationChecker(invocationRecordList);
		Instance = instance;
	}

	public IMemberSetup<Mock<TService>> Setup(Expression<Action<TService>> member) =>
		new MemberSetup<Mock<TService>>(this, member, _configuredMethods);

	public IMemberSetup<Mock<TService>, TReturn> Setup<TReturn>(Expression<Func<TService, TReturn>> member) =>
		new MemberSetup<Mock<TService>, TReturn>(this, member, _configuredMethods);

	public Mock<TService> Verify(ITimesConstraint times, Expression<Action<TService>> member)
	{
		_invocationChecker.Verify(times, member);
		return this;
	}
	public Mock<TService> Verify<TValue>(ITimesConstraint times, Expression<Func<TService, TValue>> member)
	{
		_invocationChecker.Verify(times, member);
		return this;
	}

	public Mock<TService> Verify(ITimesConstraint times, Expression<Func<TService, Task>> member)
	{
		_invocationChecker.Verify(times, member);
		return this;
	}

	public Mock<TService> Verify<TValue>(ITimesConstraint times, Expression<Func<TService, Task<TValue>>> member)
	{
		_invocationChecker.Verify(times, member);
		return this;
	}

	public Mock<TService> VerifyOnce(Expression<Action<TService>> member) =>
		Verify(TimesContstraintProvider.Instance.Once, member);

	public Mock<TService> VerifyOnce<TValue>(Expression<Func<TService, TValue>> member) =>
		Verify(TimesContstraintProvider.Instance.Once, member);

	public Mock<TService> VerifyOnce(Expression<Func<TService, Task>> member) =>
		Verify(TimesContstraintProvider.Instance.Once, member);

	public Mock<TService> VerifyOnce<TValue>(Expression<Func<TService, Task<TValue>>> member) =>
		Verify(TimesContstraintProvider.Instance.Once, member);

	public Mock<TService> VerifyExactly(uint amountOfTimes, Expression<Action<TService>> member) =>
		Verify(TimesContstraintProvider.Instance.Exactly(amountOfTimes), member);
	public Mock<TService> VerifyExactly<TValue>(uint amountOfTimes, Expression<Func<TService, TValue>> member) =>
		Verify(TimesContstraintProvider.Instance.Exactly(amountOfTimes), member);
	public Mock<TService> VerifyExactly(uint amountOfTimes, Expression<Func<TService, Task>> member) =>
		Verify(TimesContstraintProvider.Instance.Exactly(amountOfTimes), member);
	public Mock<TService> VerifyExactly<TValue>(uint amountOfTimes, Expression<Func<TService, Task<TValue>>> member) =>
		Verify(TimesContstraintProvider.Instance.Exactly(amountOfTimes), member);
	public Mock<TService> VerifyAtLeast(uint amountOfTimes, Expression<Action<TService>> member) =>
		Verify(TimesContstraintProvider.Instance.AtLeast(amountOfTimes), member);
	public Mock<TService> VerifyAtLeast<TValue>(uint amountOfTimes, Expression<Func<TService, TValue>> member) =>
		Verify(TimesContstraintProvider.Instance.AtLeast(amountOfTimes), member);
	public Mock<TService> VerifyAtLeast(uint amountOfTimes, Expression<Func<TService, Task>> member) =>
		Verify(TimesContstraintProvider.Instance.AtLeast(amountOfTimes), member);
	public Mock<TService> VerifyAtLeast<TValue>(uint amountOfTimes, Expression<Func<TService, Task<TValue>>> member) =>
		Verify(TimesContstraintProvider.Instance.AtLeast(amountOfTimes), member);
	public Mock<TService> VerifyAtMost(uint amountOfTimes, Expression<Action<TService>> member) =>
		Verify(TimesContstraintProvider.Instance.AtMost(amountOfTimes), member);
	public Mock<TService> VerifyAtMost<TValue>(uint amountOfTimes, Expression<Func<TService, TValue>> member) =>
		Verify(TimesContstraintProvider.Instance.AtMost(amountOfTimes), member);
	public Mock<TService> VerifyAtMost(uint amountOfTimes, Expression<Func<TService, Task>> member) =>
		Verify(TimesContstraintProvider.Instance.AtMost(amountOfTimes), member);
	public Mock<TService> VerifyAtMost<TValue>(uint amountOfTimes, Expression<Func<TService, Task<TValue>>> member) =>
		Verify(TimesContstraintProvider.Instance.AtMost(amountOfTimes), member);
	public Mock<TService> VerifyBetween(uint leastAmountOfTimes, uint mostAmountOfTimes, Expression<Action<TService>> member) =>
		VerifyBetween(leastAmountOfTimes, mostAmountOfTimes, false, member);
	public Mock<TService> VerifyBetween<TValue>(uint leastAmountOfTimes, uint mostAmountOfTimes, Expression<Func<TService, TValue>> member) =>
		VerifyBetween(leastAmountOfTimes, mostAmountOfTimes, false, member);
	public Mock<TService> VerifyBetween(uint leastAmountOfTimes, uint mostAmountOfTimes, Expression<Func<TService, Task>> member) =>
		VerifyBetween(leastAmountOfTimes, mostAmountOfTimes, false, member);
	public Mock<TService> VerifyBetween<TValue>(uint leastAmountOfTimes, uint mostAmountOfTimes, Expression<Func<TService, Task<TValue>>> member) =>
		VerifyBetween(leastAmountOfTimes, mostAmountOfTimes, false, member);
	public Mock<TService> VerifyBetween(uint leastAmountOfTimes, uint mostAmountOfTimes, bool inclusive, Expression<Action<TService>> member) =>
		Verify(TimesContstraintProvider.Instance.Between(leastAmountOfTimes, mostAmountOfTimes, inclusive), member);
	public Mock<TService> VerifyBetween<TValue>(uint leastAmountOfTimes, uint mostAmountOfTimes, bool inclusive, Expression<Func<TService, TValue>> member) =>
		Verify(TimesContstraintProvider.Instance.Between(leastAmountOfTimes, mostAmountOfTimes, inclusive), member);
	public Mock<TService> VerifyBetween(uint leastAmountOfTimes, uint mostAmountOfTimes, bool inclusive, Expression<Func<TService, Task>> member) =>
		Verify(TimesContstraintProvider.Instance.Between(leastAmountOfTimes, mostAmountOfTimes, inclusive), member);
	public Mock<TService> VerifyBetween<TValue>(uint leastAmountOfTimes, uint mostAmountOfTimes, bool inclusive, Expression<Func<TService, Task<TValue>>> member) =>
		Verify(TimesContstraintProvider.Instance.Between(leastAmountOfTimes, mostAmountOfTimes, inclusive), member);

	public Mock<TService> VerifyNever(Expression<Action<TService>> member) =>
		Verify(TimesContstraintProvider.Instance.Never, member);

	public Mock<TService> VerifyNever<TValue>(Expression<Func<TService, TValue>> member) =>
		Verify(TimesContstraintProvider.Instance.Never, member);

	public Mock<TService> VerifyNever(Expression<Func<TService, Task>> member) =>
		Verify(TimesContstraintProvider.Instance.Never, member);

	public Mock<TService> VerifyNever<TValue>(Expression<Func<TService, Task<TValue>>> member) =>
		Verify(TimesContstraintProvider.Instance.Never, member);

	public void VerifyNoOtherCalls() => _invocationChecker.VerifyNoOtherCalls();
	public void VerifyNoCalls() => _invocationChecker.VerifyNoCalls();
}
