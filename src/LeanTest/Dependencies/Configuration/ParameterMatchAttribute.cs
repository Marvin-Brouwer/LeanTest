using System.Reflection;

namespace LeanTest.Dependencies.Configuration;

/// <summary>
/// Mark the extension of <see cref="IParameterFactory"/> to use <typeparamref name="TParameter"/> as its filter logic.
/// </summary>
/// <example>
/// <![CDATA[
/// public static class ExampleParameterExtensions
/// {
/// 	[ParameterMatch<TrueValueMatcher>]
/// 	public static bool IsTrue(this IParameterExpressionProvider parameter)
/// 	{
/// 		_ = parameterFactory;
/// 		return true;
/// 	}
/// 	
/// 	[ParameterMatch<FalseValueMatcher>]
/// 	public static bool IsFalse(this IParameterExpressionProvider parameter)
/// 	{
/// 		_ = parameterFactory;
/// 		return false;
/// 	}
/// 
/// 	[DebuggerDisplay("When parameter is true")]
/// 	internal class TrueValueMatcher(ParameterInfo parameterInfo) : ConfiguredParameterExtension(parameterInfo)
/// 	{
/// 		public override bool MatchesRequirements<T>(T? parameterValue) where T : default
/// 		{
/// 			return true.Equals(parameterValue);
/// 		}
/// 	}
/// 
/// 	[DebuggerDisplay("When parameter is true")]
/// 	internal class FalseValueMatcher(ParameterInfo parameterInfo) : ConfiguredParameterExtension(parameterInfo)
/// 	{
/// 		public override bool MatchesRequirements<T>(T? parameterValue) where T : default
/// 		{
/// 			return false.Equals(parameterValue);
/// 		}
/// 	}
/// }
/// ]]>
/// </example>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class ParameterMatchAttribute<TParameter> : Attribute, IParameterMatchAttribute
	where TParameter : ConfiguredParameterExtension
{
	public ConfiguredParameter GetParameter(ParameterInfo parameter)
	{
		return (ConfiguredParameter)Activator.CreateInstance(typeof(TParameter), parameter)!;
	}
}

internal interface IParameterMatchAttribute
{
	abstract ConfiguredParameter GetParameter(ParameterInfo parameter);

}
