using LeanTest.Dynamic.Invocation;
using LeanTest.Dynamic.ReflectionEmitting;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.CodeAnalysis.Emit;
using System.Runtime.Loader;
using LeanTest.Dependencies.Configuration;
using static System.Net.Mime.MediaTypeNames;
using Basic.Reference.Assemblies;

namespace LeanTest.Dynamic;
internal static class RuntimeProxyGenerator
{
	private static readonly PortableExecutableReference SystemRuntimeReference;

	static RuntimeProxyGenerator()
	{
		var runtimeRef = Path.GetDirectoryName(typeof(System.Runtime.GCSettings)!.GetTypeInfo()!.Assembly!.Location)!;
		SystemRuntimeReference = MetadataReference.CreateFromFile(Path.Combine(runtimeRef, "System.Runtime.dll"));
	}

	public static void Create(Type serviceType)
	{
		var originalAssembly = serviceType.Assembly;
		var originalAssemblyName = originalAssembly.GetName()!;

		// TODO package release date

		// This doesn't need to be completely random, this just needs to be random engouh to prevent namespace classes.
		// Based on: https://stackoverflow.com/a/41723783/2319865
		var ticks = new DateTime(2016, 1, 1).Ticks;
		var timeId = DateTime.Now.Ticks - ticks;
		var assemblyName = $"{originalAssemblyName.Name}.RuntimeGenerated<>{timeId:x}";
		var namespaceName = $"{originalAssemblyName.Name}.RuntimeGenerated_{timeId:x}";
		var className = serviceType.Name.CleanClassName();

		var codeToCompile = $$"""
		using {{typeof(MethodBase).Namespace}};
		using {{typeof(IInvokeInterceptor).Namespace}};
		using {{serviceType.Namespace}};

		namespace {{namespaceName}}
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

		// TODO cancellation
		// TODO Language vesion as low as possible, this is to make sure attributes exist
		var syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile, new CSharpParseOptions(LanguageVersion.CSharp7));
		var refPaths = new[] {
			typeof(MethodBase).GetTypeInfo().Assembly.Location,
			typeof(IInvokeInterceptor).GetTypeInfo().Assembly.Location,
			serviceType.GetTypeInfo().Assembly.Location,
		};
		var assemblyReferences = refPaths
			.Select(r => MetadataReference.CreateFromFile(r))
			.Append(SystemRuntimeReference)
			.ToArray();


		var compilation = CSharpCompilation.Create(
			assemblyName,
			syntaxTrees: new[] { syntaxTree },
			references: assemblyReferences,
			options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
		);


#if WRITE_RUNTIME_DLL
		var runtimeAssembly = Assembly.GetEntryAssembly()!;
		var runtimeBinFolder = new FileInfo(runtimeAssembly.Location).Directory!;
		var simplifiedAssemblyName = assemblyName!.Split('<')[0];
#endif
#if WRITE_RUNTIME_DLL
		File.WriteAllText(
				System.IO.Path.Join(runtimeBinFolder.FullName, simplifiedAssemblyName + "." + className + ".generated.cs"),
				codeToCompile
			);
#endif

#if WRITE_RUNTIME_DLL
		var dllFileName = System.IO.Path.Join(runtimeBinFolder.FullName, simplifiedAssemblyName + "." + className + ".dll");
		using (var fs = File.OpenWrite(dllFileName))
#endif
		using (var ms = new MemoryStream())
		{
			var result = compilation.Emit(ms);
			if (!result.Success)
			{
				IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
					diagnostic.IsWarningAsError ||
					diagnostic.Severity == DiagnosticSeverity.Error);

				foreach (Diagnostic diagnostic in failures)
				{
					Console.Error.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
				}

				// TODO throw?
			}
			else
			{
#if WRITE_RUNTIME_DLL
				ms.Seek(0, SeekOrigin.Begin);
				ms.CopyTo(fs);
#endif
				ms.Seek(0, SeekOrigin.Begin);

				var assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
				var generatedType = assembly.GetType($"{namespaceName}.{className}")!;
				
			}
		}
	}

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

		methodBuilder.Append('\t', 2);
		methodBuilder.Append("public ");
		methodBuilder.Append(method.ReturnType == typeof(void)
			? "void"
			: method.ReturnType.FullName
		);
		methodBuilder.Append(" ");
		methodBuilder.Append(method.Name);
		if (method.IsGenericMethod)
		{
			methodBuilder.Append("<");
			foreach ( var genericArgument in method.GetGenericArguments())
			{
				methodBuilder.Append(genericArgument.Name);
			}
			methodBuilder.Append(">");
		}
		methodBuilder.Append("(");
		if (hasParameters)
		{
			for (int i = 0; i < parameters.Length; i++)
			{
				if (i != 0) methodBuilder.Append(", ");
				var parameter = method.GetParameters()[i];
				methodBuilder.Append(parameter.ParameterType.FullName);
				methodBuilder.Append(" ");
				methodBuilder.Append(parameter.Name);
			}
		}
		methodBuilder.Append(") => ");
		methodBuilder.AppendLine();
		methodBuilder.Append('\t', 3);
		methodBuilder.Append("_interceptor.RequestInvoke");
		if (method.ReturnType != typeof(void))
		{
			methodBuilder.Append("<");
			methodBuilder.Append(method.ReturnType.FullName);
			methodBuilder.Append(">");
		}
		methodBuilder.Append("(");
		methodBuilder.AppendLine();
		methodBuilder.Append('\t', 4);
		methodBuilder.Append($"{nameof(MethodBase)}.GetCurrentMethod()");
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
}
