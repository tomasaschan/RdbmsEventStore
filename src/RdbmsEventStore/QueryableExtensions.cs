// ReSharper disable once CheckNamespace
namespace System.Linq
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Apply<T>(this IQueryable<T> source, Func<IQueryable<T>, IQueryable<T>> projection) => projection(source);
    }
}