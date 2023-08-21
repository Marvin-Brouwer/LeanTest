using LeanTest.Dependencies.Providers;
using LeanTest.Dynamic.Generating;
using LeanTest.Tests;

using Microsoft.Extensions.Logging;

using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LeanTest.Hosting;

internal class TestFactory
{
	private readonly ILoggerFactory _loggerFactory;

	public TestFactory(ILoggerFactory loggerFactory)
	{
		_loggerFactory = loggerFactory;
	}

	public async IAsyncEnumerable<ITestScenario> InitializeScenarios(
		Assembly assembly, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		TestContext.Current.TestLoggerFactory = _loggerFactory;
		TestContext.Current.TestCancellationToken = new CancellationTokenProvider(cancellationToken);
		TestContext.Current.AssemblyContext = new RuntimeAssemblyContext(assembly);

		if (cancellationToken.IsCancellationRequested) yield break;
		var assemblySenarios = InitializeScenariosForAssembly(assembly, cancellationToken);
		if (cancellationToken.IsCancellationRequested) yield break;

		await foreach (var scenario in assemblySenarios)
		{
			if (cancellationToken.IsCancellationRequested) yield break;
			yield return scenario;
		}
	}

	private async IAsyncEnumerable<ITestScenario> InitializeScenariosForAssembly(
		Assembly assembly, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		var suiteTypes = IndexTestSuites(assembly, cancellationToken);
		var suites = InitializeSuites(suiteTypes, cancellationToken);
		if (cancellationToken.IsCancellationRequested) yield break;

		// Enumerate tests after logger's been inserted
		await foreach (var suite in suites)
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

	private async IAsyncEnumerable<ITestSuite> InitializeSuites(
		IEnumerable<Type> suiteTypes, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		foreach (var suiteType in suiteTypes)
		{
			if (cancellationToken.IsCancellationRequested) yield break;
			yield return await InitializeSuite(suiteType);
		}
	}

	private Task<ITestSuite> InitializeSuite(Type suiteType)
	{
		// Use a task completion source, this ensures it's wrapped in a task properly
		// This means the parent task doesn't block when someone buils a huge testsuite constructor
		var completion = new TaskCompletionSource<ITestSuite>();

		ITestSuite instance = default!;
		try
		{
			instance = (ITestSuite)Activator.CreateInstance(suiteType)!;
			completion.SetResult(instance);
		}
		catch (Exception ex)
		{
			// TODO handle constructor exceptions
		}

		Debug.Assert(instance is not null,
			$"Types passed to {nameof(InitializeSuite)} are known to be {nameof(ITestSuite)}");

		return completion.Task;
	}
}

