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
        private readonly int _c;
        private readonly long _m;

        public Lcg(int seed, int a = 1103515245, int c = 12345, long m = 1L << 32)
        {
            Assert.IsTrue(m > a);
            Assert.IsTrue(m > c);
            Assert.IsTrue(a > 0);
            Assert.IsTrue(c >= 0);

            this._seed = seed;
            this._a = a;
            this._c = c;
            this._m = m;
        }

        public int Next()
        {
            _seed = (int)((_a * _seed + _c) % _m);
            return _seed;
        }
    }
}
