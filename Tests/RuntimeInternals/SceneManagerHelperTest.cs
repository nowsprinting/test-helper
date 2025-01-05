// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System.Text.RegularExpressions;
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
        public async Task LoadSceneAsync_LoadedScene(string path)
        {
            await SceneManagerHelper.LoadSceneAsync(path);
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

        [TestCase("**/NotInScenesInBuild.unity", "Scene path must start with `Assets/` or `Packages/`")]
        [TestCase("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild",
            "Scene path must ends with `.unity`")]
        [TestCase("Packages/**/NotInScenesInBuild.unity", "Wildcards cannot be used in the package name of path")]
        [TestCase("Packages/com.nowsprinting.test-helper/Tests/Scenes/Not??ScenesInBuild.unity",
            "Wildcards cannot be used in the scene name of path")]
        [TestCase("Packages/com.nowsprinting.test-helper/Tests/Scenes/*InScenesInBuild.unity",
            "Wildcards cannot be used in the scene name of path")]
        public void GetExistScenePath_InvalidGlobPattern_OutputLogError(string path, string expected)
        {
            var actual = SceneManagerHelper.GetExistScenePath(path, null);

            Assert.That(actual, Is.Null);
            LogAssert.Expect(LogType.Error, new Regex(expected));
        }

        [TestCase("Packages/com.nowsprinting.test-helper/Tests/Scenes/NotExistScene.unity")] // Not exist path
        [TestCase("Packages/com.nowsprinting.test-helper/*/NotInScenesInBuild.unity")] // Not match path pattern
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor)]
        // Note: Returns scene name when running on player.
        public void GetExistScenePath_NotExistPath_InEditor_OutputLogError(string path)
        {
            var actual = SceneManagerHelper.GetExistScenePath(path, null);

            Assert.That(actual, Is.Null);
            LogAssert.Expect(LogType.Error, $"Scene `{path}` is not found in AssetDatabase");
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
