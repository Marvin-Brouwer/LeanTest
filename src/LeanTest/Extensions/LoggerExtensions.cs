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

	public static object CreateGenericLoggerForType(this ILoggerFactory loggerFactory, Type type)
	{
		var createLoggerMethod = _createLoggerMethod;

		return createLoggerMethod
			.MakeGenericMethod(type)
			.Invoke(null, new[] { loggerFactory })!;
	}


	public static ITestSuite InjectLogger(this ITestSuite suite, ILoggerFactory loggerFactory)
	{
		// This has to be done via reflection, since we need a generic typed logger
		var logger = loggerFactory.CreateGenericLoggerForType(suite.ServiceType);

		var testOutputLoggerProperty = GetTestOutputLoggerProperty(suite);
		testOutputLoggerProperty.SetValue(suite, logger);

		return suite;
	}

	private static PropertyInfo GetTestOutputLoggerProperty(ITestSuite suite)
	{
		// .GetRuntimeProperty(nameof(TestSuite<object>.TestOutputLogger)) didn't work
		var properties = suite.GetType().GetRuntimeProperties();

		foreach (var property in properties)
		{
			if (property.Name.Equals(nameof(TestSuite<object>.TestOutputLogger), StringComparison.Ordinal))
				return property;
		}

		throw new NotImplementedException("Unreachable code");
	}
}
