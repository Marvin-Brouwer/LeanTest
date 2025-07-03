using LeanTest.Hosting;
using LeanTest.Tests;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanTest.TestInvokers;
internal interface ITestInvoker
{
	bool SupportsSuite(ITestSuite test);
	Task<TestResult> Invoke(
		TestCase testCase, ITest test, TestCaseLoggerFactory loggerFactory,
		CancellationToken cancellationToken
	);
}
