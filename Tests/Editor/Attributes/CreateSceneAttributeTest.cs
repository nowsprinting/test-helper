// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TestHelper.Editor.Attributes
{
    [TestFixture]
    public class CreateSceneAttributeTest
    {
        [Test]
        [CreateScene(camera: true, light: true)]
        public void Attach_CreateNewSceneWithCameraAndLight()
        {
            var scene = SceneManager.GetActiveScene();
            Assert.That(scene.name, Is.EqualTo(
                "Scene of TestHelper.Editor.Attributes.CreateSceneAttributeTest.Attach_CreateNewSceneWithCameraAndLight"));

            var camera = GameObject.Find("Main Camera");
            Assert.That(camera, Is.Not.Null);

            var light = GameObject.Find("Directional Light");
            Assert.That(light, Is.Not.Null);
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
    }
}
