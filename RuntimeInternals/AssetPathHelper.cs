// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
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
        /// Loader of the asset-root mapping used on the player. Injection seam: tests replace it with a stub
        /// returning fixed entries and restore it afterward.
        /// </summary>
        internal static IAssetRootMappingLoader MappingLoader { get; set; } =
            new ResourcesAssetRootMappingLoader();

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
#else
            // On the player, source files do not exist on the device, so the filesystem-based resolution can
            // not work. Instead, use the asset-root mapping written at player-build time by
            // TestHelper.Editor.TemporaryCopyAssetsForPlayer.
            var mapping = MappingLoader != null ? MappingLoader.Load() : null;
            if (mapping != null)
            {
                var mappedPath = mapping.Resolve(absolutePath);
                if (mappedPath != null)
                {
                    return mappedPath;
                }
            }
#endif

            // Fallback: naive substring search for the "Assets"/"Packages" path segment.
            // This can not resolve packages placed outside the project (e.g., local packages referenced by
            // `file:`, and packages compiled from Library/PackageCache) because their real paths contain no
            // such segment, but it is kept for synthetic caller paths and for the run on player without the
            // asset-root mapping.
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

        internal static string ConvertToUnixPathSeparator(string path)
        {
            return path.Replace('\\', '/'); // Normalize path separator
        }

#if UNITY_EDITOR
        /// <summary>
        /// Get the project's Assets folder as a forward-slash full path.
        /// Note: Application.dataPath is usable here because all editor call sites run on the main thread.
        /// </summary>
        internal static string GetAssetsRootPath()
        {
            return ConvertToUnixPathSeparator(Path.GetFullPath(Application.dataPath));
        }

        /// <summary>
        /// Get the innermost UPM package root (the directory containing a package.json with a valid `name`)
        /// containing <paramref name="callerDirectory"/>.
        /// </summary>
        /// <param name="callerDirectory">Directory of a caller file path baked by <c>CallerFilePathAttribute</c></param>
        /// <param name="physicalRoot">Package root as a forward-slash full path without a trailing slash,
        /// walked up from the caller path string as-is (preserves the compiler-baked root)</param>
        /// <param name="assetRoot">Unity asset root path in `Packages/&lt;name&gt;` format</param>
        /// <returns>false if <paramref name="callerDirectory"/> does not exist (synthetic caller paths guard)
        /// or no package root is found.</returns>
        internal static bool TryGetPackageRoot(string callerDirectory, out string physicalRoot,
            out string assetRoot)
        {
            foreach (var candidate in EnumeratePackageRootCandidates(callerDirectory))
            {
                physicalRoot = candidate.PhysicalRoot;
                assetRoot = "Packages/" + candidate.PackageName;
                return true;
            }

            physicalRoot = null;
            assetRoot = null;
            return false;
        }

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
            var assetsRoot = GetAssetsRootPath();
            if (normalizedAbsolutePath.StartsWith(assetsRoot + "/", StringComparison.OrdinalIgnoreCase))
            {
                return "Assets" + normalizedAbsolutePath.Substring(assetsRoot.Length);
            }

            foreach (var candidate in EnumeratePackageRootCandidates(callerDirectory))
            {
                // Prefix guard: skip package roots that do not contain the target (the relative path escapes
                // this package); an outer package root may still contain it.
                if (normalizedAbsolutePath.StartsWith(candidate.PhysicalRoot + "/", StringComparison.Ordinal))
                {
                    return "Packages/" + candidate.PackageName +
                           normalizedAbsolutePath.Substring(candidate.PhysicalRoot.Length);
                }
            }

            return null;
        }

        /// <summary>
        /// Enumerate UPM package root candidates (directories containing a package.json with a valid `name`)
        /// from <paramref name="callerDirectory"/> upward, innermost first.
        /// </summary>
        private static IEnumerable<(string PhysicalRoot, string PackageName)> EnumeratePackageRootCandidates(
            string callerDirectory)
        {
            if (!Directory.Exists(callerDirectory))
            {
                // Synthetic caller paths (e.g., specified in unit tests) get no candidates; walking up from a
                // non-existent directory could find an unrelated package.json in an ancestor directory
                // (e.g., a npm monorepo root above the project).
                yield break;
            }

            var directory = new DirectoryInfo(Path.GetFullPath(callerDirectory));
            while (directory != null)
            {
                var packageName = GetUpmPackageName(directory.FullName);
                if (packageName != null)
                {
                    yield return (ConvertToUnixPathSeparator(directory.FullName), packageName);
                }

                directory = directory.Parent;
            }
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
