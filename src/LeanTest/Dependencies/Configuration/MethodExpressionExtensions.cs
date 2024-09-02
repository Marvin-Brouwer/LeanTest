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
				var customMatchAttribute = argumentConfiguration.Method
					.GetCustomAttributes()
					.FirstOrDefault(a => a.GetType().IsAssignableTo(typeof(IParameterMatchAttribute)));

				if (customMatchAttribute is IParameterMatchAttribute customMatch)
				{
					configuredParameters[i] = customMatch.GetParameter(originalParameter);
					continue;
				}
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
				nameof(IParameterFactory.IsAny) => ConfiguredParameter.ForAnyType(
					originalParameter
				),
				nameof(IParameterFactory.IsNull) => ConfiguredParameter.ForNullValues(
					originalParameter
				),
				nameof(IParameterFactory.NotNull) => ConfiguredParameter.ForNonNullValues(
					originalParameter
				),
				nameof(IParameterFactory.Matches) => ConfiguredParameter.ForMatch(
					originalParameter,
					(LambdaExpression)((UnaryExpression)argumentConfiguration.Arguments.First()).Operand
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
