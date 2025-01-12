// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TestHelper.Statistics.Histograms;
using UnityEngine;

namespace TestHelper.Statistics.Comparers
{
    /// <summary>
    /// Compare two <c>Bin</c> by <c>Min</c>, <c>Max</c>, and <c>Count</c>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BinCountComparer<T> : IComparer<Bin<T>> where T : IComparable
    {
        /// <inheritdoc/>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public int Compare(Bin<T> x, Bin<T> y)
        {
            var min = x.Min.CompareTo(y.Min);
            if (min != 0)
            {
                Log($"Min={x.Min}", $"Min={y.Min}");
                return min;
            }

            var max = x.Max.CompareTo(y.Max);
            if (max != 0)
            {
                Log($"Max={x.Max}", $"Max={y.Max}");
                return max;
            }

            var count = x.Count.CompareTo(y.Count);
            if (count != 0)
            {
                Log($"Count={x.Count}", $"Count={y.Count}");
            }

            return count;
        }

        private static void Log(string expected, string actual)
        {
            Debug.Log($"  Expected: {expected}\n  But was:  {actual}");
        }
    }
}
