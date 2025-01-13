// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using TestHelper.Statistics.Comparers;
using TestHelper.Statistics.RandomGenerators;
using UnityEngine;

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
                new Bin<Coin>(Coin.Head) { Frequency = 1 }, //
                new Bin<Coin>(Coin.Tail) { Frequency = 2 }
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
                new Bin<int>(0, 2) { Frequency = 0 }, //
                new Bin<int>(2, 4) { Frequency = 2 }, //
                new Bin<int>(4, 6) { Frequency = 4 }, //
                new Bin<int>(6, 8) { Frequency = 0 }, //
                new Bin<int>(8, 10) { Frequency = 1 }
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
                    { 0, new Bin<int>(0) { Frequency = 4 } }, // Median
                    { 1, new Bin<int>(1) { Frequency = 6 } }, //
                    { 2, new Bin<int>(2) { Frequency = 2 } }, // 
                    { 3, new Bin<int>(3) { Frequency = 10 } }, // Peak
                    { 4, new Bin<int>(4) { Frequency = 0 } }, // Valley
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
                    { 0, new Bin<int>(0) { Frequency = 4 } }, //
                    { 1, new Bin<int>(1) { Frequency = 6 } }, //
                    { 2, new Bin<int>(2) { Frequency = 2 } }, // 
                    { 3, new Bin<int>(3) { Frequency = 10 } }, // Peak
                    { 4, new Bin<int>(4) { Frequency = 0 } }, // Valley
                    { 5, new Bin<int>(5) { Frequency = 3 } }, // 
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
                    { 0, new Bin<int>(0) { Frequency = 100 } }, // lower 1:8 block
                    { 1, new Bin<int>(1) { Frequency = 110 } }, // lower 1:4 block
                    { 2, new Bin<int>(2) { Frequency = 120 } }, // lower 3:8 block
                    { 3, new Bin<int>(3) { Frequency = 130 } }, // lower 1:2 block
                    { 4, new Bin<int>(4) { Frequency = 140 } }, // lower 5:8 block
                    { 5, new Bin<int>(5) { Frequency = 150 } }, // lower 3:4 block
                    { 6, new Bin<int>(6) { Frequency = 160 } }, // lower 7:8 block
                    { 7, new Bin<int>(7) { Frequency = 170 } }, // full block
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
                    { 0, new Bin<int>(0) { Frequency = 100 } }, // full block
                    { 1, new Bin<int>(1) { Frequency = 100 } }, // full block
                }
            };
            sut.Calculate();
            var actual = sut.DrawHistogramAscii();

            Assert.That(actual, Is.EqualTo("\u2588\u2588"));
        }

        [Test]
        public void DrawHistogramAscii_ZeroValue_DrawSpace()
        {
            var sut = new Histogram<int>
            {
                Bins = new SortedList<int, Bin<int>>
                {
                    { 0, new Bin<int>(0) { Frequency = 1000 } }, // full block
                    { 1, new Bin<int>(1) { Frequency = 0 } }, // space
                    { 2, new Bin<int>(2) { Frequency = 1 } }, // lower 1:8 block
                }
            };
            sut.Calculate();
            var actual = sut.DrawHistogramAscii();

            Assert.That(actual, Is.EqualTo("\u2588 \u2581"));
        }

        [Test]
        public void GetSummary()
        {
            var sut = new Histogram<int>(0, 10, 2);
            sut.Plot(new[] { 2, 3, 4, 4, 5, 5, 9 }, 7, 2, 9);

            var actual = sut.GetSummary();
            Debug.Log(actual);

            Assert.That(actual, Is.EqualTo(@"---
Experimental and Statistical Summary:
  Sample size: 7
  Maximum: 9
  Minimum: 2
  Peak frequency: 4
  Valley frequency: 0
  Median: 1
  Mean: 1.40
  Histogram:  ▃█ ▁
  (Each bar represents the frequency of values in equally spaced bins.)
"));
        }

        [Test]
        public void GetSummary_WithSampleSpace()
        {
            var sampleSpace = Experiment.Run(
                () => DiceGenerator.Roll(2, 6), // 2D6
                1 << 20); // 1,048,576 times

            var histogram = new Histogram<int>();
            histogram.Plot(sampleSpace);
            Debug.Log(histogram.GetSummary());
        }
    }
}
