// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace TestHelper.Statistics.Histograms
{
    public class Histogram<T> where T : IComparable
    {
        /// <summary>
        /// Bins of this histogram.
        /// </summary>
        public SortedList<T, Bin<T>> Bins { get; internal set; }

        /// <summary>
        /// Peak value of this histogram.
        /// This property is set in the <c>Calculate()</c> method.
        /// </summary>
        public uint Peak { get; private set; }

        /// <summary>
        /// Valley value of this histogram.
        /// This property is set in the <c>Calculate()</c> method.
        /// </summary>
        public uint Valley { get; private set; }

        /// <summary>
        /// Median value of this histogram.
        /// This property is set in the <c>Calculate()</c> method.
        /// </summary>
        public double Median { get; private set; }

        /// <summary>
        /// Mean (average) value of this histogram.
        /// This property is set in the <c>Calculate()</c> method.
        /// </summary>
        public double Mean { get; private set; }

        /// <summary>
        /// Constructor.
        /// Can only be used with numeric types.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="binSize"></param>
        public Histogram(T min, T max, double binSize = 1d)
        {
            var minValue = ToDouble(min);
            var maxValue = ToDouble(max);
            var distance = maxValue - minValue;
            var binCount = (int)Math.Ceiling(distance / binSize);

            Bins = new SortedList<T, Bin<T>>(binCount);
            for (var i = 0; i < binCount; i++)
            {
                var bin = new Bin<T>(ToT(minValue), ToT(minValue + binSize));
                Bins.Add(ToT(minValue), bin);
                minValue += binSize;
            }
        }

        /// <summary>
        /// Constructor without initial bins.
        /// </summary>
        public Histogram()
        {
            Bins = new SortedList<T, Bin<T>>();
        }

        /// <summary>
        /// Plot samples into bins.
        /// </summary>
        /// <param name="samples">Input samples</param>
        public void Plot(IEnumerable<T> samples)
        {
            foreach (var current in samples)
            {
                var bin = FindBin(current);
                if (bin == null)
                {
                    bin = new Bin<T>(current);
                    Bins.Add(current, bin);
                }

                bin.Count++;
            }

            Calculate();
        }

        private Bin<T> FindBin(T value)
        {
            var left = 0;
            var right = Bins.Count - 1;

            while (left <= right)
            {
                var mid = (left + right) / 2;
                var bin = Bins.Values[mid];

                if (bin.IsInRange(value))
                {
                    return bin;
                }

                if (value.CompareTo(bin.Min) < 0)
                {
                    right = mid - 1;
                }
                else
                {
                    left = mid + 1;
                }
            }

            return null;
        }

        /// <summary>
        /// Calculate Peak, Valley, Median, and Mean.
        /// </summary>
        /// <remarks>
        /// Called in <c>Plot()</c> method.
        /// </remarks>
        internal void Calculate()
        {
            var counts = new List<uint>();
            Mean = 0;

            foreach (var bin in Bins.Values)
            {
                counts.Add(bin.Count);
                Mean += (double)bin.Count / Bins.Count;
            }

            counts.Sort();
            Peak = counts[^1];
            Valley = counts[0];
            Median = counts.Count % 2 == 0
                ? (double)(counts[counts.Count / 2 - 1] + counts[counts.Count / 2]) / 2
                : counts[counts.Count / 2];
        }

        /// <summary>
        /// Draw character-based histogram.
        /// </summary>
        /// <returns></returns>
        public string DrawHistogramAscii()
        {
            var builder = new StringBuilder();
            var blockHeight = (double)(Peak - Valley) / 7;

            foreach (var bin in Bins.Values)
            {
                if (bin.Count > 0)
                {
                    var block = blockHeight > 0
                        ? 0x2581 + (int)((bin.Count - Valley) / blockHeight)
                        : 0x2588; // full block
                    builder.Append(char.ConvertFromUtf32(block));
                }
                else
                {
                    builder.Append((char)0x20); // space
                }
            }

            return builder.ToString();
        }

        private static double ToDouble(T value)
        {
            return (double)Convert.ChangeType(value, typeof(double));
        }

        private static T ToT(double value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
