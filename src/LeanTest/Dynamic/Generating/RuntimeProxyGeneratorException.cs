using LeanTest.Exceptions;

using Microsoft.CodeAnalysis;

using System.Runtime.Serialization;
using System.Text;

namespace LeanTest.Dynamic.Generating;

#if (!NET8_0_OR_GREATER)
[Serializable]
#endif
public sealed class RuntimeProxyGeneratorException : LeanTestException
{
	internal static RuntimeProxyGeneratorException CompilationFaillure(Type serviceType, IReadOnlyList<Diagnostic> diagnostics)
	{
		var faillures = diagnostics
			.Where(diagnostic =>
				diagnostic.IsWarningAsError ||
				diagnostic.Severity == DiagnosticSeverity.Error
			)
			.ToArray();

		var message = new StringBuilder(128);
		message.AppendLine($"Failed to compile \"{serviceType.FullName}\".");

		foreach (Diagnostic diagnostic in faillures)
		{
			message.AppendFormat("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
			message.AppendLine();
		}

		return new(faillures, diagnostics, message.ToString());
	}

	/// <summary>
	/// Fake the Diagnostic thrown by attempting to inherit from a sealed type.
	/// This is just to short cirquit compiling an entire proxy that fails anyway.
	/// </summary>
	internal static RuntimeProxyGeneratorException SealedType(Type serviceType)
	{
		// https://learn.microsoft.com/en-us/dotnet/csharp/misc/cs0509?f1url=%3FappId%3Droslyn%26k%3Dk(CS0509)
		var description = $"Proxy generated class cannot derive from sealed type '{serviceType.Name}'";
		var descriptor = new DiagnosticDescriptor(
			$"CS0509", "Failed to compile \"{serviceType.FullName}\".", description, "Compiler", DiagnosticSeverity.Error, true,
			description, "https://learn.microsoft.com/en-us/dotnet/csharp/misc/cs0509?f1url=%3FappId%3Droslyn%26k%3Dk(CS0509)"
		);

		var diagnostic = Diagnostic.Create(descriptor, Location.None);

		var faillures = new List<Diagnostic>
		{
			diagnostic
		};

		return new(faillures, faillures, diagnostic.GetMessage());
	}

	public IReadOnlyList<Diagnostic> Faillures { get; }
	public IReadOnlyList<Diagnostic> FullDiagnostics { get; }

	private RuntimeProxyGeneratorException(IReadOnlyList<Diagnostic> faillures, IReadOnlyList<Diagnostic> diagnostics, string message)
		: base(message)
	{
		Faillures = faillures;
		FullDiagnostics = diagnostics;
	}

#if (!NET8_0_OR_GREATER)
	#region Serializable
	private RuntimeProxyGeneratorException(in SerializationInfo info, in StreamingContext context) : base(in info, in context)
	{
		Faillures = (IReadOnlyList<Diagnostic>)info.GetValue(nameof(Faillures), typeof(IReadOnlyList<Diagnostic>))!;
		FullDiagnostics = (IReadOnlyList<Diagnostic>)info.GetValue(nameof(FullDiagnostics), typeof(IReadOnlyList<Diagnostic>))!;
	}

	/// <inheritdoc />
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue(nameof(Faillures), Faillures);
		info.AddValue(nameof(FullDiagnostics), FullDiagnostics);

		base.GetObjectData(info, context);
	}
	#endregion Serializable
#endif
}
