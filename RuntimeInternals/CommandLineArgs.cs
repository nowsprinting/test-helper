// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TestHelper.RuntimeInternals
{
    /// <summary>
    /// Manage command line arguments for test-helper.
    /// </summary>
    public static class CommandLineArgs
    {
        internal static Dictionary<string, string> DictionaryFromCommandLineArgs(string[] args)
        {
            var result = new Dictionary<string, string>();
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-"))
                {
                    var key = args[i];
                    var value = string.Empty;
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    {
                        value = args[i + 1];
                        i++;
                    }

                    result[key] = value;
                }
            }

            return result;
        }

        /// <summary>
        /// Screenshot save directory.
        /// Returns <c>Application.persistentDataPath</c> + "/TestHelper/Screenshots/" if not specified.
        /// </summary>
        /// <returns></returns>
        public static string GetScreenshotDirectory(string[] args = null)
        {
            const string ScreenshotDirectoryKey = "-testHelperScreenshotDirectory";

            try
            {
                args = args ?? Environment.GetCommandLineArgs();
                return DictionaryFromCommandLineArgs(args)[ScreenshotDirectoryKey];
            }
            catch (KeyNotFoundException)
            {
                return Path.Combine(Application.persistentDataPath, "TestHelper", "Screenshots");
            }
        }

        /// <summary>
        /// Statistics output directory.
        /// Returns <c>Application.persistentDataPath</c> + "/TestHelper/Statistics/" if not specified.
        /// </summary>
        /// <returns></returns>
        public static string GetStatisticsDirectory(string[] args = null)
        {
            const string StatisticsDirectoryKey = "-testHelperStatisticsDirectory";

            try
            {
                args = args ?? Environment.GetCommandLineArgs();
                return DictionaryFromCommandLineArgs(args)[StatisticsDirectoryKey];
            }
            catch (KeyNotFoundException)
            {
                return Path.Combine(Application.persistentDataPath, "TestHelper", "Statistics");
            }
        }

        /// <summary>
        /// JUnit XML report save path.
        /// </summary>
        /// <returns></returns>
        public static string GetJUnitResultsPath(string[] args = null)
        {
            const string JUnitResultsKey = "-testHelperJUnitResults";

            try
            {
                args = args ?? Environment.GetCommandLineArgs();
                return DictionaryFromCommandLineArgs(args)[JUnitResultsKey];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        /// <summary>
        /// <c>GameView</c> resolution name.
        /// </summary>
        /// <returns></returns>
        public static string GetGameViewResolutionName(string[] args = null)
        {
            const string ResolutionNameKey = "-testHelperGameViewResolution";

            try
            {
                args = args ?? Environment.GetCommandLineArgs();
                return DictionaryFromCommandLineArgs(args)[ResolutionNameKey];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        /// <summary>
        /// <c>GameView</c> width and height.
        /// </summary>
        /// <returns></returns>
        public static (uint width, uint height) GetGameViewResolutionSize(string[] args = null)
        {
            const string WidthKey = "-testHelperGameViewWidth";
            const string HeightKey = "-testHelperGameViewHeight";

            args = args ?? Environment.GetCommandLineArgs();

            var dict = DictionaryFromCommandLineArgs(args);
            if (dict.TryGetValue(WidthKey, out var widthStr) && dict.TryGetValue(HeightKey, out var heightStr))
            {
                if (uint.TryParse(widthStr, out var width) && uint.TryParse(heightStr, out var height))
                {
                    return (width, height);
                }
            }

            return (0, 0);
        }
    }
}
