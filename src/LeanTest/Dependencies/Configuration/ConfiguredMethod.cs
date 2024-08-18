using System.Linq.Expressions;
using System.Reflection;

namespace LeanTest.Dependencies.Configuration;

internal abstract record ConfiguredMethod(MethodBase Method, ParameterInfo[] Parameters, Type ReturnType)
	: MethodDefinition(Method, Parameters, ReturnType)
{	internal static ConfiguredMethod ForCallback(LambdaExpression member, Delegate? callbackDelegate = null)
	{
		var (method, parameters) = GetMethodFromExpression(member);
		return new ConfiguredVoidMethod(method, parameters, callbackDelegate);
	}
	internal static ConfiguredMethod ForCallback<TReturn>(LambdaExpression member, Delegate returnDelegate)
	{
		var (method, parameters) = GetMethodFromExpression(member);
		return new ConfiguredLambdaMethod(method, parameters, typeof(TReturn), returnDelegate);
	}

	internal static ConfiguredMethod ForException(LambdaExpression member, Func<Exception> exception)
	{
		var (method, parameters) = GetMethodFromExpression(member);
		return new ConfiguredExceptionMethod(method, parameters, method.ReturnType, exception);
	}

	internal static ConfiguredMethod ForValue<TReturn>(LambdaExpression member, TReturn returnValue)
	{
		var (method, parameters) = GetMethodFromExpression(member);
		return new ConfiguredValueMethod(method, parameters, typeof(TReturn), returnValue);
	}

	private static (MethodInfo method, ParameterInfo[] parameters) GetMethodFromExpression(LambdaExpression member)
	{
		if (member.Body is not MethodCallExpression methodExpression)
			throw InvalidCallBackConfigurationException.For<MethodCallExpression>(member);

		var method = methodExpression.Method;
		var parameters = method .GetParameters();

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
	MethodBase Method, ParameterInfo[] Parameters, Type ReturnType, object? ReturnValue
) : ConfiguredMethod(Method, Parameters, ReturnType)
{
	public override object? Invoke(params object?[] parameters) => ReturnValue;
}

internal sealed record ConfiguredExceptionMethod(
	MethodBase Method, ParameterInfo[] Parameters, Type ReturnType, Func<Exception> Exception
) : ConfiguredMethod(Method, Parameters, ReturnType)
{
	// TODO analyzer to make sure they don't throw in the Throws(() => ..) expression
	// TODO analyzer to suggest using Throws instead of Executes(() => throw ...) etc.
	public override object? Invoke(params object?[] parameters)
	{
		Exception exception;

		try
		{
			exception = Exception();
		}
		catch(Exception ex)
		{
			exception = ex;
		}

		throw exception;
	}
}

internal sealed record ConfiguredLambdaMethod(
	MethodBase Method, ParameterInfo[] Parameters, Type ReturnType, Delegate? returnDelegate
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
	MethodBase Method, ParameterInfo[] Parameters, Delegate? callbackDelegate
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