// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.IO;
using NUnit.Framework;

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
            const string ExistScenePath = "Packages/com.nowsprinting.test-helper/Tests/Scenes/NotInScenesInBuild.unity";

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
        public void GetExistScenePath_NotExistPath_ThrowsFileNotFoundException(string path)
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
