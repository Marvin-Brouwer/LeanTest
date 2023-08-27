using LeanTest.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Collections.Generic;

// TODO Wrap in "TestHost.CreateDefault(args)" ??

var builder = Host.CreateDefaultBuilder(args)
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
		// TODO remove after testing
		logConfig = new Dictionary<string, string>
		{
			["Logging:LogLevel:Default"] = "Trace",
			["Logging:LogLevel:Microsoft"] = "Warning",
			["Logging:LogLevel:System"] = "Warning",
			["Logging:LogLevel:LeanTest.Hosting.TestFactory"] = "Trace",
			["Logging:LogLevel:LeanTest.Hosting.TestRunner"] = "Trace",
		};
		configuration.AddInMemoryCollection(logConfig!);
	})
    .ConfigureHostConfiguration((configuration) =>
	{
	})
    .ConfigureServices((application, services) =>
	{
		services.AddLeanTestInvoker();
		services.AddLeanTestHost<Program>();

		if (!application.HostingEnvironment.IsDevelopment()) return;

		services.Configure<TestHostingOptions>(testHost =>
		{
			testHost.CloseAfterCompletion = false;
		});
	})
    .ConfigureLogging((host, builder) =>
    {
		builder
			.ClearProviders()
			.AddFilter("Default", LogLevel.Warning)
			.AddFilter("Microsoft", LogLevel.Warning)
			.AddFilter("System", LogLevel.Warning);

		// TODO also provide builder.AddDefaultLogger
		if (!host.HostingEnvironment.IsDevelopment())
		{
			builder
				.SetMinimumLevel(LogLevel.Information)
				.AddSimpleConsole();
		}
		else
		{
			builder
				.AddDebug()
				.AddConsole();
		}
    });

var app = builder.Build();

await app.StartAsync();