// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Text;
using TestHelper.Statistics.Histograms;
using UnityEngine;

namespace TestHelper.Statistics
{
    public static class Experiment
    {
        /// <summary>
        /// Running experiments of pseudo-random number generator (PRNG).
        /// It outputs statistical summaries and a character-based histogram to the console.
        /// </summary>
        /// <param name="method">Method returns random value</param>
        /// <param name="trailCount">Trail count</param>
        /// <param name="histogramBinSize">Bin size (width) of output histogram</param>
        /// <typeparam name="T">Type of random value</typeparam>
        /// <returns>Sample space</returns>
        public static SampleSpace<T> Run<T>(Func<T> method, uint trailCount, double histogramBinSize = 1d)
            where T : IComparable
        {
            var sampleSpace = new SampleSpace<T>(trailCount);
            for (var i = 0; i < trailCount; i++)
            {
                sampleSpace.Add(method.Invoke());
            }

            LogHistogram(sampleSpace);

            return sampleSpace;
        }

        private static void LogHistogram<T>(SampleSpace<T> sampleSpace) where T : IComparable
        {
            var histogram = new Histogram<T>(); // TODO: specify min, max. and calculate binSize.
            histogram.Plot(sampleSpace.Samples);

            var builder = new StringBuilder("---\nExperimental and Statistical Summary:\n");
            builder.AppendLine($"  Sample size: {sampleSpace.Samples.Length:N0}");
            builder.AppendLine($"  Maximum: {sampleSpace.Max}");
            builder.AppendLine($"  Minimum: {sampleSpace.Min}");
            builder.AppendLine($"  Peak frequency: {histogram.Peak:N0}");
            builder.AppendLine($"  Valley frequency: {histogram.Valley:N0}");
            builder.AppendLine($"  Median: {histogram.Median:N0}");
            builder.AppendLine($"  Mean: {histogram.Mean:N2}");
            builder.AppendLine($"  Histogram: {histogram.DrawHistogramAscii()}");
            builder.AppendLine("  (Each bar represents the frequency of values in equally spaced bins.)");
            Debug.Log(builder.ToString());
        }
    }
}
