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
	// TODO this should come from settings
	private readonly bool _shouldAttach = true;

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
		if (runContext?.IsBeingDebugged == true && _shouldAttach && !Debugger.IsAttached)
			Debugger.Launch();

		if (frameworkHandle is null) return;
		if (runContext is null) return;
		if (tests is null) return;

		// Credits to fixie: https://github.com/fixie/fixie/blob/57631d69b938a8efd88c5646ea60124cb72bdbb1/src/Fixie.TestAdapter/VsTestExecutor.cs#L209C1-L214C6
		HandlePoorVsTestImplementationDetails(runContext, frameworkHandle);

		var logger = frameworkHandle.Wrap();

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

		// TODO REplace functionality
		//var testExecutor = new LeanTestExecutor(logger, frameworkHandle, _cancellationToken);
		//testExecutor.ExecuteTests(tests);
	}

	private Task InvokeTestConsole(IReadOnlyList<TestCase> tests, IFrameworkHandle frameworkHandle, ILogger logger)
	{
		// TODO maybe this can be found with the runContext or something?
		var module = tests
			.Select(test => test.GetPropertyValue<string>(TestProperties.SuiteTypeName, null))
			.Where(typeName => typeName is not null)
			.Select(typeName => Type.GetType(typeName!)!)
			.Select(suiteType => suiteType.Assembly)
			.First();

		TestAdapterContext.HostCancelationToken = _cancellationToken;;
		TestAdapterContext.HostExecutionRecorder = frameworkHandle;
		TestAdapterContext.HostLogger = logger;
		TestAdapterContext.CurrentFilteredTestCases = tests;

		var entryPoint = module.EntryPoint;
		// TODO source analyzer to make sure a program.cs is present
		if (entryPoint is null) throw new EntryPointNotFoundException(
			$"The test assembly {module.FullName ?? "?"} does not have an entrypoint.{Environment.NewLine}" +
			$"Please make sure to define it as a console application, and a Program.cs with a TestHostBuilder is defined."
		);

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

