using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace LeanTest.TestAdapter.Logging;
internal sealed class LogWrapper : ILogger
{
	private readonly IMessageLogger _messageLogger;

	public LogWrapper(IMessageLogger messageLogger)
	{
		_messageLogger = messageLogger;
	}

	public IDisposable? BeginScope<TState>(TState state) where TState : notnull
	{
		throw new NotImplementedException();
	}

	public bool IsEnabled(LogLevel logLevel)
	{
		if (logLevel == LogLevel.None) return false;

		// TODO get from settings or default to information
		return true;
	}

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		if (!IsEnabled(logLevel)) return;

		var actualLogLevel = logLevel switch
		{
			LogLevel.None => throw new NotSupportedException(),
			LogLevel.Trace => TestMessageLevel.Informational,
			LogLevel.Debug => TestMessageLevel.Informational,
			LogLevel.Information => TestMessageLevel.Informational,
			LogLevel.Warning => TestMessageLevel.Informational,
			LogLevel.Error => TestMessageLevel.Error,
			LogLevel.Critical => TestMessageLevel.Error,
			_ => throw new NotSupportedException(),
		};

		_messageLogger.SendMessage(actualLogLevel, $"[{logLevel:G}] {formatter(state, exception)}");
	}
}
