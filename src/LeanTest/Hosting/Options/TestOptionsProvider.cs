using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanTest.Hosting.Options;

public sealed class TestOptionsProvider
{
	private readonly IConfigurationSection _configuration;
	private readonly string[] _arguments;

	public TestOptionsProvider(IConfigurationRoot config, string[] commandLineArguments)
	{
		_configuration = config.GetSection(nameof(TestOptions));
		_arguments = commandLineArguments;
	}

	public IOptions<TestOptions> Build()
	{
		var namedArguments = GetNamedArguments();
		var section = _configuration.Get<TestOptions>() ?? new TestOptions();

		if (namedArguments.TryGetValue("--port", out var port))
			section.Port = Convert.ToInt32(port);

		return new OptionsWrapper<TestOptions>(section);
	}

	private Dictionary<string, string> GetNamedArguments()
	{
		var arguments = new Dictionary<string, string>();
		foreach (var argument in _arguments)
		{
			if (!argument.StartsWith("--", StringComparison.Ordinal)) continue;
			var namedArgument = argument.Split(' ', 2, StringSplitOptions.TrimEntries);
			arguments.Add(namedArgument[0], namedArgument[1]);
		}
		return arguments;
	}
}
