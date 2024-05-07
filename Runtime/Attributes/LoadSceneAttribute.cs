// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using NUnit.Framework.Interfaces;
using TestHelper.RuntimeInternals;
using UnityEngine.TestTools;

namespace TestHelper.Attributes
{
    /// <summary>
    /// Load scene before running test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class LoadSceneAttribute : BuildSceneAttribute, IOuterUnityTestAction
    {
        /// <summary>
        /// Load scene before running test.
        /// This attribute has the following benefits:
        /// - Can be use same code for running Edit Mode tests, Play Mode tests in Editor, and on Player.
        /// - Can be specified scenes that are **NOT** in "Scenes in Build".
        /// - Can be specified scene path by [glob](https://en.wikipedia.org/wiki/Glob_(programming)) pattern. However, there are restrictions, top level and scene name cannot be omitted.
        /// - Can be specified scene path by relative path from the test class file.
        /// </summary>
        /// <param name="path">Scene file path.
        /// The path starts with `Assets/` or `Packages/` or `.`.
        /// And package name using `name` instead of `displayName`, when scenes in the package.
        /// (e.g., `Packages/com.nowsprinting.test-helper/Tests/Scenes/Scene.unity`)
        /// </param>
        /// <remarks>
        /// - Load scene run after <c>OneTimeSetUp</c> and before <c>SetUp</c>. If you want to setup before loading Use <see cref="BuildSceneAttribute"/> and <see cref="SceneManagerHelper.LoadSceneAsync"/> instead.
        /// - For the process of including a Scene not in "Scenes in Build" to a build for player, see: <see cref="TestHelper.Editor.TemporaryBuildScenesUsingInTest"/>.
        /// </remarks>
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
        public LoadSceneAttribute(string path, [CallerFilePath] string callerFilePath = null)
            : base(path, callerFilePath)
        {
        }

        /// <inheritdoc />
        public IEnumerator BeforeTest(ITest test)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            yield return SceneManagerHelper.LoadSceneAsync(ScenePath, CallerFilePath);
        }

        /// <inheritdoc />
        public IEnumerator AfterTest(ITest test)
        {
            yield return null;
        }
    }
}
