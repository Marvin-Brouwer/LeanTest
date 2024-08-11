using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace LeanTest.Hosting;

internal class ResultStreamingLoggerFactory : ILoggerFactory
{
	private readonly IList<TestResultMessage> _logs;
	public IReadOnlyList<TestResultMessage> Logs => _logs.AsReadOnly();

	private readonly LoggerFilterOptions _settings;

	public ResultStreamingLoggerFactory(IOptions<LoggerFilterOptions> factoryOptions)
	{
		_logs = new List<TestResultMessage>();
		_settings = factoryOptions.Value;
	}

	public ILogger CreateLogger(string categoryName) => new ResultStreamingLogger(_logs, categoryName, _settings);

	public void AddProvider(ILoggerProvider provider)
	{
		throw new NotSupportedException("This loggerfactory only supports one type of logger");
	}

	// Redirect the NullLoggerProvider, it does nothing.
	public void Dispose() => NullLoggerFactory.Instance.Dispose();

	private class ResultStreamingLogger : ILogger
	{
		private IList<TestResultMessage> _logs;
		private string _categoryName;
		private LoggerFilterOptions _settings;
		private readonly LoggerFilterRule? _logRule;
		private readonly LogLevel _minLogLevel;

		public ResultStreamingLogger(IList<TestResultMessage> logs, string categoryName, LoggerFilterOptions settings)
		{
			_logs = logs;
			_categoryName = categoryName;
			_settings = settings;
			_logRule = _settings.Rules.LastOrDefault(rule => rule.ProviderName?.Equals(nameof(ResultStreamingLogger)) == true);
			_minLogLevel = _logRule?.LogLevel ?? _settings.MinLevel;
		}

		// Redirect the NullLogger BeginScope, it does nothing.
		public IDisposable? BeginScope<TState>(TState state) where TState : notnull => NullLogger.Instance.BeginScope(state);

		public bool IsEnabled(LogLevel logLevel)
		{
			if (logLevel == LogLevel.None) return false;
			return logLevel >= _minLogLevel;
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
		{
			if (!IsEnabled(logLevel)) return;

			// TODO figure out how _logRule.Filter works

			_logs.Add(new TestResultMessage(_categoryName, $"[{logLevel:G}] {formatter(state, exception)}"));
		}
	}
}