using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace LeanTest.Dependencies.Configuration;

public sealed class ConfiguredMethodSet
{
	private readonly ISet<ConfiguredMethod> _configuredMethods = new HashSet<ConfiguredMethod>();

	internal void Add(ConfiguredMethod configuredMethod)
	{
		_configuredMethods.Add(configuredMethod);
	}

	internal bool TryFind(MethodBase methodInfo, object?[] parameters, [NotNullWhen(true)] out ConfiguredMethod? method)
	{
		return TryFind(methodInfo, parameters, typeof(void), out method);
	}

	internal bool TryFind<TReturn>(MethodBase methodInfo, object?[] parameters, [NotNullWhen(true)] out ConfiguredMethod? method)
	{
		return TryFind(methodInfo, parameters, typeof(TReturn), out method);
	}

	internal bool TryFind(MethodBase methodInfo, object?[] parameters, Type returnType, [NotNullWhen(true)] out ConfiguredMethod? configuredMethod)
	{
		configuredMethod = _configuredMethods
			.Where(m => m.MethodShapeMatches(methodInfo, parameters, returnType))
			.OrderByDescending(m => m.Parameters.Specificity)
			.FirstOrDefault(m => m.Parameters.ParametersMatch(parameters));

		return configuredMethod is not null;
	}
}
