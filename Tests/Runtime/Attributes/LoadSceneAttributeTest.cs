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
        // Not an async Task test: this test verifies the attribute on a coroutine-style UnityTest method;
        // the async Task variant is covered by the AttachToAsyncTest_ test.
#pragma warning disable UTF4006
        public IEnumerator AttachToUnityTest_LoadedSceneNotInBuild()
#pragma warning restore UTF4006
        {
            var cube = GameObject.Find(ObjectName);
            Assert.That(cube, Is.Not.Null);

            Object.Destroy(cube); // For not giving false negatives in subsequent tests.
            yield return null;
        }

        [Test]
        [LoadScene("Packages/com.nowsprinting.test-helper/**/NotInScenesInBuildForGlob.unity")]
        public void UsingGlob_LoadedSceneNotInBuild()
        {
            var cube = GameObject.Find(ObjectName);
            Assert.That(cube, Is.Not.Null);

            Object.Destroy(cube); // For not giving false negatives in subsequent tests.
        }

        [Test]
        [LoadScene("../../Scenes/NotInScenesInBuildForRelative.unity")]
        public void UsingRelativePath_LoadedSceneNotInBuild()
        {
            var cube = GameObject.Find(ObjectName);
            Assert.That(cube, Is.Not.Null);

            Object.Destroy(cube); // For not giving false negatives in subsequent tests.
        }

        [Test]
        [LoadScene]
        public void UsingInferredPath_LoadedSceneNotInBuild()
        {
            var cube = GameObject.Find(ObjectName);
            Assert.That(cube, Is.Not.Null);

            Object.Destroy(cube); // For not giving false negatives in subsequent tests.
        }
    }
}
