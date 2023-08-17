using LeanTest.Dependencies.Providers;
using LeanTest.Tests;

using Microsoft.Extensions.Logging;

using System.Diagnostics;
using System.Reflection;

namespace LeanTest.Hosting;

internal class TestFactory
{
	private readonly ILoggerFactory _loggerFactory;

	public TestFactory(ILoggerFactory loggerFactory)
	{
		_loggerFactory = loggerFactory;
	}

	public IEnumerable<ITestScenario> InitializeScenarios(Assembly assembly, CancellationToken cancellationToken)
	{
		TestContext.Current.TestLoggerFactory = _loggerFactory;
		TestContext.Current.TestCancellationToken = new CancellationTokenProvider(cancellationToken);

		if (cancellationToken.IsCancellationRequested) yield break;
		var assemblySenarios = InitializeScenariosForAssembly(assembly, cancellationToken);
		if (cancellationToken.IsCancellationRequested) yield break;

		foreach (var scenario in assemblySenarios)
		{
			if (cancellationToken.IsCancellationRequested) yield break;
			yield return scenario;
		}
	}

	private IEnumerable<ITestScenario> InitializeScenariosForAssembly(Assembly assembly, CancellationToken cancellationToken)
	{
		var suiteTypes = IndexTestSuites(assembly, cancellationToken);
		var suites = InitializeSuites(suiteTypes, cancellationToken);
		if (cancellationToken.IsCancellationRequested) yield break;

		// Enumerate tests after logger's been inserted
		foreach (var suite in suites)
		{
			if (cancellationToken.IsCancellationRequested) yield break;
			foreach (var test in suite.Tests)
			{
				if (cancellationToken.IsCancellationRequested) yield break;
				yield return test;
			}
		}
	}

	internal static IEnumerable<Type> IndexTestSuites(Assembly assembly, CancellationToken cancellationToken)
	{
		foreach (var assemblyScannedType in assembly.GetExportedTypes())
		{
			if (cancellationToken.IsCancellationRequested) yield break;
			if (!assemblyScannedType.IsAssignableTo(typeof(ITestSuite))) continue;

			yield return assemblyScannedType;
		}
	}

	private IEnumerable<ITestSuite> InitializeSuites(IEnumerable<Type> suiteTypes, CancellationToken cancellationToken)
	{
		foreach (var suiteType in suiteTypes)
		{
			if (cancellationToken.IsCancellationRequested) yield break;
			yield return InitializeSuite(suiteType);
		}
	}

	private ITestSuite InitializeSuite(Type suiteType)
	{
		ITestSuite instance = default!;
		try
		{
			instance = (ITestSuite)Activator.CreateInstance(suiteType)!;
		}
		catch (Exception ex)
		{
			// TODO handle constructor exceptions
		}

		Debug.Assert(instance is not null,
			$"Types passed to {nameof(InitializeSuite)} are known to be {nameof(ITestSuite)}");

		return instance;
	}
}

