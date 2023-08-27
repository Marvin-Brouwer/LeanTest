using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LeanTest.Hosting;

public static class HostExensions
{
	public static IServiceCollection AddLeanTestHost<TAssemblyMarker>(this IServiceCollection services) => services
		.AddHostedService<TestHost<TAssemblyMarker>>();
	public static IServiceCollection AddLeanTestInvoker(this IServiceCollection services)
	{
		services.TryAddSingleton<TestRunner>();
		services.TryAddSingleton<TestFactory>();
		services.AddOptions<TestHostingOptions>();

		return services;
	}
}
