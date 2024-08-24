using System.Linq.Expressions;
using System.Reflection;

using Parameters = LeanTest.Dependencies.Configuration.ConfiguredParametersCollection;

namespace LeanTest.Dependencies.Configuration;

internal abstract record ConfiguredMethod(MethodBase Method, Parameters Parameters, Type ReturnType)
{
	// [Performance] If for we get performance remarks, we could split this out int he same T1..T5 setup all the way down
	// For now that adds unneccesary complexity.
	internal static ConfiguredMethod ForCallback(LambdaExpression member, Delegate? callbackDelegate = null)
	{
		var (method, parameters) = member.GetMethodFromExpression();
		return new ConfiguredVoidMethod(method, parameters, callbackDelegate);
	}
	internal static ConfiguredMethod ForCallback<TReturn>(LambdaExpression member, Delegate returnDelegate)
	{
		var (method, parameters) = member.GetMethodFromExpression();
		return new ConfiguredLambdaMethod(method, parameters, typeof(TReturn), returnDelegate);
	}

	internal static ConfiguredMethod ForException<TException>(LambdaExpression member, Func<TException> exception)
		where TException : Exception
	{
		var (method, parameters) = member.GetMethodFromExpression();
		return new ConfiguredExceptionMethod<TException>(method, parameters, method.ReturnType, exception);
	}

	internal static ConfiguredMethod ForValue<TReturn>(LambdaExpression member, TReturn returnValue)
	{
		var (method, parameters) = member.GetMethodFromExpression();
		return new ConfiguredValueMethod(method, parameters, typeof(TReturn), returnValue);
	}

	/// <summary>
	/// Wrap delegates in an <see cref="Invoke(object?[])"/> method so the amount of parameters no longer
	/// throws an exception when not matching <br />
	/// This is only really useful for the <see cref="ConfiguredValueMethod"/> <br/>
	/// However, this allows the parameter check to be put in here in stead of the Marshall
	/// </summary>
	public abstract object? Invoke(params object?[] parameters);

	/// <summary>
	/// Checks whether the basic "shape" of the method matches. <br />
	/// E.g. name, number of parameters etc. 
	/// </summary>
	/// <remarks>
	/// The checking of the parameters by the value passed is done seperately. <br />
	/// This is done by <see cref="ConfiguredParameter.MatchesRequirements{T}(T?)"/>
	/// </remarks>
	public bool MethodShapeMatches(MethodBase methodInfo, object?[] parameters, Type returnType)
	{
		if (!Method.Name.Equals(methodInfo.Name, StringComparison.Ordinal)) return false;
		if (!ReturnType.Equals(returnType)) return false;
		if (Method.IsGenericMethod != methodInfo.IsGenericMethod) return false;
		if (!Parameters.Length.Equals(parameters.Length)) return false;
		// TODO should we check by name for overloads or is the current filtering logic good enough?

		return true;
	}
}

internal sealed record ConfiguredValueMethod(
	MethodBase Method, Parameters Parameters, Type ReturnType, object? ReturnValue
) : ConfiguredMethod(Method, Parameters, ReturnType)
{
	public override object? Invoke(params object?[] parameters) => ReturnValue;
}

internal sealed record ConfiguredExceptionMethod<TException>(
	MethodBase Method, Parameters Parameters, Type ReturnType, Func<TException> Exception
) : ConfiguredMethod(Method, Parameters, ReturnType)
	where TException : Exception
{
	// TODO-analyzer to make sure they don't throw in the Throws(() => ..) expression
	// TODO-analyzer to suggest using Throws instead of Executes(() => throw ...) etc.
	public override object? Invoke(params object?[] parameters)
	{
		throw Exception();
	}
}

internal sealed record ConfiguredLambdaMethod(
	MethodBase Method, Parameters Parameters, Type ReturnType, Delegate? ReturnDelegate
) : ConfiguredMethod(Method, Parameters, ReturnType)
{
	public override object? Invoke(params object?[] parameters)
	{
		if (ReturnDelegate is null) return null;

		return parameters.Length == 0
			? ReturnDelegate.DynamicInvoke(null)!
			: ReturnDelegate.DynamicInvoke(parameters)!;
	}
}
internal sealed record ConfiguredVoidMethod(
	MethodBase Method, Parameters Parameters, Delegate? CallbackDelegate
) : ConfiguredMethod(Method, Parameters, typeof(void))
{
	public override object? Invoke(params object?[] parameters)
	{
		if (CallbackDelegate is null) return null;

		return parameters.Length == 0
			? CallbackDelegate.DynamicInvoke(null)!
			: CallbackDelegate.DynamicInvoke(parameters)!;
	}
}