using FluentAssertions;

using LeanTest.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanTest.Behavior.Tests.TestSuites;

public sealed partial class TestSuite : LeanTest.TestSuite.UnitTests
{
	public ITest UnitTest_Success => Test(() =>
	{
		// AAA
	});
	public ITest UnitTest_AssertionFailed_Faillure => Test(() =>
	{
		// AAA
		"Faillure".Should().BeEmpty();
	});
	public ITest UnitTest_CancelRequested_Canceled => Test(() =>
	{
		// AA
		CancellationToken.CancelTestRun();

		// A
		CancellationToken.ForTest.ThrowIfCancellationRequested();
	});
}
