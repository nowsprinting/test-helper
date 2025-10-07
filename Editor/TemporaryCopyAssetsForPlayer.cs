// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestHelper.Attributes;
using UnityEditor;

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

        private static IEnumerable<T> FindAttributesOnFields<T>() where T : Attribute
        {
            var symbols = TypeCache.GetFieldsWithAttribute<T>();
            foreach (var attribute in symbols
                         .Select(symbol => symbol.GetCustomAttributes(typeof(T), false))
                         .SelectMany(attributes => attributes))
            {
                yield return attribute as T;
            }
        }

        internal static void CopyAssetFiles()
        {
            foreach (var attribute in FindAttributesOnFields<LoadAssetAttribute>())
            {
                var destFileName = Path.Combine(ResourcesRoot, "Resources", attribute.AssetPath);
                var destDir = Path.GetDirectoryName(destFileName);
                if (destDir != null && !Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                File.Copy(attribute.AssetPath, destFileName, true);
            }
        }
    }
}
