using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanTest.Hosting.TestAdapter;
internal static class TestAdapterExtensions
{
	public static bool IsTestAssembly(this string assemblyPath)
	{
		var fixieAssemblies = new[]
		{
			"LeanTest.dll"
		};

		if (fixieAssemblies.Contains(Path.GetFileName(assemblyPath)))
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
