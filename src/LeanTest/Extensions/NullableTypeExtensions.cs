using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;

namespace FluentSerializer.Core.Extensions;

/// <summary>
/// Extension methods to help reflecting additional type information
/// </summary>
internal static class NullableTypeExtensions
{
	private const int NullableArgumentType = 2;
	private const string NullableAttributeName = "System.Runtime.CompilerServices.NullableAttribute";
	private const string NullableContextAttributeName = "System.Runtime.CompilerServices.NullableContextAttribute";

	/// <inheritdoc cref="IsNullable(Type)"/>
	public static bool IsNullable(this PropertyInfo property)
	{
		Debug.Assert(property is not null);
		return CheckIfNullable(property.PropertyType, property.DeclaringType, property.CustomAttributes);
	}

	/// <inheritdoc cref="IsNullable(Type)"/>
	public static bool IsNullable(this FieldInfo field)
	{
		Debug.Assert(field is not null);
		return CheckIfNullable(field.FieldType, field.DeclaringType, field.CustomAttributes);
	}

	/// <inheritdoc cref="IsNullable(Type)"/>
	public static bool IsNullable(this ParameterInfo parameter)
	{
		Debug.Assert(parameter is not null);
		return CheckIfNullable(parameter.ParameterType, parameter.Member, parameter.CustomAttributes);
	}

	/// <summary>
	/// Check whether this type is attributed to allow null values
	/// </summary>
	public static bool IsNullable(this Type type)
	{
		Debug.Assert(type is not null);
		return CheckIfNullable(type, null, type.CustomAttributes);
	}

	internal static bool CheckIfNullable(in Type memberType, in MemberInfo? declaringType, in IEnumerable<CustomAttributeData> customAttributes)
	{
		var typeToCheck = memberType.IsByRef ? memberType.GetElementType()! : memberType;

		if (typeToCheck.IsValueType)
			return Nullable.GetUnderlyingType(typeToCheck) != null;

		if (HasNullableAttribute(customAttributes)) return true;
		if (HasNullableContextAttribute(declaringType)) return true;

		// Couldn't find a suitable attribute
		return false;
	}

	private static bool HasNullableContextAttribute(MemberInfo? declaringType)
	{
		if (declaringType is null) return false;

		for (var type = declaringType; type != null; type = type.DeclaringType)
		{
			// Use full name so any runtime will pass
			var context = type.CustomAttributes
				.FirstOrDefault(attribute => attribute.AttributeType.FullName == NullableContextAttributeName);
			if (context is null) continue;

			if (context.ConstructorArguments.Count == 1 && context.ConstructorArguments[0].ArgumentType == typeof(byte))
				return (byte)context.ConstructorArguments[0].Value! == NullableArgumentType;
		}

		return false;
	}

	private static bool HasNullableAttribute(IEnumerable<CustomAttributeData> customAttributes)
	{
		// Use full name so any runtime will pass
		var nullableCompilerServiceAttribute = customAttributes
			.FirstOrDefault(attribute => attribute.AttributeType.FullName == NullableAttributeName);
		if (nullableCompilerServiceAttribute is null) return false;
		if (nullableCompilerServiceAttribute.ConstructorArguments.Count != 1) return false;

		var attributeArgument = nullableCompilerServiceAttribute.ConstructorArguments[0];
		if (attributeArgument.ArgumentType == typeof(byte[]))
		{
			var attributeArguments = (ReadOnlyCollection<CustomAttributeTypedArgument>)attributeArgument.Value!;
			if (attributeArguments.Count > 0
				&& attributeArguments[0].ArgumentType == typeof(byte)) return (byte)attributeArguments[0].Value! == NullableArgumentType;
		}

		if (attributeArgument.ArgumentType == typeof(byte))
			return (byte)attributeArgument.Value! == NullableArgumentType;

		return false;
	}
}