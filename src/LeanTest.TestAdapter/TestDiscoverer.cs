using LeanTest.Hosting.TestAdapter.VsTest;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace LeanTest.TestAdapter;

[DefaultExecutorUri(VsTestExecutor.Id)]
internal sealed class TestDiscoverer
	: VsTestDiscoverer;
