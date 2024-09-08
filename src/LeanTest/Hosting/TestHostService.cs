using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using System.Reflection;

namespace LeanTest.Hosting;

internal class TestHostService : IHostedService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly Assembly _testAssembly;

	public TestHostService(IServiceProvider serviceProvider, Assembly testAssembly)
	{
		_serviceProvider = serviceProvider;
		_testAssembly = testAssembly;
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		var runnerCancellationTokenSource = new CancellationTokenSource();
		var runnerCancellationToken = CancellationTokenSource
			.CreateLinkedTokenSource(runnerCancellationTokenSource.Token, cancellationToken)
			.Token;

		var applicationLifetime = _serviceProvider.GetRequiredService<IHostApplicationLifetime>();
		applicationLifetime.ApplicationStopping.Register(runnerCancellationTokenSource.Cancel);

		var testCases = await _serviceProvider
			.GetRequiredService<ITestFactory>()
			.InitializeTests(_testAssembly, runnerCancellationToken)
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