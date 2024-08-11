using LeanTest.Hosting.TestAdapter;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LeanTest.Hosting;

// TODO this is a test, actually the context should get providers
internal class ContextAwareLogProvider : ILoggerProvider
{
	internal static ContextAwareLogProvider Instance = new();

	public ILogger CreateLogger(string categoryName)
	{
		return TestAdapterContext.HostLogger!;
	}

	public void Dispose() => NullLoggerProvider.Instance.Dispose();
}