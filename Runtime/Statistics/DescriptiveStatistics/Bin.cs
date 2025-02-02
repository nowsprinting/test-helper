// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;

namespace TestHelper.Statistics.DescriptiveStatistics
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
        public T Min { get; private set; }

        /// <summary>
        /// Maximum value (exclusive) of this bin range.
        /// </summary>
        public T Max { get; private set; }

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
        /// <param name="minInclusive">Minimum value of this bin range</param>
        /// <param name="maxExclusive">Maximum value (exclusive) of this bin range</param>
        public Bin(T minInclusive, T maxExclusive)
        {
            Min = minInclusive;
            Max = maxExclusive;
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

            return Min.CompareTo(value) <= 0 && Max.CompareTo(value) > 0;
        }
    }
}
