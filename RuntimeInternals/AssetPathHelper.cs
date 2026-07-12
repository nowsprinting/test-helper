// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.IO;
using UnityEngine;

namespace TestHelper.RuntimeInternals
{
    /// <summary>
    /// Helper class to convert a relative path based on the caller's file location into a Unity asset path
    /// (path starting with `Assets/` or `Packages/`).
    /// </summary>
    internal static class AssetPathHelper
    {
        /// <summary>
        /// Convert a relative path based on the caller's file location into a Unity asset path.
        /// </summary>
        /// <param name="relativePath">Relative path from the caller's file location (e.g., `../Scenes/Scene.unity`)</param>
        /// <param name="callerFilePath">Caller's file path set by <c>CallerFilePathAttribute</c></param>
        /// <returns>Unity asset path starting with `Assets/` or `Packages/`. Returns null if it can not be resolved.</returns>
        internal static string GetAssetPath(string relativePath, string callerFilePath)
        {
            var callerDirectory = Path.GetDirectoryName(callerFilePath);
            // ReSharper disable once AssignNullToNotNullAttribute
            var absolutePath = Path.GetFullPath(Path.Combine(callerDirectory, relativePath));

#if UNITY_EDITOR
            var resolvedPath = ResolveByProjectLayout(absolutePath, callerDirectory);
            if (resolvedPath != null)
            {
                return resolvedPath;
            }
#endif

            // Fallback: naive substring search for the "Assets"/"Packages" path segment.
            // This can not resolve packages placed outside the project (e.g., local packages referenced by
            // `file:`, and packages compiled from Library/PackageCache) because their real paths contain no
            // such segment, but it is kept for the run on player: source files do not exist on the device,
            // so the filesystem-based resolution above is not available there.
            var assetsIndexOf = absolutePath.IndexOf("Assets", StringComparison.Ordinal);
            if (assetsIndexOf > 0)
            {
                return ConvertToUnixPathSeparator(absolutePath.Substring(assetsIndexOf));
            }

            var packageIndexOf = absolutePath.IndexOf("Packages", StringComparison.Ordinal);
            if (packageIndexOf > 0)
            {
                return ConvertToUnixPathSeparator(absolutePath.Substring(packageIndexOf));
            }

            Debug.LogError($"Can not resolve absolute path. relative: {relativePath}, caller: {callerFilePath}");
            return null;
            // Note: Do not use Exception (and Assert). Because freezes async tests on UTF v1.3.4, See UUM-25085.
        }

        private static string ConvertToUnixPathSeparator(string path)
        {
            return path.Replace('\\', '/'); // Normalize path separator
        }

#if UNITY_EDITOR
        /// <summary>
        /// Resolve a Unity asset path from the real filesystem layout: a path under the project's Assets
        /// folder, or a path under a UPM package root (the directory containing package.json).
        /// <p/>
        /// Why not <c>UnityEditor.PackageManager.PackageInfo</c>: <c>FindForAssetPath</c> accepts only Unity
        /// asset paths (circular for this purpose), <c>GetAllRegisteredPackages</c> requires Unity 2021.1+,
        /// and <c>Client.List</c> is asynchronous. Walking up to package.json works on all supported Unity
        /// versions and is agnostic to symlink differences between the compiler-baked caller path and the
        /// Package Manager's resolved path.
        /// </summary>
        /// <returns>Unity asset path starting with `Assets/` or `Packages/`. Returns null if it can not be resolved.</returns>
        private static string ResolveByProjectLayout(string absolutePath, string callerDirectory)
        {
            var normalizedAbsolutePath = ConvertToUnixPathSeparator(absolutePath);

            // Check Assets before the package.json walk; Asset Store plugins may ship package.json under Assets.
            // Note: Application.dataPath is usable here because all editor call sites run on the main thread.
            var assetsRoot = ConvertToUnixPathSeparator(Path.GetFullPath(Application.dataPath));
            if (normalizedAbsolutePath.StartsWith(assetsRoot + "/", StringComparison.OrdinalIgnoreCase))
            {
                return "Assets" + normalizedAbsolutePath.Substring(assetsRoot.Length);
            }

            if (!Directory.Exists(callerDirectory))
            {
                // Synthetic caller paths (e.g., specified in unit tests) fall through to the fallback;
                // walking up from a non-existent directory could find an unrelated package.json in an
                // ancestor directory (e.g., a npm monorepo root above the project).
                return null;
            }

            var directory = new DirectoryInfo(Path.GetFullPath(callerDirectory));
            while (directory != null)
            {
                var packageName = GetUpmPackageName(directory.FullName);
                var packageRoot = ConvertToUnixPathSeparator(directory.FullName);
                if (packageName != null &&
                    normalizedAbsolutePath.StartsWith(packageRoot + "/", StringComparison.Ordinal))
                {
                    return "Packages/" + packageName + normalizedAbsolutePath.Substring(packageRoot.Length);
                }

                directory = directory.Parent;
            }

            return null;
        }

        private static string GetUpmPackageName(string directory)
        {
            var packageJsonPath = Path.Combine(directory, "package.json");
            if (!File.Exists(packageJsonPath))
            {
                return null;
            }

            try
            {
                var manifest = JsonUtility.FromJson<PackageManifest>(File.ReadAllText(packageJsonPath));
                if (manifest == null || string.IsNullOrEmpty(manifest.name))
                {
                    // A nameless package.json (e.g., a vendored npm manifest inside the package) is not a
                    // UPM package root; skip it and continue walking up.
                    return null;
                }

                return manifest.name;
            }
            catch (Exception)
            {
                return null; // Malformed package.json; skip it and continue walking up.
            }
        }

        [Serializable]
        private class PackageManifest
        {
            // ReSharper disable once InconsistentNaming
            public string name;
        }
#endif
    }
}
