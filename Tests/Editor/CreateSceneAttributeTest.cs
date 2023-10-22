// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TestHelper.Editor
{
    [TestFixture]
    public class CreateSceneAttributeTest
    {
        [Test]
        [CreateScene]
        public void Attach_CreateNewSceneWithCameraAndLight()
        {
            var scene = SceneManager.GetActiveScene();
            Assert.That(scene.name, Is.EqualTo(
                "Scene of TestHelper.Editor.CreateSceneAttributeTest.Attach_CreateNewSceneWithCameraAndLight"));

            var camera = GameObject.Find("Main Camera");
            Assert.That(camera, Is.Not.Null);

            var light = GameObject.Find("Directional Light");
            Assert.That(light, Is.Not.Null);
        }
    }
}
