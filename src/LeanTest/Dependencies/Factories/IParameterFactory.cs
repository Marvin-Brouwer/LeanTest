using System.Linq.Expressions;

namespace LeanTest.Dependencies.Factories;

/// <summary>
/// TODOC
/// </summary>
/// <internalRemark>
/// This isn't really a factory, however, "IParameterEpxressionRepresentationProvider" sounds strange.
/// And, the other providing classes are called IXFactory too.
/// </internalRemark>
// TODO steal more from Moq
public interface IParameterFactory
{
	TParam Is<TParam>();
	TParam Matches<TParam>(Expression<Func<TParam, bool>> match);

	TParam IsReference<TParam>();
}