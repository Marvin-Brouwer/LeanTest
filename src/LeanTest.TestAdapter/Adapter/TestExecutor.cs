using LeanTest.TestAdapter.Execution;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

using System.Diagnostics;

namespace LeanTest.TestAdapter.Adapter;

[ExtensionUri(Id)]
public sealed class TestExecutor : ITestExecutor
{
	public const string Id = "executor://leantest.testadapter/";
	public static readonly Uri Uri = new(Id);

	private readonly CancellationTokenSource _cancellationSource;
	private readonly CancellationToken _cancellationToken;
	// TODO this should come from settings
	private readonly bool _shouldAttach = true;

	public TestExecutor(CancellationToken cancellationToken)
	{
		_cancellationSource = new CancellationTokenSource();
		_cancellationToken = CancellationTokenSource
			.CreateLinkedTokenSource(_cancellationSource.Token, cancellationToken)
			.Token;
	}

	public TestExecutor() : this(CancellationToken.None) { }
	public void Cancel() => _cancellationSource.Cancel();

	public void RunTests(IEnumerable<string>? sources, IRunContext? runContext, IFrameworkHandle? frameworkHandle) => throw new NotSupportedException();

	public void RunTests(IEnumerable<TestCase>? tests, IRunContext? runContext, IFrameworkHandle? frameworkHandle)
	{
		if (runContext?.IsBeingDebugged == true && _shouldAttach && !Debugger.IsAttached)
			Debugger.Launch();

		if (frameworkHandle is null) return;
		if (runContext is null) return;
		if (tests is null) return;

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

		var testExecutor = new LeanTestExecutor(logger, frameworkHandle, _cancellationToken);
		testExecutor.ExecuteTests(tests);
	}
}
