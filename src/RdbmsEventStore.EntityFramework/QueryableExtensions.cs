using System;
using System.Linq;

namespace RdbmsEventStore.EntityFramework
{
    internal static class QueryableExtensions
    {
        internal static IQueryable<T> Apply<T>(this IQueryable<T> source, Func<IQueryable<T>, IQueryable<T>> projection) => projection(source);
    }
}