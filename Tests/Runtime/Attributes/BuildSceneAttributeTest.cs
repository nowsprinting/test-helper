// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using TestHelper.RuntimeInternals;
using UnityEngine;

namespace TestHelper.Attributes
{
    /// <seealso cref="LoadSceneAttributeTest"/>
    [TestFixture]
    public class BuildSceneAttributeTest
    {
        private const string TestScene = "../../Scenes/NotInScenesInBuildForUse.unity";
        private const string ObjectName = "CubeInNotInScenesInBuild";

        [Test]
        [BuildScene(TestScene)]
        public async Task Attach_SceneIntoBuild()
        {
            var cube = GameObject.Find(ObjectName);
            Assume.That(cube, Is.Null, "Not loaded ");

            await SceneManagerHelper.LoadSceneCoroutine(TestScene); // Can also be loaded by running the player
            cube = GameObject.Find(ObjectName);
            Assume.That(cube, Is.Not.Null);
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
