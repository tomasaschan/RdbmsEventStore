using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Appends a continuation to the task, which iterates the resulting <see cref="IEnumerable{T}"/> into a <see cref="List{T}"/>.
        /// </summary>
        public static Task<List<T>> ToList<T>(this Task<IEnumerable<T>> task)
            => task.ContinueWith(t => t.Result.ToList());

        /// <summary>
        /// Appends a continuation to the task, which iterates the resulting <see cref="IEnumerable{T}"/> into a <see cref="List{T}"/>
        /// and returns it as an <see cref="IReadOnlyCollection{T}"/>.
        /// </summary>
        public static Task<IReadOnlyCollection<T>> ToReadOnlyCollection<T>(this Task<IEnumerable<T>> task)
            => task.ContinueWith(t => t.Result.ToList() as IReadOnlyCollection<T>);
    }
}