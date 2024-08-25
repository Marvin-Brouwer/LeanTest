using LeanTest.Dependencies.Configuration;
using LeanTest.Dependencies.Definitions;

using System.Linq.Expressions;
using System.Reflection;

namespace LeanTest.Dependencies.Verification;

// TODO Implement like TimesContstraint
internal class InvocationChecker
{
	private readonly InvocationRecordList _invocationRecordList;

	public InvocationChecker(InvocationRecordList invocationRecordList)
	{
		_invocationRecordList = invocationRecordList;
	}

	private void Verify(ITimesConstraint timesConstraint, MethodInfo method, ConfiguredParametersCollection parameters)
	{
		var invocations = _invocationRecordList.Count(method, parameters);
		var constraintMatch = timesConstraint.VerifyInvocations(invocations, method.DeclaringType!.Name + "." + method.Name);
		if (constraintMatch is not null) throw constraintMatch;
	}

	public void Verify<TService>(ITimesConstraint timesConstraint, Expression<Action<TService>> member)
	{
		var (method, parameters) = member.GetMethodFromExpression();
		Verify(timesConstraint, method, parameters);
	}

	public void Verify<TService, TValue>(ITimesConstraint timesConstraint, Expression<Func<TService, TValue>> member)
	{
		var (method, parameters) = member.GetMethodFromExpression();
		Verify(timesConstraint, method, parameters);
	}
	public void Verify<TService>(ITimesConstraint timesConstraint, Expression<Func<TService, Task>> member)
	{
		var (method, parameters) = member.GetMethodFromExpression();
		Verify(timesConstraint, method, parameters);
	}
	public void Verify<TService, TValue>(ITimesConstraint timesConstraint, Expression<Func<TService, Task<TValue>>> member)
	{
		var (method, parameters) = member.GetMethodFromExpression();
		Verify(timesConstraint, method, parameters);
	}
	public void VerifyNoOtherCalls()
	{
		if (_invocationRecordList.HasUncheckedItems)
			throw new ConstraintVerficationFaillure(
				"No other calls were expected. However, the records still contain some unverified invocations."
			);
	}
	public void VerifyNoCalls()
	{
		if (_invocationRecordList.HasItems)
			throw new ConstraintVerficationFaillure(
				"No calls were expected. However, at least some invocations were recorded."
			);
	}
}
