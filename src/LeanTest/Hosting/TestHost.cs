using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
		var testScenarios = _serviceProvider
			.GetRequiredService<TestFactory>()
			.InitializeScenarios(assembly, runnerCancellationToken);
		await _serviceProvider
			.GetRequiredService<TestInvoker>()
			.RunTests(testScenarios, runnerCancellationToken);
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}