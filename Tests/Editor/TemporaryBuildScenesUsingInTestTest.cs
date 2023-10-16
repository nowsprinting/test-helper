// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System.Linq;
using NUnit.Framework;

namespace TestHelper.Editor
{
    [TestFixture]
    public class TemporaryBuildScenesUsingInTestTest
    {
        [Test]
        public void GetScenesUsingInTest_AttachedToAssembly_ReturnScenesSpecifiedByAttribute()
        {
            var actual = TemporaryBuildScenesUsingInTest.GetScenesUsingInTest();
            Assert.That(actual,
                Does.Contain("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild_Assembly.unity"));
        }

        [Test]
        public void GetScenesUsingInTest_AttachedToClass_ReturnScenesSpecifiedByAttribute()
        {
            var actual = TemporaryBuildScenesUsingInTest.GetScenesUsingInTest();
            Assert.That(actual,
                Does.Contain("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild_Class.unity"));
        }

        [Test]
        public void GetScenesUsingInTest_AttachedToMethod_ReturnScenesSpecifiedByAttribute()
        {
            var actual = TemporaryBuildScenesUsingInTest.GetScenesUsingInTest();
            Assert.That(actual,
                Does.Contain("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild.unity"));
        }

        [Test]
        public void GetScenesUsingInTest_AttachedToMethodMultiple_ReturnScenesSpecifiedByAttribute()
        {
            var actual = TemporaryBuildScenesUsingInTest.GetScenesUsingInTest().ToArray();
            Assert.That(actual,
                Does.Contain("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild2.unity"));
            Assert.That(actual,
                Does.Contain("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild3.unity"));
        }

        [Test]
        public void GetScenesUsingInTest_SpecifyDirectory_ReturnScenesSpecifiedByAttribute()
        {
            var actual = TemporaryBuildScenesUsingInTest.GetScenesUsingInTest().ToArray();
            Assert.That(actual,
                Does.Contain("Packages/com.nowsprinting.test-helper/Tests/Scenes/Sub/NotInScenesInBuild4.unity"));
            Assert.That(actual,
                Does.Contain("Packages/com.nowsprinting.test-helper/Tests/Scenes/Sub/NotInScenesInBuild5.unity"));
        }
    }
}
