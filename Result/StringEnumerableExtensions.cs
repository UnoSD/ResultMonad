using System.Diagnostics;

namespace System.Collections.Generic
{
    [DebuggerStepThrough]
    static class StringEnumerableExtensions
    {
        internal static string Join(this IEnumerable<string> source, string separator) =>
            string.Join(separator, source);
    }
}