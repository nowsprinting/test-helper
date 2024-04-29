// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace TestHelper.RuntimeInternals
{
    [TestFixture]
    public class SceneManagerHelperTest
    {
        [TestCase("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild.unity")]
        [TestCase("Packages/com.nowsprinting.test-helper/**/NotInScenesInBuildForGlob.unity")]
        [TestCase("../Scenes/NotInScenesInBuildForRelative.unity")]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor)]
        // Note: Tests to run on the player, see `BuildSceneAttributeTest`
        public async Task LoadSceneCoroutine_LoadedScene(string path)
        {
            await SceneManagerHelper.LoadSceneCoroutine(path);
            var cube = GameObject.Find("CubeInNotInScenesInBuild");
            Assume.That(cube, Is.Not.Null);
        }

        [TestCase("./Scene.unity", // include `./`
            "Assets/Tests/Runtime/Caller.cs",
            "Assets/Tests/Runtime/Scene.unity")]
        [TestCase("../../BadPath/../Scenes/Scene.unity", // include `../`
            "Packages/com.nowsprinting.test-helper/Tests/Runtime/Attributes/Caller.cs",
            "Packages/com.nowsprinting.test-helper/Tests/Scenes/Scene.unity")]
        public void GetAbsolutePath(string relativePath, string callerFilePath, string expected)
        {
            var actual = SceneManagerHelper.GetAbsolutePath(relativePath, callerFilePath);
            Assert.That(actual, Is.EqualTo(expected));
        }

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

            var actual = SceneManagerHelper.GetExistScenePath(path, null);
            Assert.That(actual, Is.EqualTo(ExistScenePath));
        }

        [TestCase("Packages/**/NotInScenesInBuild.unity")]
        [TestCase("**/NotInScenesInBuild.unity")]
        [TestCase("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild")]
        [TestCase("Packages/com.nowsprinting.test-helper/Tests/Scenes/Not??ScenesInBuild.unity")]
        [TestCase("Packages/com.nowsprinting.test-helper/Tests/Scenes/*InScenesInBuild.unity")]
        public void GetExistScenePath_InvalidGlobPattern_ThrowsArgumentException(string path)
        {
            Assert.That(() => SceneManagerHelper.GetExistScenePath(path, null), Throws.TypeOf<ArgumentException>());
        }

        [TestCase("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotExistScene.unity")] // Not exist path
        [TestCase("Packages/com.nowsprinting.test-helper/*/NotInScenesInBuild.unity")] // Not match path pattern
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor)]
        public void GetExistScenePath_NotExistPath_InEditor_ThrowsFileNotFoundException(string path)
        {
            Assert.That(() => SceneManagerHelper.GetExistScenePath(path, null), Throws.TypeOf<FileNotFoundException>());
            // Note: Returns scene name when running on player.
        }

        [TestCase("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild.unity")]
        public void GetExistScenePathOnPlayer_GotSceneName(string path)
        {
            const string SceneName = "NotInScenesInBuild";

            var actual = SceneManagerHelper.GetExistScenePathOnPlayer(path);
            Assert.That(actual, Is.EqualTo(SceneName));
        }
    }
}
