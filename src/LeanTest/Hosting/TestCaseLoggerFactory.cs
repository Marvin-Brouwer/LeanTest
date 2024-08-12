using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace LeanTest.Hosting;

internal sealed class TestCaseLoggerFactory : ILoggerFactory
{
	private readonly IList<TestResultMessage> _logs;
	public IReadOnlyList<TestResultMessage> Logs => _logs.AsReadOnly();

	private readonly LoggerFilterOptions _settings;

	public TestCaseLoggerFactory(IOptions<LoggerFilterOptions> factoryOptions)
	{
		_logs = new List<TestResultMessage>();
		_settings = factoryOptions.Value;
	}

	public ILogger CreateLogger(string categoryName) => new TestCaseLogger(_logs, categoryName, _settings);

	public void AddProvider(ILoggerProvider provider) =>
		throw new NotSupportedException("This loggerfactory only supports one type of logger");

	// Redirect the NullLoggerProvider, it does nothing.
	public void Dispose() => NullLoggerFactory.Instance.Dispose();

	private sealed class TestCaseLogger : ILogger
	{
		private const string CategoryName = "LeanTest.TestLogger";

		private IList<TestResultMessage> _logs;
		private string _testCategoryName;
		private LoggerFilterOptions _settings;
		private readonly string _providerName;
		private readonly LoggerFilterRule? _logRule;
		private readonly LogLevel _minLogLevel;

		public TestCaseLogger(IList<TestResultMessage> logs, string categoryName, LoggerFilterOptions settings)
		{
			_logs = logs;
			_testCategoryName = categoryName + "." + CategoryName;
			_settings = settings;

			_providerName = GetType().FullName!;
			_logRule = _settings.Rules.LastOrDefault(rule => rule.CategoryName?.Equals(nameof(CategoryName)) == true);
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

			var filterLogLevel = _settings.Rules
				.Where(rule => rule.Filter?.Invoke(_providerName, _testCategoryName, logLevel) == true)
				.Select(rule => rule.LogLevel)
				.Where(level => level is not null)
				.Aggregate(_minLogLevel, (aggregate, current) => (current!.Value < aggregate) ? current.Value : aggregate);

			if (logLevel <= filterLogLevel) return;

			_logs.Add(new TestResultMessage(_testCategoryName, $"[{logLevel:G}] {formatter(state, exception)}"));
		}
	}
}