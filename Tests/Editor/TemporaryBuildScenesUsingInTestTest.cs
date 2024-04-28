// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;

namespace TestHelper.Editor
{
    [TestFixture]
    public class TemporaryBuildScenesUsingInTestTest
    {
        private const string TestScene = "Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild.unity";

        [Test]
        public void GetScenesUsingInTest_AttachedToMethod_ReturnScenesSpecifiedByAttribute()
        {
            var actual = TemporaryBuildScenesUsingInTest.GetScenesUsingInTest();
            Assert.That(actual, Does.Contain(TestScene));
        }
    }
}
