namespace LeanTest.Hosting.TestAdapter.VsTest;

using LeanTest.Hosting.TestAdapter.Constants;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

using System.Diagnostics;
using System.Reflection;

public abstract class VsTestExecutor : ITestExecutor
{
	public const string Id = "executor://leantest.testadapter/";
	public static readonly Uri Uri = new(Id);

	private readonly CancellationTokenSource _cancellationSource;
	private readonly CancellationToken _cancellationToken;

	public VsTestExecutor() : this(CancellationToken.None) { }
	public VsTestExecutor(CancellationToken cancellationToken)
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

		// We can't use await here because the contract from VSTest says we have to wait for everything to finish
		// before returning from this function.
		// XUnit does the same thing: https://github.com/xunit/visualstudio.xunit/blob/0ea6e6cb0991237daf727dfb69e64ced205f4553/src/xunit.runner.visualstudio/VsTestRunner.cs#L157C1-L159C43
		InvokeTestConsole(tests.ToArray(), frameworkHandle, logger).GetAwaiter().GetResult();
	}

	private Task InvokeTestConsole(IReadOnlyList<TestCase> tests, IFrameworkHandle frameworkHandle, ILogger logger)
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

		var entryPoint = module.EntryPoint;
		// TODO source analyzer to make sure a program.cs is present
		if (entryPoint is null) throw new EntryPointNotFoundException(
			$"The test assembly {module.FullName ?? "?"} does not have an entrypoint.{Environment.NewLine}" +
			$"Please make sure to define it as a console application, and a Program.cs with a TestHostBuilder is defined."
		);

		logger.LogDebug("Invoking test program");

		// TODO: We put an AsyncEntryPoint thing here, if we don't need async functionality here we might as well just call the regular entrypoint.
		// Since we call .GetAwaiter().GetResult() here anyway.
		var asyncEntryPoint = entryPoint.DeclaringType!.GetMethod(entryPoint.Name + "$", BindingFlags.Static | BindingFlags.NonPublic);
		if (asyncEntryPoint is null) return SyncFallback(entryPoint);
		else return (Task)asyncEntryPoint.Invoke(asyncEntryPoint, [Array.Empty<string>()])!;
	}

	private static Task SyncFallback(MethodInfo entryPoint)
	{
		try
		{
			entryPoint.Invoke(entryPoint, [Array.Empty<string>()]);
			return Task.CompletedTask;
		}
		catch (Exception ex)
		{
			return Task.FromException(ex);
		}
	}

	static void HandlePoorVsTestImplementationDetails(IRunContext runContext, IFrameworkHandle frameworkHandle)
	{
		if (runContext.KeepAlive) frameworkHandle.EnableShutdownAfterTestRun = true;
	}
}

