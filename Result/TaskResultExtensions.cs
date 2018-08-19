using System;
using System.Threading.Tasks;
using Result;

namespace Monad.TaskResult
{
    static class TaskResultExtensions
    {
        internal static Task<IResult<TResult>> SelectMany<T, TResult>
        (
            this Task<T> source,
            Func<T, IResult<TResult>> func
        ) => source.Map(func);

        internal static Task<IResult<TOutput>> SelectMany<T, TResult, TOutput>
        (
            this Task<T> source,
            Func<T, IResult<TResult>> func,
            Func<T, TResult, TOutput> project
        ) => source.Map(tres => func(tres).Map(fres => project(tres, fres)));

        internal static Task<IResult<TResult>> Select<T, TResult>
        (
            this IResult<T> source,
            Func<T, Task<TResult>> func
        ) => source.MapAsync(func);

        internal static Task<IResult<TResult>> MapAsync<T, TResult>
        (
            this IResult<T> source,
            Func<T, Task<TResult>> func
        ) => source.Match(result => func(result).Map(o => o.ToResult()),
                          error => error.ToFailureResult<TResult>()
                                        .ToTask());
    }
}
