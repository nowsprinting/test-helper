// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
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
        private static readonly Dictionary<string, int> s_counter = new Dictionary<string, int>();

        /// <summary>
        /// Reset counter for same test names.
        /// </summary>
        /// <remarks>
        /// This method is called from the <see cref="TestHelper.Editor.TestRunnerCallbacks.RunStarted"/> method.
        /// Depending on the Enter Play Mode Settings, it may not be cleared in Play Mode and may be used by Edit Mode tests.
        /// </remarks>
        internal static void ResetCounter()
        {
            s_counter.Clear();
        }

        /// <summary>
        /// Creates a temporary file path.
        /// By default, the path is named by the test name in the directory pointed to by <see cref="Application.temporaryCachePath"/>.
        /// <p/>
        /// Replace included special characters in parameterized tests.
        /// Adds repeat counts.
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
            if (TestContext.CurrentTestExecutionContext != null)
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
            string name;
            if (TestContext.CurrentTestExecutionContext != null)
            {
                name = TestContext.CurrentContext.Test.Name
                    .Replace('(', '_')
                    .Replace(')', '_')
                    .Replace(',', '-')
                    .Replace("\"", "")
                    .Replace("/", "")
                    .Replace("\\", "")
                    .Replace("¥", "")
                    .Replace(":", "")
                    .Replace(";", "");
                // Note: Similar to the file name created under ActualImages in the Graphics Test Framework package.
            }
            else
            {
                name = callerMemberName;
            }

            if (s_counter.TryGetValue(name, out var count))
            {
                s_counter[name] = ++count;
                return $"{name.TrimEnd('_')}_{count}";
            }

            s_counter[name] = 0;
            return name;
        }
#endif

        /// <summary>
        /// Resolves a relative path from the caller file to Unity path format (Assets/ or Packages/).
        /// </summary>
        /// <param name="relativePath">Relative path from caller file</param>
        /// <param name="callerFilePath">Caller file path</param>
        /// <returns>Unity path format (e.g., "Assets/...", "Packages/...")</returns>
        internal static string ResolveUnityPath(string relativePath, string callerFilePath)
        {
            var callerDirectory = Path.GetDirectoryName(callerFilePath);
            // ReSharper disable once AssignNullToNotNullAttribute
            var absolutePath = Path.GetFullPath(Path.Combine(callerDirectory, relativePath));

            // First, look for Assets/ (ensure it's a directory, not part of another name)
            var assetsIndexOf =
                absolutePath.IndexOf($"{Path.DirectorySeparatorChar}Assets{Path.DirectorySeparatorChar}",
                    StringComparison.Ordinal);
            if (assetsIndexOf >= 0)
            {
                return ConvertToUnixPathSeparator(absolutePath.Substring(assetsIndexOf + 1));
            }

            // Next, look for Packages/ (ensure it's a directory, not part of another name like "LocalPackages")
            var packageIndexOf =
                absolutePath.IndexOf($"{Path.DirectorySeparatorChar}Packages{Path.DirectorySeparatorChar}",
                    StringComparison.Ordinal);
            if (packageIndexOf >= 0)
            {
                return ConvertToUnixPathSeparator(absolutePath.Substring(packageIndexOf + 1));
            }

            // If neither found, it might be an Embedded package
            // Find project root and convert to Packages/{packageName}/ format
            var projectRoot = FindProjectRoot(absolutePath);
            if (projectRoot != null)
            {
                var relativeFromRoot = absolutePath.Substring(projectRoot.Length)
                    .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                var unityPath = ConvertToUnityPath(relativeFromRoot, projectRoot);
                if (unityPath != null)
                {
                    return unityPath;
                }
            }

            Debug.LogError($"Can not resolve absolute path. relative: {relativePath}, caller: {callerFilePath}");
            return null;
            // Note: Do not use Exception (and Assert). Because freezes async tests on UTF v1.3.4, See UUM-25085.

            string ConvertToUnixPathSeparator(string path)
            {
                return path.Replace('\\', '/');
            }
        }

        private static string FindProjectRoot(string absolutePath)
        {
            var directory = Path.GetDirectoryName(absolutePath);
            while (!string.IsNullOrEmpty(directory))
            {
                // Project root has Assets/ directory or Packages/manifest.json
                if (Directory.Exists(Path.Combine(directory, "Assets")) ||
                    File.Exists(Path.Combine(directory, "Packages", "manifest.json")))
                {
                    return directory;
                }

                directory = Path.GetDirectoryName(directory);
            }

            return null;
        }

        private static string ConvertToUnityPath(string relativePathFromRoot, string projectRoot)
        {
            relativePathFromRoot = relativePathFromRoot.Replace('\\', '/');
            var segments = relativePathFromRoot.Split('/');
            if (segments.Length < 2)
            {
                return null;
            }

            // Search for package.json at any depth (from deep to shallow)
            for (var depth = segments.Length - 1; depth >= 1; depth--)
            {
                var packageDirSegments = new string[depth];
                Array.Copy(segments, 0, packageDirSegments, 0, depth);
                var packageDirRelative = string.Join("/", packageDirSegments);

                // Use absolute path for File.Exists check
                var packageJsonPath = Path.Combine(projectRoot, packageDirRelative, "package.json");

                if (File.Exists(packageJsonPath))
                {
                    var packageName = GetPackageNameFromJson(packageJsonPath);
                    if (!string.IsNullOrEmpty(packageName))
                    {
                        var remainingSegments = new string[segments.Length - depth];
                        Array.Copy(segments, depth, remainingSegments, 0, remainingSegments.Length);
                        return "Packages/" + packageName + "/" + string.Join("/", remainingSegments);
                    }
                }
            }

            return null;
        }

        private static string GetPackageNameFromJson(string packageJsonPath)
        {
            try
            {
                var json = File.ReadAllText(packageJsonPath);
                // Simple JSON parsing to extract "name": "..."
                var nameIndex = json.IndexOf("\"name\"", StringComparison.Ordinal);
                if (nameIndex < 0)
                {
                    return null;
                }

                var colonIndex = json.IndexOf(":", nameIndex, StringComparison.Ordinal);
                if (colonIndex < 0)
                {
                    return null;
                }

                var openQuoteIndex = json.IndexOf("\"", colonIndex, StringComparison.Ordinal);
                if (openQuoteIndex < 0)
                {
                    return null;
                }

                var closeQuoteIndex = json.IndexOf("\"", openQuoteIndex + 1, StringComparison.Ordinal);
                if (closeQuoteIndex < 0)
                {
                    return null;
                }

                return json.Substring(openQuoteIndex + 1, closeQuoteIndex - openQuoteIndex - 1);
            }
            catch
            {
                return null;
            }
        }
    }
}
