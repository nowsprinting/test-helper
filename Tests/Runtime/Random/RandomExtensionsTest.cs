// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;
using UnityEngine;

namespace TestHelper.Random
{
    [TestFixture]
    public class RandomExtensionsTest
    {
        private const int RepeatCount = 10;

        [Test]
        [Repeat(RepeatCount)]
        public void NextScreenPosition_InRange()
        {
            var sut = new RandomWrapper();
            var actual = sut.NextScreenPosition();

            Assert.That(actual.x, Is.InRange(0, Screen.width), "In screen width");
            Assert.That(actual.y, Is.InRange(0, Screen.height), "In screen height");
        }

        [Test]
        [Repeat(RepeatCount)]
        public void NextNormalizedVector2_InRange()
        {
            var sut = new RandomWrapper();
            var actual = sut.NextNormalizedVector2();

            Assert.That(actual.x, Is.InRange(-1.0f, 1.0f));
            Assert.That(actual.y, Is.InRange(-1.0f, 1.0f));
        }

        [Test]
        [Repeat(RepeatCount)]
        public void NextNormalizedVector3_InRange()
        {
            var sut = new RandomWrapper();
            var actual = sut.NextNormalizedVector3();

            Assert.That(actual.x, Is.InRange(-1.0f, 1.0f));
            Assert.That(actual.y, Is.InRange(-1.0f, 1.0f));
            Assert.That(actual.z, Is.InRange(-1.0f, 1.0f));
        }
    }
}
