using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Result
{
    interface IResult<out T> { }

    interface ISuccessResult<out T> : IResult<T>
    {
        T Result { get; }
    }

    [DebuggerStepThrough]
    [DebuggerDisplay("Success: {" + nameof(Result) + "}")]
    class SuccessResult<T> : ISuccessResult<T>
    {
        internal SuccessResult(T result) => Result = result;

        public T Result { get; }
    }

    interface IFailureResult<out T> : IResult<T>
    {
        string Error { get; }
    }

    [DebuggerStepThrough]
    [DebuggerDisplay("Failure: {" + nameof(Error) + "}")]
    class FailureResult<T> : IFailureResult<T>
    {
        internal FailureResult(string error) => Error = error;

        public string Error { get; }
    }

    [DebuggerStepThrough]
    static class ResultExtensions
    {
        internal static IResult<T> ToResult<T>(this T result) =>
            new SuccessResult<T>(result);

        internal static IResult<T> ToFailureResult<T>(this string error) =>
            new FailureResult<T>(error);
    }

    [DebuggerStepThrough]
    static class SingleResultExtensions
    {
        internal static IResult<TResult> SelectMany<T, TResult>
        (
            this IResult<T> source,
            Func<T, IResult<TResult>> func
        ) => source.Bind(func);
        
        internal static TResult Match<T, TResult>
        (
            this IResult<T> source,
            Func<T, TResult> onSuccess,
            Func<string, TResult> onError
        ) =>
            source is IFailureResult<T> failure ? onError(failure.Error) :
            source is ISuccessResult<T> success ? onSuccess(success.Result) :
            throw new ArgumentNullException(nameof(source));

        internal static IResult<TOutput> SelectMany<T, TResult, TOutput>
        (
            this IResult<T> source,
            Func<T, IResult<TResult>> func,
            Func<T, TResult, TOutput> projection
        ) => source.Bind(func)
                   .Bind(result => projection(((ISuccessResult<T>)source).Result, result)
                                       .ToResult());

        internal static IResult<TResult> Bind<T, TResult>
        (
            this IResult<T> source,
            Func<T, IResult<TResult>> func
        ) => source.Match(func,
                          error => error.ToFailureResult<TResult>());
       
        internal static IResult<TResult> Select<T, TResult>
        (
            this IResult<T> source,
            Func<T, TResult> func
        ) => source.Map(func);

        internal static IResult<TResult> Map<T, TResult>
        (
            this IResult<T> source,
            Func<T, TResult> func
        ) => source.Match(result => func(result).ToResult(),
                          error => error.ToFailureResult<TResult>());

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

        internal static Task<IResult<TResult>> SelectMany<T, TResult>
        (
            this Task<IResult<T>> source,
            Func<T, IResult<TResult>> func
        ) => source.BindAsync(func);
        
        // The method with projection theoretically is not needed
        // when you don't need T and TResult to get to TOutput but
        // TResult IS TOutput like in the version without projection.
        // Unfortunately the compiler complains it can't use the
        // query syntax if you don't declare this as well.
        internal static Task<IResult<TOutput>> SelectMany<T, TResult, TOutput>
        (
            this Task<IResult<T>> source,
            Func<T, IResult<TResult>> func,
            Func<T, TResult, TOutput> projection
        ) => source.BindAsync(func)
                   .BindAsync(result => projection(((ISuccessResult<T>)source.Result).Result,
                                                   result)
                                            .ToResult());

        internal static Task<IResult<TResult>> BindAsync<T, TResult>
        (
            this Task<IResult<T>> source,
            Func<T, IResult<TResult>> func
        ) => source.Map(result => result.Bind(func));

        internal static Task<IResult<TResult>> SelectMany<T, TResult>
        (
            this IResult<T> source,
            Func<T, Task<IResult<TResult>>> func
        ) => source.BindAsync(func);

        internal static Task<IResult<TOutput>> SelectMany<T, TResult, TOutput>
        (
            this IResult<T> source,
            Func<T, Task<IResult<TResult>>> func,
            Func<T, TResult, TOutput> projection
        ) => source.BindAsync(func)
                   .BindAsync(result => projection(((ISuccessResult<T>)source).Result,
                                                   result)
                                            .ToResult());

        internal static Task<IResult<TResult>> BindAsync<T, TResult>
        (
            this IResult<T> source,
            Func<T, Task<IResult<TResult>>> func
        ) => source.Match(func,
                          error => error.ToFailureResult<TResult>()
                                        .ToTask());

        internal static Task<TResult> MatchAsync<T, TResult>
        (
            this Task<IResult<T>> source,
            Func<T, Task<TResult>> onSuccess,
            Func<string, TResult> onError
        ) => source.Bind(result => result.Match(onSuccess, 
                                                error => onError(error).ToTask()));
        
        internal static Task<TResult> MatchAsync<T, TResult>
        (
            this Task<IResult<T>> source,
            Func<T, TResult> onSuccess,
            Func<string, TResult> onError
        ) => source.Map(result => result.Match(onSuccess, onError));
    }
}