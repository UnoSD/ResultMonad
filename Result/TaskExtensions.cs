using System.Collections.Generic;
using System.Diagnostics;

namespace System.Threading.Tasks
{
    [DebuggerStepThrough]
    static class TaskExtensions
    {
        internal static Task<T> ToTask<T>(this T source) =>
            Task.FromResult(source);

        internal static async Task<TResult> Bind<T, TResult>
        (
            this Task<T> source,
            Func<T, TResult> func
        )
        {
            var result =
                await source.ConfigureAwait(false);

            return func(result);
        }

        internal static Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> tasks) => 
            Task.WhenAll(tasks);

        internal static Task WhenAll(this IEnumerable<Task> tasks) => 
            Task.WhenAll(tasks);

        internal static async Task WhenAllSequential(this IEnumerable<Task> tasks)
        {
            foreach (var task in tasks)
                await task.ConfigureAwait(false);
        }

        // TODO: Refactor list out of this
        internal static async Task<IReadOnlyCollection<T>> WhenAllSequential<T>
        (this IEnumerable<Task<T>> tasks)
        {
            var results = 
                new List<T>();

            foreach (var task in tasks)
                results.Add(await task.ConfigureAwait(false));

            return results;
        }

        internal static async Task<TResult> Map<T, TResult>
        (this Task<T> source, Func<T, TResult> func)
        {
            var result = 
                await source.ConfigureAwait(false);

            return func(result);
        }
    }
}