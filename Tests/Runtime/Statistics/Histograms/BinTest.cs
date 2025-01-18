// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;
using TestHelper.Statistics.RandomGenerators;

namespace TestHelper.Statistics.Histograms
{
    [TestFixture]
    public class BinTest
    {
        [Test]
        public void Constructor_Bool()
        {
            var actual = new Bin<bool>(true);
            Assert.That(actual.Min, Is.EqualTo(true));
            Assert.That(actual.Max, Is.EqualTo(true));
            Assert.That(actual.Label, Is.EqualTo("True"));
        }

        [Test]
        public void Constructor_Decimal()
        {
            var actual = new Bin<decimal>(1);
            Assert.That(actual.Min, Is.EqualTo(1));
            Assert.That(actual.Max, Is.EqualTo(1));
            Assert.That(actual.Label, Is.EqualTo("1"));
        }

        [Test]
        public void Constructor_DecimalWithRange()
        {
            var actual = new Bin<decimal>(1, 2);
            Assert.That(actual.Min, Is.EqualTo(1));
            Assert.That(actual.Max, Is.EqualTo(2));
            Assert.That(actual.Label, Is.EqualTo("1"));
        }

        [Test]
        public void Constructor_Enum()
        {
            var actual = new Bin<Coin>(Coin.Head);
            Assert.That(actual.Min, Is.EqualTo(Coin.Head));
            Assert.That(actual.Max, Is.EqualTo(Coin.Head));
            Assert.That(actual.Label, Is.EqualTo("Head"));
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(2, false)]
        public void IsInRange_HasNotRange(int value, bool expected)
        {
            var sut = new Bin<int>(1);
            Assert.That(sut.IsInRange(value), Is.EqualTo(expected));
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(2, true)]
        [TestCase(3, false)]
        [TestCase(4, false)]
        public void IsInRange_HasRange(int value, bool expected)
        {
            var sut = new Bin<int>(1, 3);
            Assert.That(sut.IsInRange(value), Is.EqualTo(expected));
        }
    }
}
