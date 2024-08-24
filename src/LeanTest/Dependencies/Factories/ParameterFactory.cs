using LeanTest.Dependencies.Configuration;

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace LeanTest.Dependencies.Factories;

/// <inheritdoc />
internal readonly partial record struct ParameterFactory : IParameterFactory
{
	internal static readonly IParameterFactory Instance = new ParameterFactory();

	public TParam Is<TParam>() => default!;
	public TParam Matches<TParam>(Expression<Func<TParam, bool>> match)
	{
		// We only use this to configure the parameter
		_ = match;
		return Is<TParam>();
	}

	// TODO see if this still works
	public TParam IsReference<TParam>() => Ref<TParam>.IsAny;

	/// <summary>
	/// Contains matchers for <see langword="ref"/> (C#) / of type <typeparamref name="TValue"/>.
	/// </summary>
	/// <typeparam name="TValue">The parameter type.</typeparam>
	internal static class Ref<TValue>
	{
		/// <summary>
		/// Matches any value that is assignment-compatible with type <typeparamref name="TValue"/>.
		/// </summary>
		internal readonly static TValue IsAny = default!;
	}
}