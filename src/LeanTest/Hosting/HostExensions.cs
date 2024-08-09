using LeanTest.Hosting.Options;

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
		services.TryAddSingleton<TestRunner>();
		services.TryAddSingleton<TestFactory>();

		services.AddOptions<TestHostingOptions>();

		services.AddOptions<TestOptions>();
		services.TryAddSingleton(s => new TestOptionsProvider(s.GetRequiredService<IConfigurationRoot>(), Environment.GetCommandLineArgs()));
		services.TryAddSingleton(s => s.GetRequiredService<TestOptionsProvider>().Build());

		return services;
	}
}
