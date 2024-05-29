using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanTest.Hosting.Options;

public sealed class TestOptions
{
	public int Port { get; set; }
	// TODO test with values
	public IReadOnlyList<string>? TestPatterns { get; set; }
}
