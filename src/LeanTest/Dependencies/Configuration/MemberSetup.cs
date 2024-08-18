using System.Linq.Expressions;

namespace LeanTest.Dependencies.Configuration;

internal class MemberSetup<TDependency> : IMemberSetup<TDependency>
	where TDependency : IDependency
{
	protected readonly TDependency Dependency;
	protected readonly LambdaExpression Method;
	protected readonly ConfiguredMethodSet ConfiguredMethods;

	internal MemberSetup(TDependency dependency, LambdaExpression method, ConfiguredMethodSet configuredMethods)
	{
		Dependency = dependency;
		Method = method;
		ConfiguredMethods = configuredMethods;
	}

	public TDependency Throws(Func<Exception> exception)
	{
		ConfiguredMethods.Add(ConfiguredMethod.ForException(Method, exception));
		return Dependency;
	}

	public TDependency Throws(Exception exception)
	{
		ConfiguredMethods.Add(ConfiguredMethod.ForException(Method, () => exception));
		return Dependency;
	}

	public TDependency Executes(Action callBack)
	{
		ConfiguredMethods.Add(ConfiguredMethod.ForCallback(Method, callBack));
		return Dependency;
	}
	public TDependency Executes()
	{
		ConfiguredMethods.Add(ConfiguredMethod.ForCallback(Method));
		return Dependency;
	}

	public TDependency Executes<T1>(Action<T1> callBack)
	{
		ConfiguredMethods.Add(ConfiguredMethod.ForCallback(Method, callBack));
		return Dependency;
	}
}
internal class MemberSetup<TDependency, TReturn> : MemberSetup<TDependency>, IMemberSetup<TDependency, TReturn>
	where TDependency : IDependency
{
	internal MemberSetup(TDependency dependency, LambdaExpression method, ConfiguredMethodSet configuredMethods)
		: base(dependency, method, configuredMethods) { }

	public TDependency Returns(TReturn returnValue)
	{
		ConfiguredMethods.Add(ConfiguredMethod.ForValue(Method, returnValue));
		return Dependency;
	}
	public TDependency Returns(Func<TReturn> callBack)
	{
		ConfiguredMethods.Add(ConfiguredMethod.ForCallback<TReturn>(Method, callBack));
		return Dependency;
	}
	public TDependency Returns<T1>(Func<T1, TReturn> callBack)
	{
		ConfiguredMethods.Add(ConfiguredMethod.ForCallback<TReturn>(Method, callBack));
		return Dependency;
	}
}

