// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;

namespace TestHelper.Random
{
    [TestFixture]
    public class StubRandomTest
    {
        [Test]
        public void Next_ReturnSpecifiedValues()
        {
            var sut = new StubRandom(1, 2, 3);

            Assert.That(sut.Next(), Is.EqualTo(1), "No argument");
            Assert.That(sut.Next(0), Is.EqualTo(2), "With max value");
            Assert.That(sut.Next(0, 0), Is.EqualTo(3), "With range");
        }

        [Test]
        public void ToString_ReturnName()
        {
            var sut = new StubRandom(1, 2, 3);

            Assert.That(sut.ToString(), Is.EqualTo(sut.GetType().FullName));
        }
    }
}
