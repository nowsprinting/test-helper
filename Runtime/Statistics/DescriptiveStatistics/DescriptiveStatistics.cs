// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestHelper.Statistics.DescriptiveStatistics
{
    /// <summary>
    /// Calculate statistical summaries and plotting a histogram.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DescriptiveStatistics<T> where T : IComparable
    {
        /// <summary>
        /// Bins of this histogram.
        /// </summary>
        public SortedList<T, Bin<T>> Bins { get; internal set; }

        /// <summary>
        /// Peak frequency of this histogram.
        /// This property is set in the <c>Calculate()</c> method.
        /// </summary>
        public uint Peak { get; private set; }

        /// <summary>
        /// Valley frequency of this histogram.
        /// This property is set in the <c>Calculate()</c> method.
        /// </summary>
        public uint Valley { get; private set; }

        /// <summary>
        /// Median frequency of this histogram.
        /// This property is set in the <c>Calculate()</c> method.
        /// </summary>
        public double Median { get; private set; }

        /// <summary>
        /// Mean (average) frequency of this histogram.
        /// This property is set in the <c>Calculate()</c> method.
        /// </summary>
        public double Mean { get; private set; }

        // only used in summary
        private uint _sampleSize;
        private T _sampleMax;
        private T _sampleMin;

        // lower bound for character-based histogram
        private uint _valleyGraterThanZero;

        /// <summary>
        /// Constructor that creates initial bins.
        /// Can only be used with numeric type.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="binSize"></param>
        public DescriptiveStatistics(T min, T max, double binSize = 1d)
        {
            var minValue = Convert.ToDouble(min);
            var maxValue = Convert.ToDouble(max);
            var distance = maxValue - minValue;
            var binCount = (int)Math.Ceiling(distance / binSize);

            Bins = new SortedList<T, Bin<T>>(binCount);
            for (var i = 0; i < binCount; i++)
            {
                var bin = new Bin<T>(ConvertToT(minValue), ConvertToT(minValue + binSize));
                Bins.Add(ConvertToT(minValue), bin);
                minValue += binSize;
            }
        }

        /// <summary>
        /// Constructor without initial bins.
        /// </summary>
        public DescriptiveStatistics()
        {
            Bins = new SortedList<T, Bin<T>>();
        }

        /// <summary>
        /// Plot samples into bins.
        /// </summary>
        /// <param name="samples">Input samples</param>
        /// <param name="size">Sample size; only used in summary</param>
        /// <param name="min">Minimum value in samples; only used in summary</param>
        /// <param name="max">Maximum value in samples; only used in summary</param>
        public void Calculate(IEnumerable<T> samples, uint size = 0, T min = default, T max = default)
        {
            _sampleSize = size;
            _sampleMax = max;
            _sampleMin = min;

            foreach (var current in samples)
            {
                var bin = FindBin(current);
                if (bin == null)
                {
                    bin = new Bin<T>(current);
                    Bins.Add(current, bin);
                }

                bin.Frequency++;
            }

            Calculate();
        }

        /// <summary>
        /// Plot samples into bins.
        /// </summary>
        /// <param name="sampleSpace">Input sample space</param>
        public void Calculate(SampleSpace<T> sampleSpace)
        {
            Calculate(sampleSpace.Samples, (uint)sampleSpace.Samples.Length, sampleSpace.Min, sampleSpace.Max);
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

        internal void Calculate()
        {
            var frequencies = new List<uint>();
            Mean = 0;

            foreach (var bin in Bins.Values)
            {
                frequencies.Add(bin.Frequency);
                Mean += (double)bin.Frequency / Bins.Count;
            }

            frequencies.Sort();
            Peak = frequencies[frequencies.Count - 1];
            Valley = frequencies[0];
            Median = frequencies.Count % 2 == 0
                ? (double)(frequencies[frequencies.Count / 2 - 1] + frequencies[frequencies.Count / 2]) / 2
                : frequencies[frequencies.Count / 2];

            foreach (var frequency in frequencies.Where(frequency => frequency > 0))
            {
                _valleyGraterThanZero = frequency;
                break;
            }
        }

        internal string DrawHistogramAscii()
        {
            var builder = new StringBuilder();
            var blockHeight = (double)(Peak - _valleyGraterThanZero) / 7;

            foreach (var bin in Bins.Values)
            {
                if (bin.Frequency > 0)
                {
                    var block = blockHeight > 0
                        ? 0x2581 + (int)((bin.Frequency - _valleyGraterThanZero) / blockHeight)
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

        /// <summary>
        /// Returns statistical summaries and a character-based histogram.
        /// </summary>
        /// <returns></returns>
        public string GetSummary()
        {
            var builder = new StringBuilder("---\nExperimental and Statistical Summary:\n");
            builder.AppendLine($"  Sample size: {_sampleSize:N0}");
            builder.AppendLine($"  Maximum: {_sampleMax}"); // No format, may not be a numeric type.
            builder.AppendLine($"  Minimum: {_sampleMin}"); // No format, may not be a numeric type.
            builder.AppendLine($"  Peak frequency: {Peak:N0}");
            builder.AppendLine($"  Valley frequency: {Valley:N0}");
            builder.AppendLine($"  Median: {Median:N0}");
            builder.AppendLine($"  Mean: {Mean:N2}");
            builder.AppendLine($"  Histogram: {DrawHistogramAscii()}");
            builder.AppendLine("  (Each bar represents the frequency of values in equally spaced bins.)");
            return builder.ToString();
        }

        private static T ConvertToT(double value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
