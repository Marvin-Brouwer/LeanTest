using System.Diagnostics;
using System.Reflection;

namespace LeanTest.Tests;

public sealed record class UnitTestCase : ITest
{
	public Func<ValueTask> TestBody { get; }
	public int LineNumber { get; }

	// TODO this should be taskCompletionSource
	private static Func<ValueTask> Wrap(Action testAction) => async () =>
	{
		testAction();
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

	public UnitTestCase(Func<ValueTask> testBody)
	{
		TestBody = testBody;
		LineNumber = GetLineNumber();
	}

	public UnitTestCase(Action testBody) : this(Wrap(testBody)) { }
}
