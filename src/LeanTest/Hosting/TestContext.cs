using LeanTest.Dependencies.Providers;
using LeanTest.Dynamic.Generating;

using Microsoft.Extensions.Logging;

namespace LeanTest.Hosting;

/// <summary>
/// We need a way to access test contextual instances. <br />
/// Since we'd like these to be available before the <see cref="TestSuite{TSut}"/> calls it's constructor,
/// and we don't like to pass it around in a base constructor, this seems the only suitable way. <br />
/// Using reflection cannot set these values on an instance of <see cref="TestSuite{TSut}"/> before the constructor
/// gets invoked.
/// </summary>
public record struct TestContext
{
	/// <summary>
	/// Access a <see cref="ThreadStatic"/> shared instance of this class. 
	/// </summary>
	[ThreadStatic]
	internal static TestContext Current;

	/// <inheritdoc cref="ICancellationTokenProvider"/>
	internal ICancellationTokenProvider TestCancellationToken;

	/// <summary>
	/// An <see cref="ILoggerFactory"/> that's able to produce an <see cref="ILogger{TCategoryName}"/>
	/// which logs to the configured test runners output
	/// </summary>
	internal ILoggerFactory TestLoggerFactory;

	/// <inheritdoc cref="RuntimeAssemblyContext"/>
	internal RuntimeAssemblyContext AssemblyContext;

	static TestContext()
	{
		Current.TestLoggerFactory = LoggerFactory.Create((c) => { });
		Current.TestCancellationToken = new CancellationTokenProvider(CancellationToken.None);
		Current.AssemblyContext = new RuntimeAssemblyContext(typeof(TestContext).Assembly);
	}
}

