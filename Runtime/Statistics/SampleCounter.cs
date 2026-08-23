// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections.Generic;

namespace TestHelper.Statistics
{
    /// <summary>
    /// Counts samples held as <see cref="IEnumerable{T}"/>.
    /// </summary>
    internal static class SampleCounter
    {
        /// <summary>
        /// Returns the number of samples.
        /// </summary>
        /// <remarks>
        /// Not <c>Enumerable.Count</c>: <c>System.Linq</c> is banned in this project. The
        /// <see cref="ICollection{T}"/> branch keeps the O(1) path that <c>Enumerable.Count</c> took for
        /// arrays and lists; sample spaces hold millions of samples, so walking them here would double the
        /// work of the caller that walks them again.
        /// </remarks>
        internal static ulong Count<T>(IEnumerable<T> samples)
        {
            if (samples is ICollection<T> collection)
            {
                return (ulong)collection.Count;
            }

            var count = 0UL;
            foreach (var sample in samples)
            {
                count++;
            }

            return count;
        }
    }
}
