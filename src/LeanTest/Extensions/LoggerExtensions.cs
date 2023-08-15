using LeanTest.Dependencies.Providers;
using LeanTest.Exceptions;

using Microsoft.Extensions.Logging;

using System.Reflection;

namespace LeanTest.Extensions;

internal static class LoggerExtensions
{
	private static readonly MethodInfo _createLoggerMethod = GetCreateLoggerMethod();

	private static MethodInfo GetCreateLoggerMethod()
	{
		var methods = typeof(LoggerFactoryExtensions)
			.GetMethods(BindingFlags.Public | BindingFlags.Static)!;

		foreach (var method in methods)
		{
			if (!method.Name
				.Equals(nameof(LoggerFactoryExtensions.CreateLogger),
				StringComparison.Ordinal)
			) continue;

			if (method.IsGenericMethod) return method;
		}

		throw new UnreachableCodeException
		{
			Why = "The LoggerFactoryExtensions.CreateLogger method is known to exist"
		};
	}

	// TODO, do we still need this?
	public static object CreateGenericLoggerForType(this ILoggerFactory loggerFactory, Type type)
	{
		var createLoggerMethod = _createLoggerMethod;

		return createLoggerMethod
			.MakeGenericMethod(type)
			.Invoke(null, new[] { loggerFactory })!;
	}
}
