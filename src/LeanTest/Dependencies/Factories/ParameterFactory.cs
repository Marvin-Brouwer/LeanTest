using System.Linq.Expressions;

namespace LeanTest.Dependencies.Factories
{
    // TODO store last match
    internal readonly record struct ParameterFactory : IParameterFactory
    {
        internal static readonly IParameterFactory Instance = new ParameterFactory();

        public TParam Is<TParam>()
        {
            throw new NotImplementedException();
        }

        public TParam Matches<TParam>(Expression<Func<TParam, bool>> match)
        {
            throw new NotImplementedException();
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
}