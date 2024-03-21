// Copyright (c) 2023-2024 Koji Hasegawa.
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
    }
}
