using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace LeanTest.Dependencies.Configuration;

public sealed class ConfiguredMethodSet
{
	private readonly ISet<ConfiguredMethod> _configuredMethods = new HashSet<ConfiguredMethod>(); // (new Comparer());

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
			.FirstOrDefault(configuredMethod => MethodBodyMatches(methodInfo, parameters, returnType, configuredMethod)
		);

		return configuredMethod is not null;
	}

	private static bool MethodBodyMatches(MethodBase methodInfo, object?[] parameters, Type returnType, ConfiguredMethod configuredMethod)
	{
		if (!configuredMethod.Method.Name.Equals(methodInfo.Name, StringComparison.Ordinal)) return false;
		if (!ParametersMatch(parameters, configuredMethod)) return false;

		return configuredMethod.ReturnType == returnType;
	}

	private static bool ParametersMatch(object?[] parameters, ConfiguredMethod configuredMethod)
	{
		// TODO better match
		return configuredMethod.Parameters.Length == parameters.Length;
	}

	private class Comparer : IEqualityComparer<ConfiguredMethod>
	{
		public bool Equals(ConfiguredMethod x, ConfiguredMethod y)
		{
			// TODO compare
			throw new NotImplementedException();
		}

		public int GetHashCode([DisallowNull] ConfiguredMethod obj)
		{
			// TODO custom?, does that even make sense
			return obj.GetHashCode();
		}
	}
}
