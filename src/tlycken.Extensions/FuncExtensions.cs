// ReSharper disable once CheckNamespace
namespace System
{
    public static class FuncExtensions
    {
        /// <summary>
        /// Fluently applies the function <paramref name="projection"/> to the argument <paramref name="source"/>.
        /// </summary>
        /// <remarks>My main use case for this as an abstraction for queryables, taking a
        /// <code>Func{IQueryable{S}, IQueryable{T}}</code> as an argument and applying it
        /// in a longer LINQ method chain.</remarks>
        /// <typeparam name="S">The element type of the queryable sequence</typeparam>
        /// <typeparam name="T">The element type of the result</typeparam>
        /// <param name="source">The source sequence</param>
        /// <param name="projection">A projection to apply to the <paramref name="source"/> sequence</param>
        /// <returns>projection(source)</returns>
        public static T Apply<S, T>(this S source, Func<S, T> projection) => projection(source);
    }
}