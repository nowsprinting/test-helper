// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;
using TestHelper.Statistics.RandomGenerators;

namespace TestHelper.Statistics
{
    [TestFixture]
    public class ArraySampleSpaceTest
    {
        [Test]
        public void ToString_Empty()
        {
            var sampleSpace = new ArraySampleSpace<int>(0);
            Assert.That(sampleSpace.ToString(), Is.EqualTo("{}"));
        }

        [Test]
        public void ToString_Single()
        {
            var sampleSpace = new ArraySampleSpace<int>(1);
            sampleSpace.Add(1);
            Assert.That(sampleSpace.ToString(), Is.EqualTo("{1}"));
        }

        [Test]
        public void ToString_Multiple()
        {
            var sampleSpace = new ArraySampleSpace<int>(2);
            sampleSpace.Add(1);
            sampleSpace.Add(2);
            Assert.That(sampleSpace.ToString(), Is.EqualTo("{1,2}"));
        }

        [Test]
        public void ToString_Bool()
        {
            var sampleSpace = new ArraySampleSpace<bool>(2);
            sampleSpace.Add(false);
            sampleSpace.Add(true);
            Assert.That(sampleSpace.ToString(), Is.EqualTo("{False,True}"));
        }

        [Test]
        public void ToString_Enum()
        {
            var sampleSpace = new ArraySampleSpace<Coin>(2);
            sampleSpace.Add(Coin.Head);
            sampleSpace.Add(Coin.Tail);
            Assert.That(sampleSpace.ToString(), Is.EqualTo("{Head,Tail}"));
        }
    }
}
