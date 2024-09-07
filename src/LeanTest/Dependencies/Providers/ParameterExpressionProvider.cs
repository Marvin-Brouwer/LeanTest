using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;

namespace LeanTest.Dependencies.Providers;

[Browsable(false), DebuggerNonUserCode]
public sealed class ParameterExpressionProvider<TParam> : ITypedParameterExpressionProvider<TParam>
{
	[Browsable(false), DebuggerNonUserCode, DebuggerHidden]
	internal ParameterExpressionProvider() =>
		throw ParameterExpressionProvider.ParameterExpressionInvokedException;

	public TParam AnyValue = default!;
	// TODO Analyzer to make sure nullable is returned
	public TParam? Null;
	// TODO Analyzer to make sure nullable is returned
	public TParam? NotNull;
	public TParam Match(Expression<Func<TParam, bool>> match) =>
		throw ParameterExpressionProvider.ParameterExpressionInvokedException;
}


[Browsable(false), DebuggerNonUserCode]
public sealed class ParameterExpressionProvider : IDynamicParameterExpressionProvider
{
	internal static readonly NotSupportedException ParameterExpressionInvokedException = new NotSupportedException(
		"The Parameter expression providers are only intended for expressions!"
	);

	[Browsable(false), DebuggerNonUserCode, DebuggerHidden]
	internal ParameterExpressionProvider() =>
		throw ParameterExpressionInvokedException;

	public DynamicObject Any = default!;
	// TODO Analyzer to make sure nullable is returned
	public DynamicObject? Null;
	// TODO Analyzer to make sure nullable is returned
	public DynamicObject? NotNull;
	public DynamicObject Matches(Expression<Func<object, bool>> match) =>
		throw ParameterExpressionInvokedException;
}