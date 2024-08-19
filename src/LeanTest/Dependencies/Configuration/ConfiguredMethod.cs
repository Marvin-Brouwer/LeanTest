using LeanTest.Dependencies.Factories;

using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

using Parameters = LeanTest.Dependencies.Configuration.ConfiguredParametersCollection;

namespace LeanTest.Dependencies.Configuration;

internal abstract record ConfiguredMethod(MethodBase Method, Parameters Parameters, Type ReturnType)
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

	private static (MethodInfo method, Parameters parameters) GetMethodFromExpression(LambdaExpression member)
	{
		if (member.Body is not MethodCallExpression methodExpression)
			throw InvalidCallBackConfigurationException.For<MethodCallExpression>(member);

		var method = methodExpression.Method;
		var parameters = method.GetParameters() ?? Array.Empty<ParameterInfo>();
		var configuredParameters = new ConfiguredParameter[parameters.Length];
		// Keep track of the "parameterSpecificity", this basically just means that a higher number get's filtered first
		var parameterSpecificity = 0;

		for (int  i = 0; i < parameters.Length; i++)
		{
			var originalParameter = parameters[i];
			var argument = methodExpression.Arguments[i];
			// TODO handle nulls correctly
			if (argument.NodeType == ExpressionType.Constant) {
				parameterSpecificity = 1;
				configuredParameters[i] = ConfiguredParameter.ForConstant(originalParameter, (ConstantExpression)argument);
				continue;
			}
			if (argument.NodeType != ExpressionType.Call)
			{
				configuredParameters[i] = ConfiguredParameter.ForParameter(originalParameter);
				continue;
			}

			if (argument is not MethodCallExpression argumentConfiguration)
			{
				configuredParameters[i] = ConfiguredParameter.ForParameter(originalParameter);
				continue;
			}
			if (argumentConfiguration.Method.DeclaringType != typeof(IParameterFactory))
			{
				configuredParameters[i] = ConfiguredParameter.ForParameter(originalParameter);
				continue;
			}

			parameterSpecificity = argumentConfiguration.Method.Name == nameof(IParameterFactory.Matches)
				? 3
				: 2;

			configuredParameters[i] = argumentConfiguration.Method.Name switch
			{
				nameof(IParameterFactory.Is) => ConfiguredParameter.ForType(
					originalParameter,
					argumentConfiguration.Method.GetGenericArguments().First()
				),
				nameof(IParameterFactory.Matches) => ConfiguredParameter.ForMatch(
					originalParameter,
					(UnaryExpression)argumentConfiguration.Arguments.First()
				),
				nameof(IParameterFactory.IsReference) => ConfiguredParameter.ForType(
					originalParameter,
					argumentConfiguration.Method.GetGenericArguments().First()
				),
				_ => throw new UnreachableException()
			};
		}

		return (method, new Parameters(configuredParameters, parameterSpecificity));
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

internal sealed record ConfiguredExceptionMethod(
	MethodBase Method, Parameters Parameters, Type ReturnType, Func<Exception> Exception
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
	MethodBase Method, Parameters Parameters, Type ReturnType, Delegate? returnDelegate
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
	MethodBase Method, Parameters Parameters, Delegate? callbackDelegate
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