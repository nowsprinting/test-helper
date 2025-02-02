// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;
using TestHelper.Statistics.RandomGenerators;

namespace TestHelper.Statistics
{
    [TestFixture]
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
    }
}
