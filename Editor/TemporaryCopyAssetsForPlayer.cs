// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.IO;
using TestHelper.Attributes;
using TestHelper.RuntimeInternals;
using UnityEditor;
using UnityEngine;

namespace TestHelper.Editor
{
    /// <summary>
    /// Temporarily copy asset files specified by <c>LoadAssetAttribute</c> to the Resources folder when running play mode tests on player.
    /// </summary>
    /// <remarks>
    /// Deleting copied files is in <see cref="TestRunnerCallbacks.RunFinished"/> method.
    /// </remarks>
    internal static class TemporaryCopyAssetsForPlayer
    {
        internal const string ResourcesRoot = "Assets/com.nowsprinting.test-helper";

        /// <summary>
        /// Write the asset-root mapping file used by <c>AssetPathHelper</c> on the player.
        /// Always writes, even if only the Assets folder entry exists.
        /// </summary>
        /// <remarks>
        /// Deleting the written file is in <see cref="TestRunnerCallbacks.RunFinished"/> method
        /// (the whole <see cref="ResourcesRoot"/> folder is deleted).
        /// </remarks>
        internal static void WriteAssetRootMappingFile()
        {
            var mapping = AssetRootMappingBuilder.Build();
            var destFileName = PrepareDestFileName(AssetRootMapping.ResourcePath + ".json");
            File.WriteAllText(destFileName, JsonUtility.ToJson(mapping));
            AssetDatabase.ImportAsset(destFileName); // Make it a TextAsset before the player build.
        }

        internal static void CopyAssetFiles()
        {
            foreach (var attribute in AttributeFinder.FindOnFields<LoadAssetAttribute>())
            {
                var destFileName = PrepareDestFileName(attribute.AssetPath);
                if (!AssetDatabase.CopyAsset(attribute.AssetPath, destFileName))
                {
                    Debug.LogError($"Failed to copy asset file from '{attribute.AssetPath}' to '{destFileName}'");
                }
            }
        }

        /// <summary>
        /// Returns the destination file path under the temporary Resources folder, creating the parent
        /// directory if necessary.
        /// </summary>
        private static string PrepareDestFileName(string relativePath)
        {
            // Path.Combine emits the OS-native separator (backslash on Windows); AssetDatabase APIs
            // require Unity asset paths with forward slashes, so normalize before returning.
            var destFileName = AssetPathHelper.ConvertToUnixPathSeparator(
                Path.Combine(ResourcesRoot, "Resources", relativePath));
            var destDir = Path.GetDirectoryName(destFileName);
            if (destDir != null && !Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            return destFileName;
        }
    }
}
