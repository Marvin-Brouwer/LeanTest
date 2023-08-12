using LeanTest.Dependencies.Definitions;

using System.Linq.Expressions;

namespace LeanTest.Dependencies;

public interface IVerifyableDependency<TService, TDependency> : IDependency<TService>
	where TDependency : IVerifyableDependency<TService, TDependency>
{
	TDependency Verify(Expression<Func<TService>> member, ITimesConstraint times);
	TDependency Verify<TValue>(Expression<Func<TService, TValue>> member, ITimesConstraint times);
	TDependency Verify(Expression<Func<TService, Task>> member, ITimesConstraint times);
	TDependency Verify<TValue>(Expression<Func<TService, Task<TValue>>> member, ITimesConstraint times);
	TDependency VerifyOnce(Expression<Func<TService>> member);
	TDependency VerifyOnce<TValue>(Expression<Func<TService, TValue>> member);
	TDependency VerifyOnce(Expression<Func<TService, Task>> member);
	TDependency VerifyOnce<TValue>(Expression<Func<TService, Task<TValue>>> member);
	TDependency VerifyNever(Expression<Func<TService>> member);
	TDependency VerifyNever<TValue>(Expression<Func<TService, TValue>> member);
	TDependency VerifyNever(Expression<Func<TService, Task>> member);
	TDependency VerifyNever<TValue>(Expression<Func<TService, Task<TValue>>> member);

	void VerifyNoOtherCalls();
}
