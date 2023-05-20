// Copyright (c) 2021-2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;

namespace TestHelper.Random
{
    /// <summary>
    /// Reference implementation of <c>IRandom</c>, wrapping <c>System.Random</c>.
    /// </summary>
    /// <seealso cref="System.Random"/>
    public class RandomWrapper : IRandom
    {
        private readonly System.Random _random;
        private readonly int _seed;

        /// <summary>
        /// Initializes a new instance of the <c>RandomWrapper</c> class,
        /// using the specified seed value.
        /// </summary>
        /// <param name="seed">A number used to calculate a starting value for the pseudo-random number sequence. If a negative number is specified, the absolute value of the number is used.</param>
        public RandomWrapper(int seed)
        {
            _random = new System.Random(seed);
            _seed = seed;
        }

        /// <summary>
        /// Initializes a new instance of the <c>RandomWrapper</c> class,
        /// using <c>Environment.TickCount</c> to seed value.
        /// </summary>
        public RandomWrapper() : this(Environment.TickCount) { }

        /// <inheritdoc />
        public virtual int Next()
        {
            return _random.Next();
        }

        /// <inheritdoc />
        public virtual int Next(int maxValue)
        {
            return _random.Next(maxValue);
        }

        /// <inheritdoc />
        public virtual int Next(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }

        /// <inheritdoc />
        public virtual void NextBytes(byte[] buffer)
        {
            _random.NextBytes(buffer);
        }

#if UNITY_2021_2_OR_NEWER
        /// <inheritdoc />
        public virtual void NextBytes(Span<byte> buffer)
        {
            _random.NextBytes(buffer);
        }
#endif

        /// <inheritdoc />
        public virtual double NextDouble()
        {
            return _random.NextDouble();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"wrapping System.Random, seed={_seed}";
        }
    }
}
