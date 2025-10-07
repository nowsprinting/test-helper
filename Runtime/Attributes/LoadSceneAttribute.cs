// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using NUnit.Framework.Interfaces;
using TestHelper.RuntimeInternals;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TestHelper.Attributes
{
    /// <summary>
    /// Load a scene before running this test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class LoadSceneAttribute : BuildSceneAttribute, IOuterUnityTestAction
    {
        /// <summary>
        /// Load a scene before running this test.
        /// <p/>
        /// This process runs after <c>OneTimeSetUp</c> and before <c>SetUp</c>.
        /// If you want to load during <c>SetUp</c> and testing, use <see cref="BuildSceneAttribute"/> and <see cref="SceneManagerHelper.LoadSceneAsync"/> instead.
        /// <p/>
        /// This attribute has the following benefits:
        /// <list type="bullet">
        ///     <item>The same code can be used for Edit Mode tests and Play Mode tests in Editor and on Player.</item>
        ///     <item>Scene that are **NOT** in "Scenes in Build" can be specified.</item>
        ///     <item>The scene file path can be specified as a relative path from the test class file.</item>
        /// </list>
        /// </summary>
        /// <param name="path">Scene file path (optional).
        /// The path must starts with `Assets/` or `Packages/` or `.`.
        /// And package name using `name` instead of `displayName`, when scene file in the package
        /// (e.g., `Packages/com.nowsprinting.test-helper/Tests/Scenes/Scene.unity`).
        /// If the value is omitted, the scene name will be derived from the test file name
        /// (e.g., `Asset/Tests/ScreenshotTest.cs` will load `Asset/Tests/ScreenshotTest.unity`).
        /// </param>
        /// <param name="callerFilePath">Test file path set by <see cref="CallerFilePathAttribute"/></param>
        /// <remarks>
        /// For the process of including a Scene not in "Scenes in Build" to a build for player, see: <see cref="TestHelper.Editor.TemporaryBuildScenesUsingInTest"/>.
        /// </remarks>
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public LoadSceneAttribute(string path = null, [CallerFilePath] string callerFilePath = null)
            : base(path, callerFilePath)
        {
        }

        /// <inheritdoc />
        public IEnumerator BeforeTest(ITest test)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            yield return SceneManagerHelper.LoadSceneAsync(ScenePath, LoadSceneMode.Single, LocalPhysicsMode.None,
                CallerFilePath);
        }

        /// <inheritdoc />
        public IEnumerator AfterTest(ITest test)
        {
            yield return null;
        }
    }
}
