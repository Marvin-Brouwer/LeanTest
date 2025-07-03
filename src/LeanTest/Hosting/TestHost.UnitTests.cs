using LeanTest.TestInvokers;

using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanTest.Hosting;

public static class TestHostExtensionsForUnitTests
{
	public static IHostBuilder AddUnitTests(this IHostBuilder hostBuilder)
	{
		hostBuilder.ConfigureServices(s => s.TryAddSingleton<ITestInvoker, UnitTestInvoker>());
		return hostBuilder;
	}
}
