// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Text;

namespace TestHelper.Statistics
{
    /// <summary>
    /// Sample space; return of <c>Experiment.Run()</c>.
    /// </summary>
    public struct SampleSpace<T> where T : IComparable
    {
        /// <summary>
        /// Samples.
        /// </summary>
        public T[] Samples { get; }

        /// <summary>
        /// Returns min value of the samples.
        /// </summary>
        public T Min { get; private set; }

        /// <summary>
        /// Returns max value of the samples.
        /// </summary>
        public T Max { get; private set; }

        private int _trailIndex;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trailCount"></param>
        public SampleSpace(uint trailCount)
        {
            Samples = new T[trailCount];
            Min = default;
            Max = default;
            _trailIndex = 0;
        }

        internal void Add(T value)
        {
            Samples[_trailIndex++] = value;

            if (_trailIndex == 1)
            {
                Min = value;
                Max = value;
                return;
            }

            if (Min.CompareTo(value) > 0)
            {
                Min = value;
            }

            if (Max.CompareTo(value) < 0)
            {
                Max = value;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (_trailIndex == 0)
            {
                return "{}";
            }

            var builder = new StringBuilder("{");
            builder.Append(Samples[0]);
            for (var i = 1; i < _trailIndex; i++)
            {
                builder.Append(",");
                builder.Append(Samples[i]);
            }

            return builder.Append("}").ToString();
        }
    }
}
