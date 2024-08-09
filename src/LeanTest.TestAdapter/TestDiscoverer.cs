using LeanTest.Hosting.TestAdapter.VsTest;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace LeanTest.TestAdapter;

[DefaultExecutorUri(VsTestExecutor.Id)]
// TODO see if this can be left out vv
[FileExtension(".exe"), FileExtension(".dll")]
internal sealed class TestDiscoverer
	: VsTestDiscoverer;
