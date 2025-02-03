// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace TestHelper.Statistics.Comparers
{
    /// <summary>
    /// Compare two <c>Bin</c> by <c>Min</c>, <c>Max</c>, and <c>Frequency</c>.
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

            var frequency = x.Frequency.CompareTo(y.Frequency);
            if (frequency != 0)
            {
                Log($"Frequency={x.Frequency}", $"Frequency={y.Frequency}");
            }

            return frequency;
        }

        private static void Log(string expected, string actual)
        {
            Debug.Log($"  Expected: {expected}\n  But was:  {actual}");
        }
    }
}
