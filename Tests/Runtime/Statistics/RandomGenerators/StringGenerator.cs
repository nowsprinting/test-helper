// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEngine;

namespace TestHelper.Statistics.RandomGenerators
{
    public static class StringGenerator
    {
        public static string Generate(int length = 8)
        {
            var result = new char[length];
            for (var i = 0; i < length; i++)
            {
                result[i] = (char)Random.Range(32, 127); // ASCII codes from 32 (space) to 126 (~)
            }

            return new string(result);
        }
    }
}
