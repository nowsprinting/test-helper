// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TestHelper.Attributes
{
    [TestFixture]
    public class CreateSceneAttributeTest
    {
        [Test]
        [CreateScene]
        public void Attach_CreateNewSceneWithoutCameraAndLight()
        {
            var scene = SceneManager.GetActiveScene();
            Assert.That(scene.name, Is.EqualTo(
                "Scene of TestHelper.Attributes.CreateSceneAttributeTest.Attach_CreateNewSceneWithoutCameraAndLight"));

            var camera = GameObject.Find("Main Camera");
            Assert.That(camera, Is.Null);

            var light = GameObject.Find("Directional Light");
            Assert.That(light, Is.Null);
        }

        [Test]
        [CreateScene(camera: true)]
        public void Attach_WithCamera_CreateNewSceneWithCamera()
        {
            var scene = SceneManager.GetActiveScene();
            Assert.That(scene.name, Is.EqualTo(
                "Scene of TestHelper.Attributes.CreateSceneAttributeTest.Attach_WithCamera_CreateNewSceneWithCamera"));

            var camera = GameObject.Find("Main Camera");
            Assert.That(camera, Is.Not.Null);
        }

        [Test]
        [CreateScene(light: true)]
        public void Attach_WithLight_CreateNewSceneWithLight()
        {
            var scene = SceneManager.GetActiveScene();
            Assert.That(scene.name, Is.EqualTo(
                "Scene of TestHelper.Attributes.CreateSceneAttributeTest.Attach_WithLight_CreateNewSceneWithLight"));

            var light = GameObject.Find("Directional Light");
            Assert.That(light, Is.Not.Null);
        }

        [Test]
        [CreateScene]
        public void AttachToParameterizedTest_CreateNewScene([Values(0, 1)] int i)
        {
            var scene = SceneManager.GetActiveScene();
            Assert.That(scene.name, Is.EqualTo(
                $"Scene of TestHelper.Attributes.CreateSceneAttributeTest.AttachToParameterizedTest_CreateNewScene({i})"));
        }

        [Test]
        [CreateScene]
        public async Task AttachToAsyncTest_CreateNewScene()
        {
            var scene = SceneManager.GetActiveScene();
            Assert.That(scene.name, Is.EqualTo(
                "Scene of TestHelper.Attributes.CreateSceneAttributeTest.AttachToAsyncTest_CreateNewScene"));

            await Task.Yield();
        }

        [UnityTest]
        [CreateScene]
        public IEnumerator AttachToUnityTest_CreateNewScene()
        {
            var scene = SceneManager.GetActiveScene();
            Assert.That(scene.name, Is.EqualTo(
                "Scene of TestHelper.Attributes.CreateSceneAttributeTest.AttachToUnityTest_CreateNewScene"));

            yield return null;
        }
    }
}
