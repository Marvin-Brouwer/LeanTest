using System.Linq.Expressions;

namespace LeanTest.Dependencies.Factories;

// TODO steal more from Moq
public interface IParameterFactory
{
	TParam Is<TParam>();
	TParam Matches<TParam>(Expression<Func<TParam, bool>> match);

	TParam IsReference<TParam>();
}