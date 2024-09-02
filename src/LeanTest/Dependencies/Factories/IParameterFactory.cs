using System.Linq.Expressions;

namespace LeanTest.Dependencies.Factories;

/// <summary>
/// TODOC
/// </summary>
/// <internalRemark>
/// This isn't really a factory, however, "IParameterExpressionRepresentationProvider" sounds strange.
/// And, the other providing classes are called IXFactory too.
/// </internalRemark>
public interface IParameterFactory
{
	TParam Is<TParam>() where TParam : notnull;
	object? IsAny();

	// TODO Analyzer for preferring T
	object? IsNull();
	// TODO Analyzer to make sure nullable is returned
	TParam? IsNull<TParam>();
	// TODO Analyzer for preferring T
	object? NotNull();
	// TODO Analyzer for matching types
	TParam? NotNull<TParam>();

	TParam Matches<TParam>(Expression<Func<TParam, bool>> match);

	TParam IsReference<TParam>();
}