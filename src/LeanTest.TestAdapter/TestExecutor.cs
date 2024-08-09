using LeanTest.Hosting.TestAdapter.VsTest;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace LeanTest.TestAdapter;

[ExtensionUri(Id)]
internal sealed class TestExecutor : VsTestExecutor;
