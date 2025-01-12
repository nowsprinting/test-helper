// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;

namespace TestHelper.Statistics
{
    public static class Experiment
    {
        /// <summary>
        /// Run experiment of random value generation.
        /// Side effect: write a histogram.
        /// </summary>
        /// <param name="method">Method returns random value</param>
        /// <param name="trailCount">Trail count</param>
        /// <param name="histogramBinSize">Bin size (width) of output histogram. Default: auto calculation</param>
        /// <typeparam name="T">Type of random value</typeparam>
        /// <returns>Sample space</returns>
        public static SampleSpace<T> Run<T>(Func<T> method, uint trailCount, double histogramBinSize = 0)
            where T : IComparable
        {
            var sampleSpace = new SampleSpace<T>(trailCount);
            for (var i = 0; i < trailCount; i++)
            {
                sampleSpace.Add(method.Invoke());
            }

            // TODO: write histogram

            return sampleSpace;
        }
    }
}
