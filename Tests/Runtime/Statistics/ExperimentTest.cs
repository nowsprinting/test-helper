// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using TestHelper.Statistics.RandomGenerators;
using UnityEngine;

namespace TestHelper.Statistics
{
    [TestFixture]
    [SuppressMessage("Assertion", "NUnit2045:Use Assert.Multiple")]
    public class ExperimentTest
    {
        [Test]
        public void Run_Enum_ReturnsSampleSpace()
        {
            var actual = Experiment.Run(
                () => CoinGenerator.Toss(),
                1 << 20); // 1,048,576 times

            Assert.That(actual.Min, Is.EqualTo(Coin.Head));
            Assert.That(actual.Max, Is.EqualTo(Coin.Tail));
        }

        [Test]
        public void Run_Int_ReturnsSampleSpace()
        {
            var actual = Experiment.Run(
                () => DiceGenerator.Roll(2, 6), // 2D6
                1 << 20); // 1,048,576 times

            Assert.That(actual.Min, Is.EqualTo(2));
            Assert.That(actual.Max, Is.EqualTo(12));
        }

        [Test]
        public void Run_Float_ReturnsSampleSpace()
        {
            var sampleSpace = Experiment.Run(
                () => Random.value, // 0.0f to 1.0f
                1 << 20); // 1,048,576 times

            var range = sampleSpace.Max - sampleSpace.Min;
            Assert.That(range, Is.EqualTo(1.0f).Within(0.01f));
        }
    }
}
