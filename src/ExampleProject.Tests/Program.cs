

using LeanTest.Extensions;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// TODO Wrap in "TestHost.CreateDefault(args)"

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
    })
    .ConfigureLogging(builder =>
    {
        builder.AddConsole();
    });

var host =  builder.Build();

await host
    .RunTests<Program>();

await host
    .WaitForShutdownAsync();