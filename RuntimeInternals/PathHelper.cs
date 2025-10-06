// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
#endif

namespace TestHelper.RuntimeInternals
{
    public static class PathHelper
    {
        /// <summary>
        /// Creates a temporary file path.
        /// By default, the path is named by the test name in the directory pointed to by <see cref="Application.temporaryCachePath"/>.
        /// <p/>
        /// Replace included special characters in parameterized tests.
        /// Adds repeat counts by command-line arguments to the filename (required test-framework v1.3.5 or newer).
        /// </summary>
        /// <param name="extension">File extension if necessary.</param>
        /// <param name="namespaceToDirectory">Insert subdirectory named from test namespace if true.</param>
        /// <param name="createDirectory">Create directory if true.</param>
        /// <param name="deleteIfExists">Delete existing file if true.</param>
        /// <param name="callerMemberName">The name of the calling method to use when called outside a test context.</param>
        /// <returns>Temporary file path in running tests.</returns>
        public static string CreateTemporaryFilePath(
            string extension = null,
            bool namespaceToDirectory = false,
            bool createDirectory = true,
            bool deleteIfExists = true,
            [CallerMemberName] string callerMemberName = null)
        {
            return CreateFilePath(
                baseDirectory: Application.temporaryCachePath,
                extension: extension,
                namespaceToDirectory: namespaceToDirectory,
                createDirectory: createDirectory,
                deleteIfExists: deleteIfExists,
                callerMemberName: callerMemberName);
        }

        internal static string CreateFilePath(
            string baseDirectory,
            string extension = null,
            bool namespaceToDirectory = false,
            bool createDirectory = true,
            bool deleteIfExists = true,
            [CallerMemberName] string callerMemberName = null)
        {
            var directory = Path.GetFullPath(baseDirectory);

#if UNITY_INCLUDE_TESTS
            if (namespaceToDirectory)
            {
                directory = Path.Combine(directory, GetSubdirectoryFromNamespace());
            }
#endif

            if (createDirectory)
            {
                Directory.CreateDirectory(directory);
            }

            extension = extension != null ? $".{extension.TrimStart('.')}" : string.Empty;
#if UNITY_INCLUDE_TESTS
            var path = Path.Combine(directory, GetFilename(callerMemberName) + extension);
#else
            var path = Path.Combine(directory, callerMemberName + extension);
#endif

            if (deleteIfExists)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }

            return path;
        }

#if UNITY_INCLUDE_TESTS
        private static string GetSubdirectoryFromNamespace()
        {
            if (TestContext.CurrentContext != null)
            {
                var fullName = TestContext.CurrentContext.Test.FullName;
                var testName = TestContext.CurrentContext.Test.Name;
                var testNameIndex = fullName.LastIndexOf(testName, StringComparison.Ordinal);
                return fullName.Substring(0, testNameIndex).Replace('.', Path.DirectorySeparatorChar);
            }

            return string.Empty;
        }

        private static string GetFilename(string callerMemberName)
        {
            if (TestContext.CurrentContext != null)
            {
                var name = TestContext.CurrentContext.Test.Name
                    .Replace('(', '_')
                    .Replace(')', '_')
                    .Replace(',', '-')
                    .Replace('.', '-')
                    .Replace("\"", "");
                // Note: Similar to the file name created under ActualImages in the Graphics Test Framework package, but also replace the period.

                var iteration = GetIntProperty("repeatIteration") + GetIntProperty("retryIteration");
                // Note: "retryIteration" is not incremented during execution, so it is always 0 (in test-framework v1.5.1)
                if (iteration > 0)
                {
                    return $"{name.TrimEnd('_')}_{iteration}";
                }

                return name;
            }

            return callerMemberName;
        }

        private static int GetIntProperty(string key)
        {
            var properties = TestContext.CurrentContext.Test.Properties;
            var value = properties.Get(key);
            if (value != null && value is int i)
            {
                return i;
            }

            return 0;
        }
#endif
    }
}
