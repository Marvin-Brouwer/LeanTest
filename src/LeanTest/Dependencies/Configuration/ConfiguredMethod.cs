using System.Linq.Expressions;
using System.Reflection;

namespace LeanTest.Dependencies.Configuration;

internal abstract record ConfiguredMethod(
	MethodInfo Method, ParameterInfo[] Parameters, Type? ReturnType
)
{
	internal static ConfiguredMethod ForCallback(LambdaExpression member, Delegate? callbackDelegate = null)
	{
		var (method, parameters) = GetMethodFromExpression(member);
		return new ConfiguredVoidMethod(method, parameters, callbackDelegate);
	}
	internal static ConfiguredMethod ForCallback<TReturn>(LambdaExpression member, Delegate returnDelegate)
	{
		var (method, parameters) = GetMethodFromExpression(member);
		return new ConfiguredLambdaMethod(method, parameters, typeof(TReturn), returnDelegate);
	}
	internal static ConfiguredMethod ForValue<TReturn>(LambdaExpression member, TReturn returnValue)
	{
		var (method, parameters) = GetMethodFromExpression(member);
		return new ConfiguredValueMethod(method, parameters, typeof(TReturn), returnValue);
	}

	private static (MethodInfo method, ParameterInfo[] parameters) GetMethodFromExpression(LambdaExpression member)
	{
		if (member.Body is not MethodCallExpression methodExpression)
		{
			// TODO, better exception
			throw new NotSupportedException();
		}
		//new System.Linq.Expressions.Expression.MethodCallExpressionProxy(new System.Linq.Expressions.Expression.LambdaExpressionProxy(member).Body).Method
		// TODO figure these out
		var method = methodExpression.Method;
		var parameters = method.GetParameters();

		return (method, parameters);
	}

	/// <summary>
	/// Wrap delegates in an <see cref="Invoke(object?[])"/> method so the amount of parameters no longer
	/// throws an exception when not matching <br />
	/// This is only really useful for the <see cref="ConfiguredValueMethod"/> <br/>
	/// However, this allows the parameter check to be put in here in stead of the Marshall
	/// </summary>
	public abstract object? Invoke(params object?[] parameters);
}

internal sealed record ConfiguredValueMethod(
	MethodInfo Method, ParameterInfo[] Parameters, Type? ReturnType, object? ReturnValue
) : ConfiguredMethod(Method, Parameters, ReturnType)
{
	public override object? Invoke(params object?[] parameters) => ReturnValue;
}

internal sealed record ConfiguredLambdaMethod(
	MethodInfo Method, ParameterInfo[] Parameters, Type? ReturnType, Delegate? returnDelegate
) : ConfiguredMethod(Method, Parameters, ReturnType)
{
	public override object? Invoke(params object?[] parameters)
	{
		if (returnDelegate is null) return null;

		return parameters.Length == 0
			? returnDelegate.DynamicInvoke(null)!
			: returnDelegate.DynamicInvoke(parameters)!;
	}
}
internal sealed record ConfiguredVoidMethod(
	MethodInfo Method, ParameterInfo[] Parameters, Delegate? callbackDelegate
) : ConfiguredMethod(Method, Parameters, typeof(void))
{
	public override object? Invoke(params object?[] parameters)
	{
		if (callbackDelegate is null) return null;

		return parameters.Length == 0
			? callbackDelegate.DynamicInvoke(null)!
			: callbackDelegate.DynamicInvoke(parameters)!;
	}
}