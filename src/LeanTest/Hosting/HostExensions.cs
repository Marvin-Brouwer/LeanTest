using LeanTest.Hosting.Options;
using LeanTest.Hosting.TestAdapter;
using LeanTest.Hosting.TestAdapter.Standalone;
using LeanTest.Hosting.TestAdapter.VsTest;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LeanTest.Hosting;

public static class HostExensions
{
	public static IServiceCollection AddLeanTestHost<TAssemblyMarker>(this IServiceCollection services) => services
		.AddHostedService<TestHostService<TAssemblyMarker>>();
	public static IServiceCollection AddLeanTestInvoker(this IServiceCollection services)
	{
		// Configure discovery and state services
		services.AddSingleton<TestResultBuilder>();

		if (TestAdapterContext.CurrentFilteredTestCases is not null)
		{
			services.AddSingleton<ITestFactory, ContextualTestFactory>();
			services.AddSingleton(TestAdapterContext.HostExecutionRecorder!);
		}
		else
		{
			services.AddSingleton<ITestFactory, DiscoveringTestFactory>();
			// TODO custom execution recorder
			//services.AddSingleton<ITestExecutionRecorder>(TestAdapterContext.HostExecutionRecorder!);
		}

		// Configure settings
		services.AddOptions<TestHostingOptions>();
		services.AddOptions<TestOptions>();
		services.TryAddSingleton(s => new TestOptionsProvider(s.GetRequiredService<IConfigurationRoot>(), Environment.GetCommandLineArgs()));
		services.TryAddSingleton(s => s.GetRequiredService<TestOptionsProvider>().Build());

		// Add the actual TestRunner 
		services.AddSingleton<TestRunner>();

		return services;
	}
}
