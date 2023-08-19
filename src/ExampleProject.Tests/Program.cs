using LeanTest.Hosting;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// TODO Wrap in "TestHost.CreateDefault(args)" ??

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((application, configuration) =>
	{
		// TODO configure threading
		if (!application.HostingEnvironment.IsDevelopment())
		{
			// TODO configure app stop after test run (for pipelines)
		}
	})
    .ConfigureHostConfiguration((configuration) =>
    {
    })
    .ConfigureServices((services) =>
    {
        services.AddLeanTestInvoker();
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