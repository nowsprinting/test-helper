// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace TestHelper.RuntimeInternals
{
    /// <summary>
    /// Manage command line arguments for test-helper.
    /// </summary>
    public static class CommandLineArgs
    {
        internal static string[] CachedCommandLineArgs = Environment.GetCommandLineArgs();

        /// <summary>
        /// Screenshot save directory.
        /// Returns <c>Application.persistentDataPath</c> + "/TestHelper/Screenshots/" if not specified.
        /// </summary>
        /// <returns></returns>
        public static string GetScreenshotDirectory()
        {
            const string ScreenshotDirectoryKey = "-testHelperScreenshotDirectory=";

            var arg = CachedCommandLineArgs.FirstOrDefault(x => x.StartsWith(ScreenshotDirectoryKey));
            if (arg != null)
            {
                return arg.Substring(ScreenshotDirectoryKey.Length);
            }
            else
            {
                return Path.Combine(Application.persistentDataPath, "TestHelper", "Screenshots");
            }
        }
    }
}
