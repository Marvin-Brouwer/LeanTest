using LeanTest.TestAdapter.Logging;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace LeanTest.TestAdapter.Adapter;

internal static class TestAdapterExtensions
{
	public static bool IsTestAssembly(this string assemblyPath)
	{
		var testAssemblies = new[]
		{
			"LeanTest.dll"
		};

		if (testAssemblies.Contains(Path.GetFileName(assemblyPath)))
			return false;

		return File.Exists(Path.Combine(FolderPath(assemblyPath), "LeanTest.dll"));
	}
	public static string FolderPath(string assemblyPath)
	{
		return new FileInfo(assemblyPath).Directory!.FullName;
	}

	public static ILogger Wrap(this IMessageLogger logger)
	{
		return new LogWrapper(logger);
	}

}
