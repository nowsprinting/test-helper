// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEngine.Assertions;

namespace TestHelper.Statistics.RandomGenerators
{
    /// <summary>
    /// Linear Congruential Generator (LCG).
    /// </summary>
    public class Lcg
    {
        private int _seed;
        private readonly int _a;
        private readonly int _b;
        private readonly long _m;

        public Lcg(int seed, int a = 1103515245, int b = 12345, long m = 4294967296)
        {
            Assert.IsTrue(m > a);
            Assert.IsTrue(m > b);
            Assert.IsTrue(a > 0);
            Assert.IsTrue(b >= 0);

            this._seed = seed;
            this._a = a;
            this._b = b;
            this._m = m;
        }

        public int Next()
        {
            _seed = (int)((_a * _seed + _b) % _m);
            return _seed;
        }
    }
}
