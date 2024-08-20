using System.Diagnostics;
using System.Reflection;

namespace LeanTest.Tests;

public sealed record class UnitTestCase : ITest
{
	public Func<ValueTask> TestBody { get; }
	public string FilePath { get; }
	public int LineNumber { get; }

	// TODO this should be taskCompletionSource
	private static Func<ValueTask> Wrap(Action testAction) => async () =>
	{
		testAction();
		await Task.CompletedTask;
	};

	// TODO extract
	private (string filePath, int lineNumber) GetFileMetaData()
	{
		try
		{
			var stackTrace = new StackTrace(true)?.GetFrames() ?? Array.Empty<StackFrame>();
			foreach (var frame in stackTrace)
			{
				if (!frame.HasMethod()) continue;
				if (frame.GetMethod() is not MethodInfo methodInfo) continue;
				if (methodInfo.ReturnType != typeof(ITest)) continue;

				return (frame.GetFileName()!, frame.GetFileLineNumber());
			}
			return (GetType().FullName!, -1);
		}
		catch (Exception ex)
		{
			if (!Debugger.IsAttached) Debugger.Launch();
			return (ex.Message, -1);
		}
	}

	public UnitTestCase(Func<ValueTask> testBody)
	{
		TestBody = testBody;

		var (filePath, lineNumber) = GetFileMetaData();
		FilePath = filePath;
		LineNumber = lineNumber;
	}

	public UnitTestCase(Action testBody) : this(Wrap(testBody)) { }
}
