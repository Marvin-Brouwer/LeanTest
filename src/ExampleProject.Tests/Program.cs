using LeanTest.Hosting;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// TODO Wrap in "TestHost.CreateDefault(args)" ??

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((application, configuration) =>
	{
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

var app = builder.Build();

await app.StartAsync();