using System.Diagnostics;
using System.Reflection;

namespace LeanTest.Tests;

public abstract record class Test : ITest
{
	public Delegate TestBody { get; }
	public string FilePath { get; }
	public int LineNumber { get; }

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

	public Test(Delegate testBody)
	{
		TestBody = testBody;

		var (filePath, lineNumber) = GetFileMetaData();
		FilePath = filePath;
		LineNumber = lineNumber;
	}
}
