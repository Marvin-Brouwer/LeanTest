using LeanTest.Dynamic.Invocation;

using System.Linq.Expressions;

namespace LeanTest.Dependencies.Wrappers;

internal class MemberSetup<TDependency> : IMemberSetup<TDependency>
	where TDependency : IDependency
{
	protected readonly TDependency Dependency;
	protected readonly ConfiguredMethod Method;
	protected readonly ConfiguredMethodSet ConfiguredMethods;

	internal MemberSetup(TDependency dependency, LambdaExpression method, ConfiguredMethodSet configuredMethods)
		: this(dependency, ConfiguredMethod.FromExpression(method, null), configuredMethods) { }

	internal MemberSetup(TDependency dependency, ConfiguredMethod method, ConfiguredMethodSet configuredMethods)
	{
		Dependency = dependency;
		Method = method;
		ConfiguredMethods = configuredMethods;
	}

	public TDependency Executes(Action callBack)
	{
		ConfiguredMethods.Add(Method with { ReturnDelegate = callBack });
		return Dependency;
	}
	public TDependency Executes()
	{
		ConfiguredMethods.Add(Method);
		return Dependency;
	}
}
internal class MemberSetup<TDependency, TReturn> : MemberSetup<TDependency>, IMemberSetup<TDependency, TReturn>
	where TDependency : IDependency
{
	internal MemberSetup(TDependency dependency, LambdaExpression method, ConfiguredMethodSet configuredMethods)
		: base(dependency, ConfiguredMethod.FromExpression(method, typeof(TReturn)), configuredMethods) { }

	public TDependency Returns(Func<TReturn> callBack)
	{
		ConfiguredMethods.Add(Method with { ReturnDelegate = callBack });
		return Dependency;
	}
	public TDependency Returns<T1>(Func<T1, TReturn> callBack)
	{
		ConfiguredMethods.Add(Method with { ReturnDelegate = callBack });
		return Dependency;
	}
}

