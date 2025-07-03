using LeanTest.Hosting.TestAdapter;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Diagnostics;

namespace LeanTest.Hosting;

// TODO cleanup unused
public static class TestHost
{
	static TestHost()
	{
		
	}

	public static IHostBuilder CreateDefault(string[]? args)
	{
		var callingType = new StackTrace().GetFrames().First(x => x.GetMethod()?.ReflectedType?.Name == "Program");
		var testAssembly = callingType.GetMethod()!.ReflectedType!.Assembly;

		return Host
			.CreateDefaultBuilder(args)
			.ConfigureAppConfiguration((application, configuration) =>
			{
				var logConfig = new Dictionary<string, string>
				{
					["Logging:LogLevel:Default"] = "Warning",
					["Logging:LogLevel:Microsoft"] = "Warning",
					["Logging:LogLevel:System"] = "Warning",
					["Logging:LogLevel:LeanTest.Hosting.TestFactory"] = "Warning",
					["Logging:LogLevel:LeanTest.Hosting.TestRunner"] = "Warning",
				};
#if (DEBUG)
				logConfig = new Dictionary<string, string>
				{
					["Logging:LogLevel:Default"] = "Trace",
					["Logging:LogLevel:Microsoft"] = "Warning",
					["Logging:LogLevel:System"] = "Warning",
					["Logging:LogLevel:LeanTest.Hosting.TestFactory"] = "Trace",
					["Logging:LogLevel:LeanTest.Hosting.TestRunner"] = "Trace",
				};
#endif
				configuration.AddInMemoryCollection(logConfig!);
			})
			.ConfigureHostConfiguration((configuration) =>
			{
			})
			.ConfigureServices((application, services) =>
			{
				services.AddLeanTestInvoker();
				services.AddLeanTestHost(testAssembly);
			})
			.ConfigureLogging((host, builder) =>
			{
				builder
					.AddFilter("Default", LogLevel.Warning)
					.AddFilter("Microsoft", LogLevel.Warning)
					.AddFilter("System", LogLevel.Warning)
#if DEBUG
					.AddFilter(TestAdapterLoggerProvider.CategoryName, LogLevel.Debug);
#else
					;
#endif

				// TODO also provide builder.AddDefaultLogger
				if (!host.HostingEnvironment.IsDevelopment())
				{
					builder
#if (DEBUG)
						.SetMinimumLevel(LogLevel.Trace)
#else
						.SetMinimumLevel(LogLevel.Information)
#endif
						.AddSimpleConsole();
				}
				else
				{
					builder
						.AddDebug()
						.AddConsole();
				}
				if (TestAdapterContext.HostMessageLogger is not null)
				{
					builder.ClearProviders();
					builder.Services.AddSingleton(TestAdapterContext.HostMessageLogger);
					builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, TestAdapterLoggerProvider>());
				}
			});
	}
}
