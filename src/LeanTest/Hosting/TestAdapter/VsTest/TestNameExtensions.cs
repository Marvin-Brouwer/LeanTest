using System.Reflection;
using System.Text;

namespace LeanTest.Hosting.TestAdapter.VsTest;

internal static class TestNameExtensions
{
	internal static (string fullyQualifiedName, string displayName) GetTestNames(this PropertyInfo testProperty, string testSuiteName, object?[] testData)
	{

		var baseName = GenerateBaseName(testProperty, testSuiteName);
		var dataName = GenerateDataName(testData);
		var displayName = baseName + dataName;

		return (baseName + "...", displayName);
	}
	internal static (string fullyQualifiedName, string displayName) GetTestNames(this PropertyInfo testProperty, string testSuiteName)
	{
		var baseName = GenerateBaseName(testProperty, testSuiteName);
		var displayName = baseName + "()";

		return (baseName, displayName);
	}

	private static string GenerateBaseName(PropertyInfo testProperty, string testSuiteName)
	{

		return $"{testSuiteName}.{testProperty.Name}";
	}

	private static string GenerateDataName(object?[] dataTestCase)
	{
		var stringBuilder = new StringBuilder(64);
		stringBuilder.Append('(');
		var first = true;
		foreach (var input in dataTestCase)
		{
			if (first) first = false;
			else stringBuilder.Append(", ");

			if (input is null)
			{
				stringBuilder.Append("null");
				continue;
			}
			if (input is string)
			{
				stringBuilder.Append('"');
				stringBuilder.Append(input);
				stringBuilder.Append('"');
				continue;
			}
			if (input is bool booleanInput)
			{
				stringBuilder.Append(booleanInput ? "true" : "false");
				continue;
			}
			if (input.GetType().IsPrimitive)
			{
				stringBuilder.Append(input.ToString());
				continue;
			}

			stringBuilder.Append(input.ToString());

		}
		stringBuilder.Append(')');

		return stringBuilder.ToString();
	}
}
