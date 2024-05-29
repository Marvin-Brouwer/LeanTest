using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanTest.Hosting.TestAdapter;

[ExtensionUri(Id)]
public sealed class TestExecutor : ITestExecutor
{
	public const string Id = "executor://leantest.testadapter/";
	public static readonly Uri Uri = new(Id);

	private readonly CancellationTokenSource _cancellationSource;
	private readonly CancellationToken _cancellationToken;
	// TODO this should come from settings
	private readonly bool _shouldAttach = true;

	public TestExecutor(CancellationToken cancellationToken)
	{
		_cancellationSource = new CancellationTokenSource();
		_cancellationToken = CancellationTokenSource
			.CreateLinkedTokenSource(_cancellationSource.Token, cancellationToken)
			.Token;
	}

	public TestExecutor() : this (CancellationToken.None) { }
	public void Cancel() => _cancellationSource.Cancel();

	public void RunTests(IEnumerable<string>? sources, IRunContext? runContext, IFrameworkHandle? frameworkHandle) => throw new NotSupportedException();

	public void RunTests(IEnumerable<TestCase>? tests, IRunContext? runContext, IFrameworkHandle? frameworkHandle)
	{
		if (runContext?.IsBeingDebugged == true && _shouldAttach && !Debugger.IsAttached)
			Debugger.Launch();

		throw new NotImplementedException();
	}
}
