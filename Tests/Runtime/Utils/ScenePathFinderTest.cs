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
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor)]
        public void GetExistScenePath_InEditor_GotExistScenePath(string path)
        {
            const string ExistScenePath = "Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild.unity";

            var actual = ScenePathFinder.GetExistScenePath(path);
            Assert.That(actual, Is.EqualTo(ExistScenePath));
        }

        [TestCase("Tests/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild.unity")]
        [TestCase("Packages/**/NotInScenesInBuild.unity")]
        [TestCase("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild")]
        [TestCase("Packages/com.nowsprinting.test-helper/Tests/Scenes/Not??ScenesInBuild.unity")]
        [TestCase("Packages/com.nowsprinting.test-helper/Tests/Scenes/*InScenesInBuild.unity")]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor)]
        public void GetExistScenePath_InEditor_ThrowsArgumentException(string path)
        {
            Assert.That(() => ScenePathFinder.GetExistScenePath(path), Throws.TypeOf<ArgumentException>());
        }

        [TestCase("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotExistScene.unity")]
        [TestCase("Packages/com.nowsprinting.test-helper/*/NotInScenesInBuild.unity")] // Not match path pattern
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor)]
        public void GetExistScenePath_InEditor_ThrowsFileNotFoundException(string path)
        {
            Assert.That(() => ScenePathFinder.GetExistScenePath(path), Throws.TypeOf<FileNotFoundException>());
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
