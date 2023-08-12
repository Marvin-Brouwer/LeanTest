using LeanTest.TestRunner;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LeanTest.Extensions;

    public static class ServiceExtensions
    {
        public static IServiceCollection AddLeanTestInvoker(this IServiceCollection services)
        {
            services.TryAddSingleton<TestFactory>();
            services.TryAddSingleton<TestInvoker>();

            return services;
        }
    }
