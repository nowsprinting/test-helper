// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;
using TestHelper.Statistics.Comparers;

namespace TestHelper.Statistics.Histograms
{
    [TestFixture]
    public class BinCountComparerTest
    {
        [Test]
        public void Compare_Success()
        {
            var expected = new Bin<int>(0, 10) { Count = 100 };
            var actual = new Bin<int>(0, 10) { Count = 100 };
            Assert.That(actual, Is.EqualTo(expected).Using(new BinCountComparer<int>()));
        }

        [TestCase(-1, 10, 100u)] // different min
        [TestCase(1, 10, 100u)] // different min
        [TestCase(0, 9, 100u)] // different max
        [TestCase(0, 11, 100u)] // different max
        [TestCase(0, 10, 99u)] // different count
        [TestCase(0, 10, 101u)] // different count
        public void Compare_Failure(int min, int max, uint count)
        {
            var expected = new Bin<int>(0, 10) { Count = 100 };
            var actual = new Bin<int>(min, max) { Count = count };
            Assert.That(() =>
            {
                Assert.That(actual, Is.EqualTo(expected).Using(new BinCountComparer<int>()));
            }, Throws.TypeOf<AssertionException>());
        }
    }
}
