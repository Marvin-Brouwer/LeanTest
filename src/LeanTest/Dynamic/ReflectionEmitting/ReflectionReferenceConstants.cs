using LeanTest.Dynamic.Invocation;

using System.Reflection;

namespace LeanTest.Dynamic.ReflectionEmitting;

internal readonly record struct ReflectionReferenceConstants
{
	internal static readonly MethodInfo GetCurrentMethod = typeof(MethodBase)
		.GetMethod(nameof(MethodBase.GetCurrentMethod))!;

	internal static readonly MethodInfo ObjectArrayEmpty = typeof(Array)
		.GetMethod(nameof(Array.Empty))!
		.GetGenericMethodDefinition()!
		.MakeGenericMethod(new[] { typeof(object) })!;

	internal static readonly MethodInfo InvokeGenericNoParameters = GetInterceptorMethod(false, false);
	internal static readonly MethodInfo InvokeGenericWithParameters = GetInterceptorMethod(false, true);
	internal static readonly MethodInfo InvokeVoidNoParameters = GetInterceptorMethod(true, false);
	internal static readonly MethodInfo InvokeVoidWithParameters = GetInterceptorMethod(true, true);

	private static MethodInfo GetInterceptorMethod(bool isVoid, bool hasParameters)
	{
		foreach(var method in typeof(IInvokeInterceptor).GetMethods())
		{
			if (isVoid && method.ReturnType != typeof(void)) continue;
			if (!isVoid && method.ReturnType == typeof(void)) continue;
			if (hasParameters && method.GetParameters().Length == 1) continue;

			return method;
		}

		throw new NotSupportedException();
	}
}
