using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;

namespace LeanTest.Dependencies.Providers;

// TODO Analyzer for preferring T
[Browsable(false), DebuggerNonUserCode]
public sealed class ParameterExpressionProvider<TParam> : ITypedParameterExpressionProvider<TParam>
{
	[Browsable(false), DebuggerNonUserCode, DebuggerHidden]
	internal ParameterExpressionProvider() => throw new NotSupportedException();

	public TParam AnyValue = default!;
	// TODO Analyzer to make sure nullable is returned
	public TParam? Null;
	// TODO Analyzer to make sure nullable is returned
	public TParam? NotNull;
	public TParam Match(Expression<Func<TParam, bool>> match)
	{
		_ = match;
		return default!;
	}
}


[Browsable(false), DebuggerNonUserCode]
public sealed class ParameterExpressionProvider : IDynamicParameterExpressionProvider
{
	[Browsable(false), DebuggerNonUserCode, DebuggerHidden]
	internal ParameterExpressionProvider() => throw new NotSupportedException();

	public DynamicObject Any = default!;
	// TODO Analyzer to make sure nullable is returned
	public DynamicObject? Null;
	// TODO Analyzer to make sure nullable is returned
	public DynamicObject? NotNull;
	public DynamicObject Matches(Expression<Func<DynamicObject, bool>> match)
	{
		_ = match;
		return default!;
	}
}