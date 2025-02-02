// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;

namespace TestHelper.Statistics
{
    public static class Experiment
    {
        /// <summary>
        /// Running experiments of pseudo-random number generator (PRNG).
        /// </summary>
        /// <param name="method">Method returns random value</param>
        /// <param name="trailCount">Trail count</param>
        /// <typeparam name="T">Type of random value</typeparam>
        /// <returns>Sample space</returns>
        public static ArraySampleSpace<T> Run<T>(Func<T> method, int trailCount)
            where T : IComparable
        {
            var sampleSpace = new ArraySampleSpace<T>(trailCount);
            for (var i = 0; i < trailCount; i++)
            {
                sampleSpace.Add(method.Invoke());
            }

            return sampleSpace;
        }
    }
}
