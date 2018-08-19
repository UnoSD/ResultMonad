using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Result;

namespace Test
{
    static class Extensions
    {
        static T Cast<T>(this object source) =>
            (T)source;

        internal static T Result<T>(this IResult<T> source) =>
            source.Cast<ISuccessResult<T>>().Result;
        
        internal static T Result<T>(this Task<IResult<T>> source) =>
            source.Result.Result();

        internal static T Result<T>(this Task<IResult<IEnumerable<T>>> source) =>
            source.Result.Result();

        internal static T Result<T>(this IResult<IEnumerable<T>> source) =>
            source.Result<IEnumerable<T>>().Single();
    }
}
