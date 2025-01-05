// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine;

namespace TestHelper.Editor.Attributes
{
    [TestFixture]
    public class LoadSceneAttributeTest
    {
        private const string TestScene = "Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild.unity";
        private const string ObjectName = "CubeInNotInScenesInBuild";

        [Test]
        [LoadScene(TestScene)]
        public void Attach_LoadedSceneNotInBuild()
        {
            var cube = GameObject.Find(ObjectName);
            Assert.That(cube, Is.Not.Null);
        }
    }
}
