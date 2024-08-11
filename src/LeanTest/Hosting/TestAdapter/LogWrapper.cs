using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace LeanTest.Hosting.TestAdapter;

internal sealed class LogWrapper : ILogger
{
	private readonly IMessageLogger _messageLogger;
	private readonly LogLevel _minimumLogLevel;

	public LogWrapper(IMessageLogger messageLogger, LogLevel minimumLogLevel)
	{
		_messageLogger = messageLogger;
		_minimumLogLevel = minimumLogLevel;
	}

	// Redirect the NullLogger BeginScope, it does nothing.
	public IDisposable? BeginScope<TState>(TState state) where TState : notnull => NullLogger.Instance.BeginScope(state);

	public bool IsEnabled(LogLevel logLevel)
	{
		if (logLevel == LogLevel.None) return false;
		return logLevel >= _minimumLogLevel;
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
			LogLevel.Warning => TestMessageLevel.Warning,
			LogLevel.Error => TestMessageLevel.Error,
			LogLevel.Critical => TestMessageLevel.Error,
			_ => throw new NotSupportedException(),
		};

		_messageLogger.SendMessage(actualLogLevel, $"[{logLevel:G}] {formatter(state, exception)}");
	}
}
