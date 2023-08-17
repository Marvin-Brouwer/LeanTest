using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LeanTest.Hosting;
// Move to hosting namespace
public static class HostExensions
{
	public static IServiceCollection AddLeanTestHost<TAssemblyMarker>(this IServiceCollection services) => services
		.AddHostedService<TestHost<TAssemblyMarker>>();
	public static IServiceCollection AddLeanTestInvoker(this IServiceCollection services)
	{
		services.TryAddSingleton<TestFactory>();

		return services;
	}
}
