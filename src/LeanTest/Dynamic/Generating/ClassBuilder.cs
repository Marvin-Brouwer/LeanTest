using LeanTest.Dynamic.Invocation;

using System.Reflection;
using System.Text;

namespace LeanTest.Dynamic.Generating;

internal static class ClassBuilder
{
	// TODO Generate properties
	internal static string GenerateProxyClass(RuntimeAssemblyContext context, Type serviceType, string className) =>
		$$"""
		using {{typeof(MethodBase).Namespace}};
		using {{typeof(IInvokeInterceptor).Namespace}};
		using {{serviceType.Namespace}};

		namespace {{context.NamespaceName}}
		{
			public class {{className}} : {{serviceType.Name}}
			{
				private readonly IInvokeInterceptor _interceptor;

				public {{className}}(IInvokeInterceptor interceptor) {
					_interceptor = interceptor;
				}

				// TODO Generate properties

				{{GenerateMethods(serviceType)}}
			}
		}
		""";

	private static string GenerateMethods(Type serviceType)
	{
		var methodBuilder = new StringBuilder(128);

		foreach (MethodInfo method in serviceType.GetMethods())
		{
			methodBuilder.AppendLine();
			GenerateMethod(methodBuilder, method);
		}

		return methodBuilder.ToString();
	}

	private static void GenerateMethod(StringBuilder methodBuilder, MethodInfo method)
	{
		var parameters = method.GetParameters();
		var hasParameters = parameters.Length > 0;
		var isVoidMethod = method.ReturnType == typeof(void);

		methodBuilder.Append('\t', 2);
		methodBuilder.Append("public ");
		methodBuilder.Append(isVoidMethod
			? "void"
			: FormatType(method.ReturnType)
		);
		methodBuilder.Append(' ');
		methodBuilder.Append(method.Name);
		if (method.IsGenericMethod)
		{
			methodBuilder.Append('<');
			var genericArguments = method.GetGenericArguments();
			for (int i = 0; i < genericArguments.Length; i++)
			{
				if (i != 0) methodBuilder.Append(", ");
				methodBuilder.Append(genericArguments[i].Name);
			}
			methodBuilder.Append('>');
		}
		methodBuilder.Append('(');
		if (hasParameters)
		{
			for (int i = 0; i < parameters.Length; i++)
			{
				if (i != 0) methodBuilder.Append(", ");
				var parameter = method.GetParameters()[i];
				methodBuilder.Append(FormatType(parameter.ParameterType));
				methodBuilder.Append(' ');
				methodBuilder.Append(parameter.Name);
			}
		}
		methodBuilder.Append(") => ");
		methodBuilder.AppendLine();
		methodBuilder.Append('\t', 3);
		methodBuilder.Append("_interceptor.RequestInvoke");
		if (!isVoidMethod)
		{
			methodBuilder.Append('<');
			methodBuilder.Append(FormatType(method.ReturnType));
			methodBuilder.Append('>');
		}
		methodBuilder.Append('(');
		methodBuilder.AppendLine();
		methodBuilder.Append('\t', 4);
		methodBuilder.Append($"{nameof(MethodBase)}.{nameof(MethodBase.GetCurrentMethod)}()");
		if (hasParameters)
		{
			methodBuilder.Append(',');
			methodBuilder.AppendLine();
			methodBuilder.Append('\t', 4);
			methodBuilder.Append("new object[] { ");
			for (int i = 0; i < parameters.Length; i++)
			{
				if (i != 0) methodBuilder.Append(", ");
				var parameter = parameters[i];
				methodBuilder.Append(parameter.Name);
			}
			methodBuilder.Append(" }");
		}
		methodBuilder.AppendLine();
		methodBuilder.Append('\t', 3);
		methodBuilder.Append(");");
	}

	private static string FormatType(Type returnType)
	{
		if (!returnType.IsGenericType)
			return (returnType.FullName ?? returnType.Name).Trim();

		var genericType = returnType.GetGenericTypeDefinition();
		var bareTypeName = (genericType.FullName ?? genericType.Name).Split('`')[0];

		var genericTypeBuilder = new StringBuilder(32);
		genericTypeBuilder.Append(bareTypeName);
		genericTypeBuilder.Append('<');
		var innerTypes = returnType.GetGenericArguments();
		for (int i = 0; i < innerTypes.Length; i++)
		{
			if (i != 0) genericTypeBuilder.Append(", ");

			var innerType = innerTypes[i]!;
			genericTypeBuilder.Append(FormatType(innerType));
		}
		genericTypeBuilder.Append('>');

		return genericTypeBuilder.ToString();
	}
}
