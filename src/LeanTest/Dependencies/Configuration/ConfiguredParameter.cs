using FluentSerializer.Core.Extensions;

using System.Linq.Expressions;
using System.Reflection;

namespace LeanTest.Dependencies.Configuration;

public abstract class ConfiguredParameter
{
	public static ConfiguredParameter ForConstant(ParameterInfo parameter, ConstantExpression valueExpression) => new ValueConstrainedParameter
	{
		Parameter = parameter,
		Value = valueExpression.Value,
	};
	public static ConfiguredParameter ForParameter(ParameterInfo parameter) => new TypeConstrainedParameter
	{
		Parameter = parameter,
		IsNullable = parameter.IsNullable(),
		ParameterType = parameter.ParameterType,
	};

	public static ConfiguredParameter ForType(ParameterInfo parameter, Type parameterType) => new TypeConstrainedParameter
	{
		Parameter = parameter,
		IsNullable = parameter.IsNullable(),
		ParameterType = parameterType,
	};

	public static ConfiguredParameter ForMatch(ParameterInfo parameter, UnaryExpression expression)
	{
		var match = (LambdaExpression)expression.Operand;
		var matchDelegate = (object? obj) => {
			try
			{
				return (bool)match.Compile().DynamicInvoke(obj)!;
			}
			catch
			{
				// https://github.com/Marvin-Brouwer/LeanTest/issues/7
				return false;
			}
		};

		return new MatchConstrainedParameter
		{
			Parameter = parameter,
			MatchDelegate = matchDelegate
		};
	}

	public required ParameterInfo Parameter { get; init; }
	public abstract bool MatchesRequirements<T>(T? obj) ;
}

internal sealed class ValueConstrainedParameter : ConfiguredParameter
{
	public required object? Value { get; init; }

	public override bool MatchesRequirements<T>(T? obj) where T : default
	{
		if (obj is null) return Value is null;
		return obj.Equals(Value);
	}
}

internal sealed class TypeConstrainedParameter : ConfiguredParameter
{
	public required Type ParameterType { get; init; }
	public required bool IsNullable { get; init; }

	public override bool MatchesRequirements<T>(T? obj) where T : default
	{
		if (obj is null) return IsNullable;
		return obj.GetType() == Parameter.ParameterType;
	}
}
internal sealed class MatchConstrainedParameter : ConfiguredParameter
{
	public required Func<object?, bool> MatchDelegate { get; init; }

	public override bool MatchesRequirements<T>(T? obj)
		where T : default => MatchDelegate(obj);
}