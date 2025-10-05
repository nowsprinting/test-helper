// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
#endif

namespace TestHelper.RuntimeInternals
{
    public static class TemporaryFileHelper
    {
        /// <summary>
        /// Creates a temporary file path.
        /// </summary>
        /// <param name="baseDirectory">If omitted, use <see cref="Application.temporaryCachePath"/>.</param>
        /// <param name="extension">File extension if necessary.</param>
        /// <param name="namespaceToDirectory">Insert subdirectory named from test namespace if true.</param>
        /// <param name="createDirectory">Create directory if true.</param>
        /// <param name="deleteIfExists">Delete existing file if true.</param>
        /// <param name="callerMemberName">The name of the calling method to use when called outside a test context.</param>
        /// <returns></returns>
        public static string CreateTemporaryFilePath(
            string baseDirectory = null,
            string extension = null,
            bool namespaceToDirectory = false,
            bool createDirectory = true,
            bool deleteIfExists = true,
            [CallerMemberName] string callerMemberName = null)
        {
            var directory = baseDirectory != null ? Path.GetFullPath(baseDirectory) : Application.temporaryCachePath;

            if (namespaceToDirectory)
            {
                directory = Path.Join(directory, GetSubdirectoryFromNamespace());
            }

            if (createDirectory)
            {
                Directory.CreateDirectory(directory);
            }

            extension = extension != null ? $".{extension.TrimStart('.')}" : string.Empty;
            var path = Path.Join(directory, GetFilename(callerMemberName) + extension);

            if (deleteIfExists)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }

            return path;
        }

        private static string GetSubdirectoryFromNamespace()
        {
#if UNITY_INCLUDE_TESTS
            if (TestContext.CurrentTestExecutionContext != null)
            {
                return TestContext.CurrentTestExecutionContext.CurrentTest.FullName
                    .Replace(TestContext.CurrentTestExecutionContext.CurrentTest.Name, "")
                    .Replace('.', Path.DirectorySeparatorChar);
            }
#endif
            return string.Empty;
        }

        private static string GetFilename(string callerMemberName)
        {
#if UNITY_INCLUDE_TESTS
            if (TestContext.CurrentTestExecutionContext != null)
            {
                return TestContext.CurrentTestExecutionContext.CurrentTest.Name
                    .Replace('(', '_')
                    .Replace(')', '_')
                    .Replace(',', '-')
                    .Replace('.', '-')
                    .Replace("\"", "");
                // Note: Similar to the file name created under ActualImages in the Graphics Test Framework package, but also replace the period.
            }
#endif
            return callerMemberName;
        }
    }
}
