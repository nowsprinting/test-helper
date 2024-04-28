// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace TestHelper.Utils
{
    [TestFixture]
    public class ScenePathFinderTest
    {
        [TestCase("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild.unity")]
        [TestCase("Packages/com.nowsprinting.test-helper/Tes?s/S?enes/NotInScenesInBuild.unity")]
        [TestCase("Packages/com.nowsprinting.test-helper/*/*/NotInScenesInBuild.unity")]
        [TestCase("Packages/com.nowsprinting.test-helper/**/NotInScenesInBuild.unity")]
        public void GetExistScenePath_ExistPath_GotExistScenePath(string path)
        {
#if UNITY_EDITOR
            const string ExistScenePath = "Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild.unity";
#else
            const string ExistScenePath = "NotInScenesInBuild"; // Scene name only
#endif

            var actual = ScenePathFinder.GetExistScenePath(path);
            Assert.That(actual, Is.EqualTo(ExistScenePath));
        }

        [TestCase("Packages/**/NotInScenesInBuild.unity")]
        [TestCase("**/NotInScenesInBuild.unity")]
        [TestCase("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild")]
        [TestCase("Packages/com.nowsprinting.test-helper/Tests/Scenes/Not??ScenesInBuild.unity")]
        [TestCase("Packages/com.nowsprinting.test-helper/Tests/Scenes/*InScenesInBuild.unity")]
        public void GetExistScenePath_InvalidGlobPattern_ThrowsArgumentException(string path)
        {
            Assert.That(() => ScenePathFinder.GetExistScenePath(path), Throws.TypeOf<ArgumentException>());
        }

        [TestCase("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotExistScene.unity")] // Not exist path
        [TestCase("Packages/com.nowsprinting.test-helper/*/NotInScenesInBuild.unity")] // Not match path pattern
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor)]
        public void GetExistScenePath_NotExistPath_InEditor_ThrowsFileNotFoundException(string path)
        {
            Assert.That(() => ScenePathFinder.GetExistScenePath(path), Throws.TypeOf<FileNotFoundException>());
            // Note: Returns scene name when running on player.
        }

        [TestCase("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild.unity")]
        public void GetExistScenePathOnPlayer_GotSceneName(string path)
        {
            const string SceneName = "NotInScenesInBuild";

            var actual = ScenePathFinder.GetExistScenePathOnPlayer(path);
            Assert.That(actual, Is.EqualTo(SceneName));
        }
    }
}
