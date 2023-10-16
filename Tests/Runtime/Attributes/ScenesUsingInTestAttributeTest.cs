// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TestHelper.Attributes
{
    [TestFixture]
    [SuppressMessage("ReSharper", "Unity.LoadSceneUnexistingScene")]
    public class ScenesUsingInTestAttributeTest
    {
        [UnityTest]
        [ScenesUsingInTest("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild.unity")]
        public IEnumerator Attach_CanLoadSceneNotIncludedBuild()
        {
            yield return SceneManager.LoadSceneAsync("NotInScenesInBuild");
            var cube = GameObject.Find("CubeInNotInScenesInBuild");
            Assert.That(cube, Is.Not.Null);
        }

        [UnityTest]
        [ScenesUsingInTest("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild.unity")]
        [ScenesUsingInTest("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild2.unity")]
        public IEnumerator AttachMultiple_CanLoadScenesNotIncludedBuild()
        {
            yield return SceneManager.LoadSceneAsync("NotInScenesInBuild");
            var cube = GameObject.Find("CubeInNotInScenesInBuild");
            Assert.That(cube, Is.Not.Null);

            yield return SceneManager.LoadSceneAsync("NotInScenesInBuild2");
            var cube2 = GameObject.Find("CubeInNotInScenesInBuild2");
            Assert.That(cube2, Is.Not.Null);
        }
    }
}
