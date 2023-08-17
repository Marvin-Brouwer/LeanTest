using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace LeanTest.Dynamic.Invocation;

public sealed class ConfiguredMethodSet
{
	private readonly ISet<ConfiguredMethod> _configuredMethods = new HashSet<ConfiguredMethod>(); // (new Comparer());

	internal void Add(ConfiguredMethod configuredMethod)
	{
		_configuredMethods.Add(configuredMethod);
	}

	internal bool TryFind(MethodBase methodInfo, object?[] parameters, [NotNullWhen(true)] out Action? returnAction)
	{
		if (!TryFind(methodInfo, parameters, typeof(void), out var returnDelegate))
		{
			returnAction = null;
			return false;
		}

		returnAction =  parameters.Length == 0
			? () => returnDelegate.DynamicInvoke(null)
			: () => returnDelegate.DynamicInvoke(parameters);

		return true;
	}

	internal bool TryFind<TReturn>(MethodBase methodInfo, object?[] parameters, [NotNullWhen(true)] out Func<TReturn>? returnFunction)
	{
		if (!TryFind(methodInfo, parameters, typeof(TReturn), out var returnDelegate))
		{
			returnFunction = null;
			return false;
		}

		returnFunction = parameters.Length == 0
			? () => (TReturn)returnDelegate.DynamicInvoke(null)!
			: () => (TReturn)returnDelegate.DynamicInvoke(parameters)!;
		return true;
	}

	internal bool TryFind(MethodBase methodInfo, object?[] parameters, Type returnType, [NotNullWhen(true)] out Delegate? returnDelegate)
	{
		var configuredMethod = _configuredMethods
			.FirstOrDefault(configuredMethod => MethodBodyMatches(methodInfo, parameters, returnType, configuredMethod)
		);

		if (configuredMethod == default)
		{
			returnDelegate = null;
			return false;
		}

		returnDelegate = configuredMethod.ReturnDelegate!;
		return true;
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
