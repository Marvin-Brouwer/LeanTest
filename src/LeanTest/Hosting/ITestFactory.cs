using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using System.Reflection;
using System.Runtime.CompilerServices;

namespace LeanTest.Hosting;
internal interface ITestFactory
{
#pragma warning disable CS8424 // The EnumeratorCancellationAttribute will have no effect. The attribute is only effective on a parameter of type CancellationToken in an async-iterator method returning IAsyncEnumerable
	IAsyncEnumerable<TestCase> InitializeTests(Assembly assembly, [EnumeratorCancellation] CancellationToken cancellationToken);
#pragma warning restore CS8424 // The EnumeratorCancellationAttribute will have no effect. The attribute is only effective on a parameter of type CancellationToken in an async-iterator method returning IAsyncEnumerable
}