using LeanTest.Hosting;
using LeanTest.Hosting.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;

// TODO Wrap in "TestHost.CreateDefault(args)" ??

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((application, configuration) =>
	{
	})
    .ConfigureHostConfiguration((config) =>
    {

    })
    .ConfigureServices((services) =>
    {
        services.AddLeanTestInvoker();
		services.AddOptions<TestOptions>();
		services.TryAddSingleton(s => new TestOptionsProvider(s.GetRequiredService<IConfigurationRoot>(), Environment.GetCommandLineArgs()));
		services.TryAddSingleton(s => s.GetRequiredService<TestOptionsProvider>().Build());
		services.AddLeanTestHost<Program>();
    })
    .ConfigureLogging((host, builder) =>
    {
		// TODO also provide builder.AddDefaultLogger
		if (!host.HostingEnvironment.IsDevelopment())
		{
			builder.AddSimpleConsole();
		}
		else
		{
			builder.AddConsole();
		}
    });

await builder
	.Build()
	.StartAsync();