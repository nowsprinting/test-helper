// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using TestHelper.Statistics.Comparers;
using TestHelper.Statistics.RandomGenerators;

namespace TestHelper.Statistics.Histograms
{
    [TestFixture]
    [SuppressMessage("Assertion", "NUnit2045:Use Assert.Multiple")]
    public class HistogramTest
    {
        [Test]
        public void Constructor_Double()
        {
            var actual = new Histogram<double>(0.0d, 1.0d, 0.5d);
            var expected = new SortedList<double, Bin<double>>
            {
                { 0.0d, new Bin<double>(0.0d, 0.5d) }, //
                { 0.5d, new Bin<double>(0.5d, 1.0d) }
            };
            Assert.That(actual.Bins, Is.EquivalentTo(expected).Using(new BinCountComparer<double>()));
        }

        [Test]
        public void Constructor_Int()
        {
            var actual = new Histogram<int>(0, 10, 2);
            var expected = new SortedList<int, Bin<int>>
            {
                { 0, new Bin<int>(0, 2) }, //
                { 2, new Bin<int>(2, 4) }, //
                { 4, new Bin<int>(4, 6) }, //
                { 6, new Bin<int>(6, 8) }, //
                { 8, new Bin<int>(8, 10) }
            };
            Assert.That(actual.Bins, Is.EquivalentTo(expected).Using(new BinCountComparer<int>()));
        }

        [Test]
        public void Constructor_WithoutInitialBins()
        {
            var actual = new Histogram<Coin>();
            Assert.That(actual.Bins, Is.Empty);
        }

        [Test]
        public void Plot_WithoutRange()
        {
            var sut = new Histogram<Coin>();
            sut.Plot(new[] { Coin.Head, Coin.Tail, Coin.Tail });

            var expected = new List<Bin<Coin>>
            {
                new Bin<Coin>(Coin.Head) { Count = 1 }, //
                new Bin<Coin>(Coin.Tail) { Count = 2 }
            };
            Assert.That(sut.Bins.Values, Is.EquivalentTo(expected).Using(new BinCountComparer<Coin>()));
        }

        [Test]
        public void Plot_WithRange()
        {
            var sut = new Histogram<int>(0, 10, 2);
            sut.Plot(new[] { 2, 3, 4, 4, 5, 5, 9 });

            var expected = new List<Bin<int>>
            {
                new Bin<int>(0, 2) { Count = 0 }, //
                new Bin<int>(2, 4) { Count = 2 }, //
                new Bin<int>(4, 6) { Count = 4 }, //
                new Bin<int>(6, 8) { Count = 0 }, //
                new Bin<int>(8, 10) { Count = 1 }
            };
            Assert.That(sut.Bins.Values, Is.EquivalentTo(expected).Using(new BinCountComparer<int>()));
        }

        [Test]
        public void Calculate_Odd_CalculatedFeaturesSetToProperties()
        {
            var sut = new Histogram<int>
            {
                Bins = new SortedList<int, Bin<int>>
                {
                    { 0, new Bin<int>(0) { Count = 4 } }, // Median
                    { 1, new Bin<int>(1) { Count = 6 } }, //
                    { 2, new Bin<int>(2) { Count = 2 } }, // 
                    { 3, new Bin<int>(3) { Count = 10 } }, // Peak
                    { 4, new Bin<int>(4) { Count = 0 } }, // Valley
                }
            };
            sut.Calculate();

            Assert.That(sut.Peak, Is.EqualTo(10));
            Assert.That(sut.Valley, Is.EqualTo(0));
            Assert.That(sut.Median, Is.EqualTo(4));
            Assert.That(sut.Mean, Is.EqualTo(4.4)); // 22 / 5
        }

        [Test]
        public void Calculate_Even_CalculatedFeaturesSetToProperties()
        {
            var sut = new Histogram<int>
            {
                Bins = new SortedList<int, Bin<int>>
                {
                    { 0, new Bin<int>(0) { Count = 4 } }, //
                    { 1, new Bin<int>(1) { Count = 6 } }, //
                    { 2, new Bin<int>(2) { Count = 2 } }, // 
                    { 3, new Bin<int>(3) { Count = 10 } }, // Peak
                    { 4, new Bin<int>(4) { Count = 0 } }, // Valley
                    { 5, new Bin<int>(5) { Count = 3 } }, // 
                }
            };
            sut.Calculate();

            Assert.That(sut.Peak, Is.EqualTo(10));
            Assert.That(sut.Valley, Is.EqualTo(0));
            Assert.That(sut.Median, Is.EqualTo(3.5)); // (3 + 4) / 2
            Assert.That(sut.Mean, Is.EqualTo(4.17).Within(0.01)); // 25 / 6
        }

        [Test]
        public void DrawHistogramAscii()
        {
            var sut = new Histogram<int>
            {
                Bins = new SortedList<int, Bin<int>>
                {
                    { 0, new Bin<int>(0) { Count = 100 } }, //
                    { 1, new Bin<int>(1) { Count = 110 } }, //
                    { 2, new Bin<int>(2) { Count = 120 } }, // 
                    { 3, new Bin<int>(3) { Count = 130 } }, //
                    { 4, new Bin<int>(4) { Count = 140 } }, //
                    { 5, new Bin<int>(5) { Count = 150 } }, // 
                    { 6, new Bin<int>(6) { Count = 160 } }, // 
                    { 7, new Bin<int>(7) { Count = 170 } }, // 
                }
            };
            sut.Calculate();
            var actual = sut.DrawHistogramAscii();

            Assert.That(actual, Is.EqualTo("\u2581\u2582\u2583\u2584\u2585\u2586\u2587\u2588"));
        }

        [Test]
        public void DrawHistogramAscii_OneValue_DrawFullBlock()
        {
            var sut = new Histogram<int>
            {
                Bins = new SortedList<int, Bin<int>>
                {
                    { 0, new Bin<int>(0) { Count = 100 } }, //
                    { 1, new Bin<int>(1) { Count = 100 } }, //
                }
            };
            sut.Calculate();
            var actual = sut.DrawHistogramAscii();

            Assert.That(actual, Is.EqualTo("\u2588\u2588"));
        }
    }
}
