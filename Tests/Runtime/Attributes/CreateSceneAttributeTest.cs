// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
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

            var rootGameObjects = scene.GetRootGameObjects(); // Note: GameObject.Find finds objects in inactive scenes
            Assert.That(rootGameObjects, Is.Empty);
        }

        [Test]
        [CreateScene(camera: true)]
        public void Attach_WithCamera_CreateNewSceneWithCamera()
        {
            var scene = SceneManager.GetActiveScene();
            Assert.That(scene.name, Is.EqualTo(
                "Scene of TestHelper.Attributes.CreateSceneAttributeTest.Attach_WithCamera_CreateNewSceneWithCamera"));

            var rootGameObjects = scene.GetRootGameObjects(); // Note: GameObject.Find finds objects in inactive scenes
            Assert.That(rootGameObjects, Has.Length.EqualTo(1));
            Assert.That(rootGameObjects[0].name, Is.EqualTo("Main Camera"));
        }

        [Test]
        [CreateScene(light: true)]
        public void Attach_WithLight_CreateNewSceneWithLight()
        {
            var scene = SceneManager.GetActiveScene();
            Assert.That(scene.name, Is.EqualTo(
                "Scene of TestHelper.Attributes.CreateSceneAttributeTest.Attach_WithLight_CreateNewSceneWithLight"));

            var rootGameObjects = scene.GetRootGameObjects(); // Note: GameObject.Find finds objects in inactive scenes
            Assert.That(rootGameObjects, Has.Length.EqualTo(1));
            Assert.That(rootGameObjects[0].name, Is.EqualTo("Directional Light"));
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
