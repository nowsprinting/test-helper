// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace TestHelper.Statistics
{
    /// <summary>
    /// Sample space by Array; return of <c>Experiment.Run(int)</c>.
    /// </summary>
    public struct ArraySampleSpace<T> : ISampleSpace<T> where T : IComparable
    {
        /// <summary>
        /// Samples.
        /// </summary>
        public IEnumerable<T> Samples
        {
            get
            {
                return _samples;
            }
        }

        /// <summary>
        /// Returns min value of the samples.
        /// </summary>
        public T Min { get; private set; }

        /// <summary>
        /// Returns max value of the samples.
        /// </summary>
        public T Max { get; private set; }

        private readonly T[] _samples;
        private int _trialIndex;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trialCount"></param>
        public ArraySampleSpace(int trialCount)
        {
            _samples = new T[trialCount];
            _trialIndex = 0;
            Min = default;
            Max = default;
        }

        internal void Add(T value)
        {
            _samples[_trialIndex++] = value;

            if (_trialIndex == 1)
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
            if (_trialIndex == 0)
            {
                return "{}";
            }

            var builder = new StringBuilder("{");
            builder.Append(_samples[0]);
            for (var i = 1; i < _trialIndex; i++)
            {
                builder.Append(",");
                builder.Append(_samples[i]);
            }

            return builder.Append("}").ToString();
        }
    }
}
