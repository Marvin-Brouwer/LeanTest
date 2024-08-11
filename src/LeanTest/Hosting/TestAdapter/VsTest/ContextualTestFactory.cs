using LeanTest.Hosting.TestAdapter;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using System.Reflection;
using System.Runtime.CompilerServices;

namespace LeanTest.Hosting.TestAdapter.VsTest;


/// <summary>
/// Just a very dumb version of the <see cref="ITestFactory"/> where we pretend to be IAsyncEnumerable
/// TODO: See if IReadonlyList is feasible if we implement the same discovery
/// </summary>
internal sealed class ContextualTestFactory : ITestFactory
{
	public async IAsyncEnumerable<TestCase> InitializeTests(Assembly assembly, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		var asyncCases = (TestAdapterContext.CurrentFilteredTestCases ?? Array.Empty<TestCase>()).ToAsyncEnumerable().WithCancellation(cancellationToken);
		if (cancellationToken.IsCancellationRequested) yield break;

		await foreach (var testCase in asyncCases) yield return testCase;
	}
}