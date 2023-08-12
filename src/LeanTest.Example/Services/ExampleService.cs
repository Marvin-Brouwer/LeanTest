using Microsoft.Extensions.Logging;

namespace LeanTest.Example.Services
{
    public class ExampleService : IExampleService
    {

        public ExampleService(ISomeThing something, IServiceOutOfScope outOfScope, ILogger<IExampleService> logger)
        {
        }

        public async Task<string> DoThing(string thing) => await Task.FromResult(thing);
    }
}