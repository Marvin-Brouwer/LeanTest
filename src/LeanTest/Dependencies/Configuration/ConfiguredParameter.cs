using FluentSerializer.Core.Extensions;

using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace LeanTest.Dependencies.Configuration;

/// <summary>
/// Create an extended <see cref="ConfiguredParameter"/>, meant for use in extension methods.
/// </summary>
[DebuggerDisplay("Parameter matches user defined condition")]
public abstract class ConfiguredParameterExtension : ConfiguredParameter {
	protected ConfiguredParameterExtension(ParameterInfo parameterInfo) 
	{
		Parameter = parameterInfo;
	}
}

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

	public static ConfiguredParameter ForAnyType(ParameterInfo parameter) => new UnconstrainedParameter
	{
		Parameter = parameter
	};

	public static ConfiguredParameter ForNullValues(ParameterInfo parameter) => new NullConstrainedParameter
	{
		Parameter = parameter
	};

	public static ConfiguredParameter ForNonNullValues(ParameterInfo parameter) => new NonNullConstrainedParameter
	{
		Parameter = parameter
	};

	public static ConfiguredParameter ForMatch(ParameterInfo parameter, LambdaExpression match)
	{
		var matchDelegate = (object? parameterValue) => {
			try
			{
				return (bool)match.Compile().DynamicInvoke(parameterValue)!;
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

	public static ConfiguredParameter ForExtension<T>(ParameterInfo parameter, Func<T, bool> match)
	{
		var matchDelegate = (object? parameterValue) => {
			try
			{
				return (bool)match.DynamicInvoke(parameterValue)!;
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

	/// <summary>
	/// Don't use this class for extension, <see cref="ConfiguredParameterExtension"/>.
	/// </summary>
	internal ConfiguredParameter() { }

	public required ParameterInfo Parameter { get; init; }
	public abstract bool MatchesRequirements<T>(T? parameterValue) ;
}

[DebuggerDisplay("When parameter equals constant Value")]
internal sealed class ValueConstrainedParameter : ConfiguredParameter
{
	public required object? Value { get; init; }

	public override bool MatchesRequirements<T>(T? parameterValue) where T : default
	{
		if (parameterValue is null) return Value is null;
		return parameterValue.Equals(Value);
	}
}

[DebuggerDisplay("When parameter matches anything")]
internal sealed class UnconstrainedParameter : ConfiguredParameter
{
	public override bool MatchesRequirements<T>(T? parameterValue) where T : default => true;
}

[DebuggerDisplay("When parameter is null")]
internal sealed class NullConstrainedParameter : ConfiguredParameter
{
	public override bool MatchesRequirements<T>(T? parameterValue) where T : default => parameterValue is null;
}

[DebuggerDisplay("When parameter is not null")]
internal sealed class NonNullConstrainedParameter : ConfiguredParameter
{
	public override bool MatchesRequirements<T>(T? parameterValue) where T : default => parameterValue is not null;
}

[DebuggerDisplay("When parameter type matches {ParameterType.FullName}")]
internal sealed class TypeConstrainedParameter : ConfiguredParameter
{
	public required Type ParameterType { get; init; }
	public required bool IsNullable { get; init; }

	public override bool MatchesRequirements<T>(T? parameterValue) where T : default
	{
		if (parameterValue is null) return Parameter.IsOut || IsNullable;
		if (ParameterType == typeof(DynamicObject)) return true;

		if (ParameterType.IsByRef)
			return parameterValue.GetType().IsAssignableTo(Parameter.ParameterType.GetElementType());
		return parameterValue.GetType().IsAssignableTo(Parameter.ParameterType);
	}
}

[DebuggerDisplay("When parameter satisfies delegate")]
internal sealed class MatchConstrainedParameter : ConfiguredParameter
{
	public required Func<object?, bool> MatchDelegate { get; init; }

	public override bool MatchesRequirements<T>(T? parameterValue)
		where T : default => MatchDelegate(parameterValue);
}