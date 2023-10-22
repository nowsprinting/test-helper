// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// ReSharper disable InvalidXmlDocComment

namespace TestHelper.Attributes
{
    /// <summary>
    /// Load scene before running test.
    ///
    /// It has the following benefits:
    ///  - Can be used when running play mode tests in-editor and on-player
    ///  - Can be specified scenes that are not in "Scenes in Build"
    ///
    /// Notes:
    ///  - Load scene run after <c>OneTimeSetUp</c> and before <c>SetUp</c>
    ///  - For the process of including a Scene not in "Scenes in Build" to a build for player, see: <see cref="Editor.TemporaryBuildScenesUsingInTest"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class LoadSceneAttribute : NUnitAttribute, IOuterUnityTestAction
    {
        internal string ScenePath { get; private set; }

        /// <summary>
        /// Load scene before running test.
        /// </summary>
        /// <param name="path">Scene file path.
        /// The path starts with `Assets/` or `Packages/`.
        /// And package name using `name` instead of `displayName`, when scenes in the package.
        /// (e.g., `Packages/com.nowsprinting.test-helper/Tests/Scenes/Scene.unity`)
        /// </param>
        public LoadSceneAttribute(string path)
        {
            ScenePath = path;
        }

        /// <inheritdoc />
        public IEnumerator BeforeTest(ITest test)
        {
            AsyncOperation loadSceneAsync = null;

            if (Application.isEditor)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    // Play Mode tests running in Editor
                    loadSceneAsync = EditorSceneManager.LoadSceneAsyncInPlayMode(
                        ScenePath,
                        new LoadSceneParameters(LoadSceneMode.Single));
                }
                else
                {
                    // Edit Mode tests
                    EditorSceneManager.OpenScene(ScenePath);
                }
#endif
            }
            else
            {
                // Play Mode tests running on Player
                loadSceneAsync = SceneManager.LoadSceneAsync(ScenePath);
            }

            yield return loadSceneAsync;
        }

        /// <inheritdoc />
        public IEnumerator AfterTest(ITest test)
        {
            yield return null;
        }
    }
}
