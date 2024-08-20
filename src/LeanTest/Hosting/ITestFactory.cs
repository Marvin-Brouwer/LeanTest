using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using System.Reflection;
using System.Runtime.CompilerServices;

namespace LeanTest.Hosting;
internal interface ITestFactory
{
	IAsyncEnumerable<TestCase> InitializeTests(Assembly assembly, [EnumeratorCancellation] CancellationToken cancellationToken);
}