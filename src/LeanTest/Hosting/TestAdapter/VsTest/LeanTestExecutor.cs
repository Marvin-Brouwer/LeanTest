namespace LeanTest.Hosting.TestAdapter.VsTest;

using LeanTest.Hosting.TestAdapter.Constants;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

using System.Diagnostics;
using System.Reflection;

public abstract class LeanTestExecutor : ITestExecutor
{
	public const string Id = "executor://leantest.testadapter/";
	public static readonly Uri Uri = new(Id);

	private readonly CancellationTokenSource _cancellationSource;
	private readonly CancellationToken _cancellationToken;

	public LeanTestExecutor() : this(CancellationToken.None) { }
	public LeanTestExecutor(CancellationToken cancellationToken)
	{
		_cancellationSource = new CancellationTokenSource();
		_cancellationToken = CancellationTokenSource
			.CreateLinkedTokenSource(_cancellationSource.Token, cancellationToken)
			.Token;
	}

	public void Cancel() => _cancellationSource.Cancel();

	public void RunTests(IEnumerable<string>? sources, IRunContext? runContext, IFrameworkHandle? frameworkHandle) => throw new NotSupportedException();

	public void RunTests(IEnumerable<TestCase>? tests, IRunContext? runContext, IFrameworkHandle? frameworkHandle)
	{
		if (runContext?.IsBeingDebugged == true && !Debugger.IsAttached) Debugger.Launch();

		if (frameworkHandle is null) return;
		if (runContext is null) return;
		if (tests is null) return;

		// Credits to fixie: https://github.com/fixie/fixie/blob/57631d69b938a8efd88c5646ea60124cb72bdbb1/src/Fixie.TestAdapter/VsTestExecutor.cs#L209C1-L214C6
		HandlePoorVsTestImplementationDetails(runContext, frameworkHandle);

#if DEBUG
		var logger = frameworkHandle.Wrap(LogLevel.Debug);
#else
		var logger = frameworkHandle.Wrap(LogLevel.Information);
#endif

		if (_cancellationToken.IsCancellationRequested)
		{
			const string preRunCancellationMessage = "Cancellation requested before running tests";
			logger.LogWarning(preRunCancellationMessage);

			foreach (var testCase in tests)
				frameworkHandle.RecordResult(new TestResult(testCase)
				{
					Outcome = TestOutcome.None,
					ErrorMessage = preRunCancellationMessage
				}); ;
			return;
		}

		InvokeTestConsole(tests.ToArray(), frameworkHandle, logger);
	}

	private void InvokeTestConsole(IReadOnlyList<TestCase> tests, IFrameworkHandle frameworkHandle, ILogger logger)
	{
		var module = tests
			.Select(test => test.GetPropertyValue<string>(TestProperties.SuiteTypeName, null))
			.Where(typeName => typeName is not null)
			.Select(typeName => Type.GetType(typeName!)!)
			.Select(suiteType => suiteType.Assembly)
			// Apparently dynamic loaded assemblies aren't idempotent
			.DistinctBy(assembly => assembly.FullName ?? assembly.Location)
			// We expect this to be run per test project, so only one AssemblyModule should be present
			.Single();

		logger.LogInformation("Running tests on assembly: {AssemblyName}", module.FullName);
		logger.LogDebug("Configuring TestAdapterContext");

		// Set these globally scoped so the EntryPoint can use these.
		TestAdapterContext.HostCancelationToken = _cancellationToken;;
		TestAdapterContext.HostExecutionRecorder = frameworkHandle;
		TestAdapterContext.HostMessageLogger = frameworkHandle;
		TestAdapterContext.CurrentFilteredTestCases = tests;

		// https://github.com/Marvin-Brouwer/LeanTest/issues/9
		var entryPoint = module.EntryPoint;
		if (entryPoint is null) throw new EntryPointNotFoundException(
			$"The test assembly {module.FullName ?? "?"} does not have an entrypoint.{Environment.NewLine}" +
			$"Please make sure to define it as a console application, and a Program.cs with a TestHostBuilder is defined."
		);

		logger.LogDebug("Invoking test program");

		// Even though the app may have async top level statements, it's compiled into
		// `.GetAwaiter().GetResult()` anyways.
		// This is not an issue since we should only ever have one test program running.
		_ = entryPoint.Invoke(entryPoint, [Array.Empty<string>()])!;
	}

	static void HandlePoorVsTestImplementationDetails(IRunContext runContext, IFrameworkHandle frameworkHandle)
	{
		if (runContext.KeepAlive) frameworkHandle.EnableShutdownAfterTestRun = true;
	}
}

