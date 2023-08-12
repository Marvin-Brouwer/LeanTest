using LeanTest.TestRunner;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LeanTest.Extensions
{
    public static class HostExensions
    {
        public static async Task RunTests<TAssemblyMarker>(this IHost host, CancellationToken cancellationToken = default)
        {
            var runnerCancellationTokenSource = new CancellationTokenSource();
            var runnerCancellationToken = CancellationTokenSource
                .CreateLinkedTokenSource(runnerCancellationTokenSource.Token, cancellationToken)
                .Token;
           
            var applicationLifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
            applicationLifetime.ApplicationStopping.Register(runnerCancellationTokenSource.Cancel);

            await host.StartAsync(runnerCancellationToken);

            var assembly = typeof(TAssemblyMarker).Assembly;
            var testScenarios = host.Services
                .GetRequiredService<TestFactory>()
                .InitializeScenarios(assembly, runnerCancellationToken);
            await host.Services
                .GetRequiredService<TestInvoker>()
                .RunTests(testScenarios, runnerCancellationToken);
        }
    }
}
