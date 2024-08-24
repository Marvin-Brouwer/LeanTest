using LeanTest.Dependencies.Factories;

using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

using Parameters = LeanTest.Dependencies.Configuration.ConfiguredParametersCollection;

namespace LeanTest.Dependencies.Configuration;

internal static class MethodExpressionExtensions
{
	public static (MethodInfo method, Parameters parameters) GetMethodFromExpression(this LambdaExpression member)
	{
		if (member.Body is not MethodCallExpression methodExpression)
			throw InvalidCallBackConfigurationException.For<MethodCallExpression>(member);

		var method = methodExpression.Method;
		var parameters = method.GetParameters() ?? [];

		var configuredParameters = MapParameters(parameters, methodExpression.Arguments);

		return (method, configuredParameters);
	}

	private static Parameters MapParameters(IReadOnlyList<ParameterInfo> parameters, IReadOnlyList<Expression> arguments)
	{
		var configuredParameters = new ConfiguredParameter[parameters.Count];
		// Keep track of the "parameterSpecificity", this basically just means that a higher number get's filtered first
		var parameterSpecificity = 0;

		for (int i = 0; i < parameters.Count; i++)
		{
			var originalParameter = parameters[i];
			var argument = arguments[i];
			// TODO handle nulls correctly
			if (argument.NodeType == ExpressionType.Constant)
			{
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

		return new Parameters(configuredParameters, parameterSpecificity);
	}
}
