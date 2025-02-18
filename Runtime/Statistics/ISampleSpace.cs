// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;

namespace TestHelper.Statistics
{
    /// <summary>
    /// Sample space interface; return of <c>Experiment.Run</c>.
    /// </summary>
    public interface ISampleSpace<T> where T : IComparable
    {
        /// <summary>
        /// Samples.
        /// </summary>
        IEnumerable<T> Samples { get; }

        /// <summary>
        /// Returns min value of the samples.
        /// </summary>
        T Min { get; }

        /// <summary>
        /// Returns max value of the samples.
        /// </summary>
        T Max { get; }
    }
}
