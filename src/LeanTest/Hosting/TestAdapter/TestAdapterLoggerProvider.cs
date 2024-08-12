using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace LeanTest.Hosting.TestAdapter;

internal sealed class TestAdapterLoggerProvider : ILoggerProvider
{
	public const string CategoryName = "LeanTest.TestAdapterLogger";

	private readonly LoggerFilterOptions _settings;
	private readonly IMessageLogger _messageLogger;

	public TestAdapterLoggerProvider(IMessageLogger messageLogger, IOptions<LoggerFilterOptions> settings)
	{
		_messageLogger = messageLogger;
		_settings = settings.Value;
	}

	public ILogger CreateLogger(string categoryName)
	{
		var logRule = _settings.Rules.LastOrDefault(rule => rule.CategoryName?.Equals(CategoryName) == true);
		var minLogLevel = logRule?.LogLevel ?? _settings.MinLevel;

		// TODO copy settings and filter rules from TestCaseLogger
		return new LogWrapper(_messageLogger, minLogLevel);
	}

	// Redirect the NullLogger BeginScope, it does nothing.
	public void Dispose() => NullLoggerProvider.Instance.Dispose();
}