// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System.Linq;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TestHelper.RuntimeInternals
{
    /// <summary>
    /// Temporary add test scene to "Scenes in Build" during the build for tests.
    /// What not use LoadSceneAttribute? because not depend on the TestHelper assembly.
    /// </summary>
    internal class ScreenshotHelperTestSceneBuilder : IPrebuildSetup, IPostBuildCleanup
    {
#if UNITY_EDITOR
        private EditorBuildSettingsScene[] _scenesInBuild;
#endif

        /// <inheritdoc />
        public void Setup()
        {
#if UNITY_EDITOR
            _scenesInBuild = EditorBuildSettings.scenes;

            var scenesList = _scenesInBuild.ToList();
            scenesList.Add(new EditorBuildSettingsScene(
                ScreenshotHelperTest.TestScene,
                true));

            EditorBuildSettings.scenes = scenesList.ToArray();
#endif
        }

        /// <inheritdoc />
        public void Cleanup()
        {
#if UNITY_EDITOR
            EditorBuildSettings.scenes = _scenesInBuild;
#endif
        }
    }
}
