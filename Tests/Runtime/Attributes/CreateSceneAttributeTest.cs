// Copyright (c) 2023-2025 Koji Hasegawa.
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
        [Category("Acceptance")]
        [CreateScene(camera: true)]
        public void Attach_WithCamera_CameraMainIsCreatedCamera()
        {
            var mainCamera = Camera.main;
            Assert.That(mainCamera, Is.Not.Null);
            Assert.That(mainCamera.gameObject.name, Is.EqualTo("Main Camera"));
            Assert.That(SceneManager.GetActiveScene().GetRootGameObjects(), Has.Member(mainCamera.gameObject));
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
        // Not an async Task test: this test verifies the attribute on a coroutine-style UnityTest method;
        // the async Task variant is covered by the AttachToAsyncTest_ test.
#pragma warning disable UTF4006
        public IEnumerator AttachToUnityTest_CreateNewScene()
#pragma warning restore UTF4006
        {
            var scene = SceneManager.GetActiveScene();
            Assert.That(scene.name, Is.EqualTo(
                "Scene of TestHelper.Attributes.CreateSceneAttributeTest.AttachToUnityTest_CreateNewScene"));

            yield return null;
        }

        [UnityTest]
        [CreateScene]
        // Not an async Task test: awaiting AsyncOperation requires Unity 2023.1 or later,
        // but this package supports older versions.
#pragma warning disable UTF4006
        public IEnumerator UnloadCreatedSceneInTest_NoErrorInAfterTest()
#pragma warning restore UTF4006
        {
            var createdScene = SceneManager.GetActiveScene();

            var newScene = SceneManager.CreateScene("New Scene");
            SceneManager.SetActiveScene(newScene);

            yield return SceneManager.UnloadSceneAsync(createdScene);
        }

        [TestFixture]
        public class UnloadOthersOptionTest
        {
            private const string TestSceneName = "UnloadOthersOptionTestScene";

            [Test]
            [Order(0)]
            public void UnloadOthersOption_BeforeTest()
            {
                SceneManager.CreateScene(TestSceneName); // create dummy scene, not activate.
            }

            [Test]
            [Order(1)]
            [CreateScene(unloadOthers: false)]
            public void UnloadOthersOption_False_NotUnloadOtherScenes()
            {
                var scene = SceneManager.GetSceneByName(TestSceneName);
                Assert.That(scene.isLoaded, Is.True);
            }

            [Test]
            [Order(2)]
            [CreateScene(unloadOthers: true)]
            public void UnloadOthersOption_True_UnloadOtherScenes()
            {
                var scene = SceneManager.GetSceneByName(TestSceneName);
                Assert.That(scene.isLoaded, Is.False);
            }
        }
    }
}
