// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestHelper.Attributes;
using UnityEditor;
using UnityEngine;
#if !UNITY_2020_1_OR_NEWER
using System.Reflection;
#endif

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
#if UNITY_2020_1_OR_NEWER
            var symbols = TypeCache.GetFieldsWithAttribute<T>();
            var attributes = symbols.SelectMany(symbol => symbol.GetCustomAttributes(typeof(T), false));
            foreach (var attribute in attributes)
            {
                yield return attribute as T;
            }
#else
            foreach (var attribute in AppDomain.CurrentDomain.GetAssemblies()
                         .SelectMany(x => x.GetTypes())
                         .SelectMany(x => x.GetFields(
                             BindingFlags.Public | BindingFlags.NonPublic |
                             BindingFlags.Instance | BindingFlags.Static))
                         .SelectMany(symbol => symbol.GetCustomAttributes(typeof(T), false)))
            {
                yield return attribute as T;
            }
#endif
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

                if (!AssetDatabase.CopyAsset(attribute.AssetPath, destFileName))
                {
                    Debug.LogError($"Failed to copy asset file from '{attribute.AssetPath}' to '{destFileName}'");
                }
            }
        }
    }
}
