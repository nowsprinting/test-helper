// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEngine;

namespace TestHelper.Random
{
    /// <summary>
    /// Some useful methods for testing.
    /// </summary>
    public static class RandomExtensions
    {
        private const int Digits = 3;

        /// <summary>
        /// Return a random screen position.
        /// </summary>
        /// <returns>A random screen position.</returns>
        public static Vector2Int NextScreenPosition(this IRandom random)
        {
            var width = random.Next(Screen.width);
            var height = random.Next(Screen.height);
            return new Vector2Int(width, height);
        }

        /// <summary>
        /// Return a random normalized <c>Vector2</c>.
        /// </summary>
        /// <returns>A random normalized <c>Vector2</c>.</returns>
        public static Vector2 NextNormalizedVector2(this IRandom random)
        {
            var denominator = Mathf.Pow(10, Digits);
            var max = (int)denominator;
            var min = -max;

            var x = random.Next(min, max) / denominator;
            var y = random.Next(min, max) / denominator;
            return new Vector2(x, y).normalized;
        }

        /// <summary>
        /// Return a random normalized <c>Vector3</c>.
        /// </summary>
        /// <returns>A random normalized <c>Vector3</c>.</returns>
        public static Vector3 NextNormalizedVector3(this IRandom random)
        {
            var denominator = Mathf.Pow(10, Digits);
            var max = (int)denominator;
            var min = -max;

            var x = random.Next(min, max) / denominator;
            var y = random.Next(min, max) / denominator;
            var z = random.Next(min, max) / denominator;
            return new Vector3(x, y, z).normalized;
        }
    }
}
