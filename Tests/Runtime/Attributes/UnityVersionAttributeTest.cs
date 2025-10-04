// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;

namespace TestHelper.Attributes
{
    [TestFixture]
    public class UnityVersionAttributeTest
    {
        private const string UnityVersion = "2023.2.16f1";

        [TestCase("2023.2.16f1")] // include equal version
        [TestCase("2023.2.15f1")]
        [TestCase("2023.2")]
        [TestCase("2023")]
        [TestCase("2022")]
        public void IsSkip_newerThanOrEqual_NotSkip(string newerThanOrEqual)
        {
            var sut = new UnityVersionAttribute(newerThanOrEqual);
            var actual = sut.IsSkip(UnityVersion);
            Assert.That(actual, Is.False);
        }

        [TestCase("2023.2.17f1")]
        [TestCase("2023.3")]
        [TestCase("6000")]
        public void IsSkip_newerThanOrEqual_Skip(string newerThanOrEqual)
        {
            var sut = new UnityVersionAttribute(newerThanOrEqual);
            var actual = sut.IsSkip(UnityVersion);
            Assert.That(actual, Is.True);
        }

        [TestCase("2023.2.17f1")]
        [TestCase("6000")]
        public void IsSkip_olderThan_NotSkip(string olderThan)
        {
            var sut = new UnityVersionAttribute(olderThan: olderThan);
            var actual = sut.IsSkip(UnityVersion);
            Assert.That(actual, Is.False);
        }

        [TestCase("2023.2.16f1")] // exclude equal version
        [TestCase("2023.2.15f1")]
        [TestCase("2023.2")]
        [TestCase("2023")]
        [TestCase("2021")]
        public void IsSkip_olderThan_Skip(string olderThan)
        {
            var sut = new UnityVersionAttribute(olderThan: olderThan);
            var actual = sut.IsSkip(UnityVersion);
            Assert.That(actual, Is.True);
        }

        [TestCase("2023.2.16f1", "2023.2.17f1")]
        public void IsSkip_Both_NotSkip(string newerThanOrEqual, string olderThan)
        {
            var sut = new UnityVersionAttribute(newerThanOrEqual, olderThan);
            var actual = sut.IsSkip(UnityVersion);
            Assert.That(actual, Is.False);
        }

        [TestCase("2023.2.16f1", "2023.2.16f1")]
        [TestCase("2023.2.17f1", "2023.2.17f1")]
        public void IsSkip_Both_Skip(string newerThanOrEqual, string olderThan)
        {
            var sut = new UnityVersionAttribute(newerThanOrEqual, olderThan);
            var actual = sut.IsSkip(UnityVersion);
            Assert.That(actual, Is.True);
        }

        [Test]
        [UnityVersion("2019")]
        public void Attach_newerThanOrEqual2019_NotSkip()
        {
        }

        [Test]
        [UnityVersion(olderThan: "2019.4.0f1")]
        public void Attach_olderThan2019_4_0f1_Skip()
        {
            Assert.Fail("This test should be skipped.");
        }
    }
}
