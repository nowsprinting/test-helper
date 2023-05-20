// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;

namespace TestHelper.Random
{
    /// <summary>
    /// Random number generator interface like a <c>System.Random</c> class.
    /// </summary>
    /// <seealso cref="System.Random"/>
    public interface IRandom
    {
        /// <summary>
        /// Returns a non-negative random integer.
        /// </summary>
        /// <returns>A 32-bit signed integer greater than or equal to zero and less than <c>int.MaxValue</c>.</returns>
        int Next();

        /// <summary>
        /// Returns a non-negative random integer that is less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">The exclusive upper bound of the random number to be generated. maxValue must be greater than or equal to zero.</param>
        /// <returns>A 32-bit signed integer greater than or equal to zero, and less than maxValue; that is, the range of return values ordinarily includes zero but not maxValue. However, if maxValue equals zero, maxValue is returned.</returns>
        int Next(int maxValue);

        /// <summary>
        /// Returns a random integer that is within a specified range.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
        /// <returns>A 32-bit signed integer greater than or equal to minValue and less than maxValue; that is, the range of return values includes minValue but not maxValue. If minValue equals maxValue, minValue is returned.</returns>
        int Next(int minValue, int maxValue);

        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// <param name="buffer">An array of bytes to contain random numbers.</param>
        void NextBytes(byte[] buffer);

#if UNITY_2021_2_OR_NEWER
        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// <param name="buffer">An array of bytes to contain random numbers.</param>
        void NextBytes(Span<byte> buffer);
#endif

        /// <summary>
        /// Returns a random floating-point number between 0.0 and 1.0.
        /// </summary>
        /// <returns>A double-precision floating point number greater than or equal to 0.0, and less than 1.0.</returns>
        double NextDouble();
    }
}
