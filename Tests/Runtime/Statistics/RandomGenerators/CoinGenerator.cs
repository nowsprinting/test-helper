// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEngine;

namespace TestHelper.Statistics.RandomGenerators
{
    public enum Coin
    {
        Head,
        Tail
    }

    public static class CoinGenerator
    {
        public static Coin Toss()
        {
            return Random.value < 0.5f ? Coin.Head : Coin.Tail;
        }
    }
}
