// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[assembly: ScenesUsingInTest("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild_Assembly.unity")]

namespace TestHelper.Attributes
{
    [TestFixture]
    [ScenesUsingInTest("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild_Class.unity")]
    [SuppressMessage("ReSharper", "Unity.LoadSceneUnexistingScene")]
    public class ScenesUsingInTestAttributeTest
    {
        [UnityTest]
        public IEnumerator AttachedToAssembly_CanLoadSceneNotIncludedBuild()
        {
            yield return SceneManager.LoadSceneAsync("NotInScenesInBuild_Assembly");
            var cube = GameObject.Find("CubeInNotInScenesInBuild_Assembly");
            Assert.That(cube, Is.Not.Null);
        }

        [UnityTest]
        public IEnumerator AttachedToClass_CanLoadSceneNotIncludedBuild()
        {
            yield return SceneManager.LoadSceneAsync("NotInScenesInBuild_Class");
            var cube = GameObject.Find("CubeInNotInScenesInBuild_Class");
            Assert.That(cube, Is.Not.Null);
        }

        [UnityTest]
        [ScenesUsingInTest("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild.unity")]
        public IEnumerator AttachedToMethod_CanLoadSceneNotIncludedBuild()
        {
            yield return SceneManager.LoadSceneAsync("NotInScenesInBuild");
            var cube = GameObject.Find("CubeInNotInScenesInBuild");
            Assert.That(cube, Is.Not.Null);
        }

        [UnityTest]
        [ScenesUsingInTest("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild2.unity")]
        [ScenesUsingInTest("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild3.unity")]
        public IEnumerator AttachedToMethodMultiple_CanLoadScenesNotIncludedBuild()
        {
            yield return SceneManager.LoadSceneAsync("NotInScenesInBuild2");
            var cube2 = GameObject.Find("CubeInNotInScenesInBuild2");
            Assert.That(cube2, Is.Not.Null);

            yield return SceneManager.LoadSceneAsync("NotInScenesInBuild3");
            var cube3 = GameObject.Find("CubeInNotInScenesInBuild3");
            Assert.That(cube3, Is.Not.Null);
        }

        [UnityTest]
        [ScenesUsingInTest("Packages/com.nowsprinting.test-helper/Tests/Scenes/Sub/")]  // Note: Need trailing slash on Linux?
        public IEnumerator SpecifyDirectory_CanLoadScenesNotIncludedBuild()
        {
            yield return SceneManager.LoadSceneAsync("NotInScenesInBuild4");
            var cube4 = GameObject.Find("CubeInNotInScenesInBuild4");
            Assert.That(cube4, Is.Not.Null);

            yield return SceneManager.LoadSceneAsync("NotInScenesInBuild5");
            var cube5 = GameObject.Find("CubeInNotInScenesInBuild5");
            Assert.That(cube5, Is.Not.Null);
        }
    }
}
