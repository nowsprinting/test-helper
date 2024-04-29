// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;
using UnityEngine;

namespace TestHelper.Attributes
{
    /// <seealso cref="LoadSceneAttributeTest"/>
    [TestFixture]
    public class BuildSceneAttributeTest
    {
        private const string ObjectName = "CubeInNotInScenesInBuild";

        [Test]
        [BuildScene("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuildForUse.unity")]
        public void Attach_BuildScene()
        {
            var cube = GameObject.Find(ObjectName);
            Assume.That(cube, Is.Null, "Not loaded ");

            // TODO: Load scene
        }

        [TestCase("./Scene.unity", // include `./`
            "Assets/Tests/Runtime/Caller.cs",
            "Assets/Tests/Runtime/Scene.unity")]
        [TestCase("../../BadPath/../Scenes/Scene.unity", // include `../`
            "Packages/com.nowsprinting.test-helper/Tests/Runtime/Attributes/Caller.cs",
            "Packages/com.nowsprinting.test-helper/Tests/Scenes/Scene.unity")]
        public void GetAbsolutePath(string relativePath, string callerFilePath, string expected)
        {
            var actual = BuildSceneAttribute.GetAbsolutePath(relativePath, callerFilePath);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
