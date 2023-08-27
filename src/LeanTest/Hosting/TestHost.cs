using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace LeanTest.Hosting;

internal class TestHost<TAssembly> : IHostedService
{
	private readonly IServiceProvider _serviceProvider;

	public TestHost(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		var runnerCancellationTokenSource = new CancellationTokenSource();
		var runnerCancellationToken = CancellationTokenSource
			.CreateLinkedTokenSource(runnerCancellationTokenSource.Token, cancellationToken)
			.Token;

		var applicationLifetime = _serviceProvider.GetRequiredService<IHostApplicationLifetime>();
		applicationLifetime.ApplicationStopping.Register(runnerCancellationTokenSource.Cancel);

		var assembly = typeof(TAssembly).Assembly;
		var testCases = await _serviceProvider
			.GetRequiredService<TestFactory>()
			.InitializeTests(assembly, runnerCancellationToken)
			.ToArrayAsync(cancellationToken);

		await _serviceProvider
			.GetRequiredService<TestRunner>()
			.RunTests(testCases, runnerCancellationToken);

		var testHostConfiguration = _serviceProvider.GetRequiredService<IOptions<TestHostingOptions>>();
		if (testHostConfiguration.Value.CloseAfterCompletion)
			applicationLifetime.StopApplication();
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		// Unecessary to stop tasks due to CancellationToken everywhere
		return Task.CompletedTask;
	}
}