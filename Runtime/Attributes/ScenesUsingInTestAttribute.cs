// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using NUnit.Framework;

namespace TestHelper.Attributes
{
    /// <summary>
    /// Temporarily add scene files to "Scenes in Build" when running a play mode tests.
    /// 
    /// It has the following benefits:
    ///  - Can specify scene by name to `SceneManager.LoadScene()` method
    ///  - Can load scenes when running a test on a standalone player
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
        /// Specify scene file or directory path to temporarily add to "Scenes in Build" when running a play mode test.
        /// </summary>
        /// <param name="scenePath">Scene file or directory path not in "Scenes in Build".
        /// The path starts with `Assets/` or `Packages/`.
        /// And use `name` instead of `displayName` of the package, when scenes in the package.
        /// </param>
        public ScenesUsingInTestAttribute(string scenePath)
        {
            ScenePath = scenePath;
        }
    }
}
