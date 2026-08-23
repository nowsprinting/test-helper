// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using TestHelper.Attributes;
using TestHelper.RuntimeInternals;

namespace TestHelper.Editor
{
    /// <summary>
    /// Build <see cref="AssetRootMapping"/> from the caller file paths baked into attributes at compile time.
    /// The physical roots are derived by walking up from each caller file path to the innermost
    /// <c>package.json</c> with a valid <c>name</c>, so the mapping keys are the same strings the compiler
    /// baked into the attributes (not the Package Manager's resolved paths, which can differ via symlinks).
    /// </summary>
    internal static class AssetRootMappingBuilder
    {
        /// <summary>
        /// Collect caller file paths from <c>LoadAssetAttribute</c> fields and <c>BuildSceneAttribute</c>
        /// (assemblies/types/methods, including derived <c>LoadSceneAttribute</c>), then delegate to
        /// <see cref="CreateFromCallerFilePaths"/>.
        /// </summary>
        internal static AssetRootMapping Build()
        {
            var callerFilePaths = new List<string>();
            foreach (var attribute in AttributeFinder.FindOnFields<LoadAssetAttribute>())
            {
                callerFilePaths.Add(attribute.CallerFilePath);
            }

            foreach (var attribute in AttributeFinder.FindOnAssemblies<BuildSceneAttribute>())
            {
                callerFilePaths.Add(attribute.CallerFilePath);
            }

            foreach (var attribute in AttributeFinder.FindOnTypes<BuildSceneAttribute>())
            {
                callerFilePaths.Add(attribute.CallerFilePath);
            }

            foreach (var attribute in AttributeFinder.FindOnMethods<BuildSceneAttribute>())
            {
                callerFilePaths.Add(attribute.CallerFilePath);
            }

            return CreateFromCallerFilePaths(callerFilePaths);
        }

        /// <summary>
        /// Create a mapping from caller file paths.
        /// Always contains the entry for the project's Assets folder. Adds one entry per distinct UPM package
        /// root found by walking up from each caller file path; paths under the Assets folder and paths whose
        /// package root can not be found (e.g., synthetic or deleted caller paths) are skipped.
        /// </summary>
        internal static AssetRootMapping CreateFromCallerFilePaths(IEnumerable<string> callerFilePaths)
        {
            var mapping = new AssetRootMapping();
            var assetsRoot = AssetPathHelper.GetAssetsRootPath();
            mapping.AddEntry(assetsRoot, "Assets");

            // Many attributes share the same directory; walk up to the package root only once per directory
            // (each walk step reads and parses a package.json candidate).
            var processedCallerDirectories = new HashSet<string>();

            foreach (var callerFilePath in callerFilePaths)
            {
                if (string.IsNullOrEmpty(callerFilePath))
                {
                    continue;
                }

                var callerDirectory = Path.GetDirectoryName(callerFilePath);
                if (string.IsNullOrEmpty(callerDirectory))
                {
                    continue;
                }

                var normalizedCallerDirectory =
                    AssetPathHelper.ConvertToUnixPathSeparator(Path.GetFullPath(callerDirectory));
                if (!processedCallerDirectories.Add(normalizedCallerDirectory))
                {
                    continue;
                }

                // Skip paths under the Assets folder: they are already covered by the Assets entry, and
                // walking up from inside the project could find an unrelated package.json above the project
                // (e.g., a npm monorepo root).
                if (normalizedCallerDirectory.StartsWith(assetsRoot + "/", StringComparison.OrdinalIgnoreCase) ||
                    normalizedCallerDirectory.Equals(assetsRoot, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (AssetPathHelper.TryGetPackageRoot(callerDirectory, out var physicalRoot, out var assetRoot))
                {
                    mapping.AddEntry(physicalRoot, assetRoot);
                }
            }

            return mapping;
        }
    }
}
