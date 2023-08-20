namespace LeanTest.Dynamic.Generating;

internal static class ReflectionExtensions
{
	internal static string CleanClassName(this Type serviceType)
	{
		const string BaseFix = "Base";
		const int BaseFixLength = 4;

		var serviceTypeName = serviceType.Name;

		var cleanServiceName = serviceTypeName.TrimStart('I');
		if (serviceTypeName.StartsWith(BaseFix, StringComparison.Ordinal))
			cleanServiceName = cleanServiceName.Substring(BaseFixLength);
		else if (serviceTypeName.EndsWith(BaseFix, StringComparison.Ordinal))
			cleanServiceName = cleanServiceName.Substring(0, cleanServiceName.Length - BaseFixLength);

		return cleanServiceName;
	}

	public static TService InitializeType<TService>(this Type generatedType, params object[] constructorParameters)
		where TService : class
	{
		var wrappedInstance = Activator.CreateInstance(generatedType, constructorParameters)!;
		return (TService)wrappedInstance;
	}
}
