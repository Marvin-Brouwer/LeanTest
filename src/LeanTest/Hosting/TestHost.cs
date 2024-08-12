using LeanTest.Hosting.Options;
using LeanTest.Hosting.TestAdapter;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace LeanTest.Hosting;

// TODO cleanup unused
public static class TestHost
{
	static TestHost()
	{
		
	}

	// TODO infer TProgram from stackFrome or something and then check if it's the same as the test context
	public static IHostBuilder CreateDefault<TProgram>(string[]? args) where TProgram : class, new() => Host
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
				services.AddLeanTestHost<TProgram>();
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
#if(DEBUG)
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
