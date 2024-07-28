using System.Diagnostics;
using System.Runtime.Serialization;

namespace LeanTest.Tests;

public interface ITest
{
	internal int LineNumber { get; }
}
