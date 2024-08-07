using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;

namespace LeanTest.Tests;

// TODO typed parameters
public sealed record class UnitTestDataScenario: ITest
{
	public IReadOnlyList<object?[]> TestData { get; }
	public Func<object?[], ValueTask> TestBody { get; }
	public int LineNumber { get; }

	// TODO this should be taskCompletionSource
	private static Func<object?[], ValueTask> Wrap(Action<object?[]> testAction) => async (data) =>
	{
		testAction(data);
		await Task.CompletedTask;
	};

	// TODO extract
	private static int GetLineNumber()
	{
		try
		{
			var stackTrace = new StackTrace(true)?.GetFrames() ?? Array.Empty<StackFrame>();
			foreach (var frame in stackTrace)
			{
				if (!frame.HasMethod()) continue;
				if (frame.GetMethod() is not MethodInfo methodInfo) continue;
				if (methodInfo.ReturnType != typeof(ITest)) continue;

				return frame.GetFileLineNumber();
			}
			return -1;
		}
		catch (Exception ex)
		{
			if (!Debugger.IsAttached) Debugger.Launch();
			return -2;
		}
	}

	public UnitTestDataScenario(
		IReadOnlyList<object?[]> testData,
		Func<object?[], ValueTask> testBody
	)
	{
		TestData = testData;
		TestBody = testBody;

		LineNumber = GetLineNumber();
	}
	public UnitTestDataScenario(IEnumerable<object?[]> testData, Action<object?[]> testAction) : this(testData.ToImmutableList(), Wrap(testAction)) { }
}
