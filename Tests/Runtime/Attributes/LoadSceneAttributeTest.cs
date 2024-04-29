// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace TestHelper.Attributes
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

            Object.Destroy(cube); // For not giving false negatives in subsequent tests.
        }

        [Test]
        [LoadScene(TestScene)]
        public async Task AttachToAsyncTest_LoadedSceneNotInBuild()
        {
            var cube = GameObject.Find(ObjectName);
            Assert.That(cube, Is.Not.Null);

            Object.Destroy(cube); // For not giving false negatives in subsequent tests.
            await Task.Yield();
        }

        [UnityTest]
        [LoadScene(TestScene)]
        public IEnumerator AttachToUnityTest_LoadedSceneNotInBuild()
        {
            var cube = GameObject.Find(ObjectName);
            Assert.That(cube, Is.Not.Null);

            Object.Destroy(cube); // For not giving false negatives in subsequent tests.
            yield return null;
        }

        [Test]
        [LoadScene("Packages/com.nowsprinting.test-helper/**/NotInScenesInBuild.unity")]
        public void UsingGlob_LoadedSceneNotInBuild()
        {
            var cube = GameObject.Find(ObjectName);
            Assert.That(cube, Is.Not.Null);

            Object.Destroy(cube); // For not giving false negatives in subsequent tests.
        }

        [Test]
        [LoadScene("../../Scenes/NotInScenesInBuild.unity")]
        public void UsingRelativePath_LoadedSceneNotInBuild()
        {
            var cube = GameObject.Find(ObjectName);
            Assert.That(cube, Is.Not.Null);

            Object.Destroy(cube); // For not giving false negatives in subsequent tests.
        }

        [TestCase("./Scene.unity", // include `./`
            "Assets/Tests/Runtime/Caller.cs",
            "Assets/Tests/Runtime/Scene.unity")]
        [TestCase("../../BadPath/../Scenes/Scene.unity", // include `../`
            "Packages/com.nowsprinting.test-helper/Tests/Runtime/Attributes/Caller.cs",
            "Packages/com.nowsprinting.test-helper/Tests/Scenes/Scene.unity")]
        public void GetAbsolutePath(string relativePath, string callerFilePath, string expected)
        {
            var actual = LoadSceneAttribute.GetAbsolutePath(relativePath, callerFilePath);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
