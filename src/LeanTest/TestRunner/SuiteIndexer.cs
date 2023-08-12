using System.Reflection;

namespace LeanTest.Indexing;

internal static class SuiteIndexer
{
	internal static IEnumerable<Type> IndexTestSuites(Assembly assembly, CancellationToken cancellationToken)
	{
		foreach (var assemblyScannedType in assembly.GetExportedTypes())
		{
			if (cancellationToken.IsCancellationRequested) yield break;
			if (!assemblyScannedType.IsAssignableTo(typeof(ITestSuite))) continue;

			yield return assemblyScannedType;
		}
	}
}

