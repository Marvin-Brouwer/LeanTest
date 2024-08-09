using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace LeanTest.Hosting.TestAdapter.Constants;

internal static class TestProperties
{
	public static readonly TestProperty SuiteTypeName = TestProperty.Register("LeanTest.Type", "Type", typeof(string), typeof(TestProperties));
	public static readonly TestProperty PropertyName = TestProperty.Register("LeanTest.Test", "Test", typeof(string), typeof(TestProperties));
	public static readonly TestProperty DataParametersIndex = TestProperty.Register("LeanTest.DataIndex", "DataIndex", typeof(int), typeof(TestProperties));
}
