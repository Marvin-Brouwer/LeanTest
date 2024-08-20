using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace LeanTest.Hosting.TestAdapter;

/// <summary>
/// Ideally you'd have a standalone test runner in charge of discovery and execution and VS would just expose a socket of some kind to post updates to.
/// However, we don't have that. So, we'll resort to globals.
/// </summary>
public static class TestAdapterContext
{
	public static ITestExecutionRecorder? HostExecutionRecorder { get; internal set; }
	public static IMessageLogger? HostMessageLogger { get; internal set; }
	public static IReadOnlyList<TestCase>? CurrentFilteredTestCases { get; internal set; }
	public static CancellationToken HostCancelationToken { get; internal set; }
}
