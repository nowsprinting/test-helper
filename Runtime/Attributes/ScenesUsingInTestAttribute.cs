// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using NUnit.Framework;

namespace TestHelper.Attributes
{
    /// <summary>
    /// Temporarily build scene files not added to "Scenes in Build" when running a test.
    /// It has the following effects:
    ///  - Can specify only scene name to `SceneManager.LoadScene()` method
    ///  - Can load in `SceneManager.LoadScene()` method when running a test on a standalone player.
    ///
    /// This attribute is effective for the entire test run, not individual tests.
    /// So, I recommend specifying it at the assembly level.
    /// </summary>
    /// <remarks>
    /// The name is "Scene*s*" to like a "Scenes in Build".
    /// The argument is a single string, but can attach multiple this attribute.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class ScenesUsingInTestAttribute : NUnitAttribute
    {
        internal string ScenePath { get; private set; }

        /// <summary>
        /// Specify scene file path to temporarily build when running a test.
        /// </summary>
        /// <param name="scenePath">Scene path not in "Scenes in Build".
        /// The scene file path starts with `Assets/` or `Packages/`, and ends with `.unity`.
        /// Use `name` instead of `displayName` in package paths.
        /// </param>
        public ScenesUsingInTestAttribute(string scenePath)
        {
            ScenePath = scenePath;
        }
    }
}
