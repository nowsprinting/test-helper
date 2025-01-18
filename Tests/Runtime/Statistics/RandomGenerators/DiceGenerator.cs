// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEngine;

namespace TestHelper.Statistics.RandomGenerators
{
    public static class DiceGenerator
    {
        private static int Roll(int sides)
        {
            return Random.Range(1, sides + 1);
        }

        public static int Roll(int dice, int sides)
        {
            var sum = 0;
            for (var i = 0; i < dice; i++)
            {
                sum += Roll(sides);
            }

            return sum;
        }
    }
}
