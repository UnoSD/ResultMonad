﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Result;

namespace EnumerableResult
{
    [DebuggerStepThrough]
    static class EnumerableResultExtensions
    {
        static IResult<IEnumerable<T>> AllOrNothing<T>
        (this IEnumerable<IResult<T>> source) =>
            source.Aggregate
            (
                 Enumerable.Empty<T>()
                           .ToResult(),
                 (left, right) =>
                     left is IFailureResult<IEnumerable<T>> failure ?
                         $"{failure.Error}\n{right.Match(_ => string.Empty, error => error)}"
                             .ToFailureResult<IEnumerable<T>>() :
                     left is ISuccessResult<IEnumerable<T>> success ?
                         right is ISuccessResult<T> s ? success.Result
                                                               .Cons(s.Result)
                                                               .ToResult() :
                         right is IFailureResult<T> f ? f.Error
                                                         .ToFailureResult<IEnumerable<T>>() :
                         throw new ArgumentNullException(nameof(right)) :
                         throw new ArgumentNullException(nameof(left))
            );

        // This call is ambiguous with SelectMany<IEnumerable<T>, T>
        // the query syntax will prefer silently the other. This
        // needs to be into another namespace.
        internal static IResult<IEnumerable<TResult>> SelectMany<T, TResult>
        (
            this IResult<IEnumerable<T>> source, 
            Func<T, IResult<TResult>> func
        ) => source.BindMany(func);

        // We enumerate the enumerable two times, one for the BindMany
        // and one for the Zip, should really be an IReadOnlyCollection<T>
        // or we should ToList it.
        internal static IResult<IEnumerable<TOutput>> SelectMany<T, TResult, TOutput>
        (
            this IResult<IEnumerable<T>> source, 
            Func<T, IResult<TResult>> func,
            Func<T, TResult, TOutput> projection
        ) => source.BindMany(func)
                   .Bind(result =>
                             ((ISuccessResult<IEnumerable<T>>)source)
                                 .Result
                                 .Zip(result, projection)
                                 .ToResult());

        internal static IResult<IEnumerable<TResult>> BindMany<T, TResult>
        (
            this IResult<IEnumerable<T>> source, 
            Func<T, IResult<TResult>> func
        ) => source.Match(success => success.Select(func).AllOrNothing(),
                          failure => failure.ToFailureResult<TResult[]>());
    }
}