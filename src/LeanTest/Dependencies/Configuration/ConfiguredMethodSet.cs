using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace LeanTest.Dependencies.Configuration;

public sealed class ConfiguredMethodSet
{
	private readonly ISet<ConfiguredMethod> _configuredMethods = new HashSet<ConfiguredMethod>(Comparer.Instance);

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
		// TODO verify methodInfo parameters and parameters are of equal length
		// TODO verify returntype and methodInfo type are the same type
		// TODO verify all methodInfo types corrispond to parameter types

		var methodToFind = MethodDefinition.FromMethodInfo(methodInfo);
		var methodQuery = Comparer.Find(methodToFind);

		configuredMethod = _configuredMethods.FirstOrDefault(methodQuery);
		return configuredMethod is not null;
	}

	private class Comparer : IEqualityComparer<MethodDefinition>
	{
		internal static readonly Comparer Instance = new ();
		internal static Func<ConfiguredMethod, bool> Find(MethodDefinition methodToFind) => (ConfiguredMethod configuredMethod) =>
		{
			if (Instance.GetHashCode().Equals(methodToFind.GetHashCode())) return true;

			return Instance.Equals(configuredMethod, methodToFind);
		};

		public bool Equals(MethodDefinition? x, MethodDefinition? y)
		{
			if (ReferenceEquals(x, y)) return true;
			if (x is null) return y is null;
			if (y is null) return x is null;

			if (!x.Method.Name.Equals(y.Method.Name, StringComparison.Ordinal)) return false;
			if (!x.ReturnType.Equals(y.ReturnType)) return false;

			// TODO compare generics (don't forget to update the hash)
			if (x.Method.IsGenericMethod != y.Method.IsGenericMethod) return false;

			return ParametersMatch(x.Parameters, y.Parameters);
		}

		private static bool ParametersMatch(ParameterInfo[] xParameters, ParameterInfo[] yParameters)
		{
			if (!xParameters.Length.Equals(yParameters.Length)) return false;

			for(var i = 0; i < xParameters.Length; i++)
			{
				// TODO: See if there are constraints that allow for this to not work
				// Like for example a generic overload with the same parameter names and types.
				// (don't forget to update the hash)
				if (!xParameters[i].Name!.Equals(yParameters[i].Name, StringComparison.Ordinal)) return false;
				// TODO how to handle generics? (perhaps wrap ParameterInfo so we can use the actual passed parameters
				// Keep in mind the IParameterFactory.Is() etc.
				if (xParameters[i].ParameterType.IsGenericParameter) continue;
				if (yParameters[i].ParameterType.IsGenericParameter) continue;
				if (!xParameters[i].ParameterType!.Equals(yParameters[i].ParameterType)) return false;
			}

			return true;
		}

		public int GetHashCode([DisallowNull] MethodDefinition obj)
		{
			var hash = new HashCode();

			hash.Add(obj.Method.Name.GetHashCode());
			hash.Add(obj.ReturnType.GetHashCode());

			foreach (var param in obj.Parameters)
			{
				hash.Add(param.Name!.GetHashCode());
				hash.Add(param.ParameterType!.GetHashCode());
			}

			return hash.ToHashCode();
		}
	}
}
