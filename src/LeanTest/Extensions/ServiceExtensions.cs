using LeanTest.TestRunner;

using Microsoft.Extensions.DependencyInjection;

namespace LeanTest.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddLeanTestInvoker(this IServiceCollection services)
        {
            // TODO check if inserted

            services.AddSingleton<TestFactory>();
            services.AddSingleton<TestInvoker>();

            return services;
        }
    }
}
