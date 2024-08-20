using LeanTest.Hosting.TestAdapter.VsTest;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace LeanTest.TestAdapter;

[DefaultExecutorUri(LeanTestExecutor.Id)]
internal sealed class LeanTestVsTestDiscoverer : LeanTestDiscoverer;
