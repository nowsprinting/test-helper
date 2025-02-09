// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;

namespace TestHelper.Statistics
{
    /// <summary>
    /// Bin of histogram.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Bin<T> where T : IComparable
    {
        /// <summary>
        /// Minimum value of this bin range.
        /// </summary>
        public T Min { get; }

        /// <summary>
        /// Maximum value (exclusive) of this bin range.
        /// By default, the maximum value is excluded.
        /// </summary>
        public T Max { get; }

        /// <summary>
        /// Includes the maximum value if true
        /// </summary>
        private bool IsMaxInclusive { get; }

        /// <summary>
        /// Frequency of this bin.
        /// </summary>
        public uint Frequency { get; set; }

        /// <summary>
        /// Label of this bin.
        /// </summary>
        public string Label
        {
            get
            {
                return Min.ToString();
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="min">Minimum value of this bin range, it always inclusive.</param>
        /// <param name="max">Maximum value of this bin range</param>
        /// <param name="maxInclusive">Includes the maximum value if true</param>
        public Bin(T min, T max, bool maxInclusive = false)
        {
            Min = min;
            Max = max;
            IsMaxInclusive = maxInclusive;
            Frequency = 0;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">Value of this bin range</param>
        public Bin(T value) : this(value, value) { }

        /// <summary>
        /// Returns value is in this bin range.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>true if value is in this bin range</returns>
        public bool IsInRange(T value)
        {
            if (Min.CompareTo(Max) == 0)
            {
                return Min.CompareTo(value) == 0;
            }

            if (IsMaxInclusive)
            {
                return Min.CompareTo(value) <= 0 && Max.CompareTo(value) >= 0;
            }
            else
            {
                return Min.CompareTo(value) <= 0 && Max.CompareTo(value) > 0;
            }
        }
    }
}
