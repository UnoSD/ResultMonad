using System.Diagnostics;
using System.Linq;

namespace System.Collections.Generic
{
    [DebuggerStepThrough]
    static class EnumerableExtensions
    {
        internal static IEnumerable<T> Cons<T>(this IEnumerable<T> source, T item) =>
            new[] { item }.Concat(source);
    }
}