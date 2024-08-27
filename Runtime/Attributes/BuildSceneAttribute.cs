// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace TestHelper.Attributes
{
    /// <summary>
    /// Build scene before running test on player.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class BuildSceneAttribute : NUnitAttribute
    {
        internal string ScenePath { get; private set; }
        internal string CallerFilePath { get; private set; }

        /// <summary>
        /// Build scene before running test on player.
        /// This attribute has the following benefits:
        /// - Can be specified scenes that are **NOT** in "Scenes in Build".
        /// - Can be specified scene path by [glob](https://en.wikipedia.org/wiki/Glob_(programming)) pattern. However, there are restrictions, top level and scene name cannot be omitted.
        /// - Can be specified scene path by relative path from the test class file.
        /// </summary>
        /// <param name="path">Scene file path. (Optional)
        /// The path starts with `Assets/` or `Packages/` or `.`.
        /// And package name using `name` instead of `displayName`, when scenes in the package.
        /// (e.g., `Packages/com.nowsprinting.test-helper/Tests/Scenes/Scene.unity`)
        ///
        /// If the value is omitted, the scene name will be derived from the test file name .
        /// (e.g., `Asset/Tests/ScreenshotTest.cs` will load `Asset/Tests/ScreenshotTest.unity`)
        /// </param>
        /// <remarks>
        /// - For the process of including a Scene not in "Scenes in Build" to a build for player, see: <see cref="TestHelper.Editor.TemporaryBuildScenesUsingInTest"/>.
        /// </remarks>
        [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
        public BuildSceneAttribute(string path = null, [CallerFilePath] string callerFilePath = null)
        {
            ScenePath = ResolveScenePath(path, callerFilePath);
            CallerFilePath = callerFilePath;
        }
        
        /// <summary>
        /// Get matching scene of the callerFilePath
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callerFilePath"></param>
        static string ResolveScenePath(string path, string callerFilePath)
        {
            if (!string.IsNullOrEmpty(path))
                return path;
            return $"./{Path.GetFileNameWithoutExtension(callerFilePath)}.unity";
        }
    }
}
