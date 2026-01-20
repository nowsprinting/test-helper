// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TestHelper.Attributes;
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

        private static IEnumerable<(LoadAssetAttribute attribute, string originalPath)>
            FindAttributesOnFieldsWithOriginalPath()
        {
#if UNITY_2020_1_OR_NEWER
            var fields = TypeCache.GetFieldsWithAttribute<LoadAssetAttribute>();
            foreach (var field in fields)
            {
                var attribute = field.GetCustomAttribute<LoadAssetAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                var attrData = field.GetCustomAttributesData()
                    .FirstOrDefault(a => a.AttributeType == typeof(LoadAssetAttribute));
                var originalPath = attrData?.ConstructorArguments[0].Value as string;

                yield return (attribute, originalPath);
            }
#else
            foreach (var field in AppDomain.CurrentDomain.GetAssemblies()
                         .SelectMany(x => x.GetTypes())
                         .SelectMany(x => x.GetFields(
                             BindingFlags.Public | BindingFlags.NonPublic |
                             BindingFlags.Instance | BindingFlags.Static)))
            {
                var attribute = field.GetCustomAttribute<LoadAssetAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                var attrData = field.GetCustomAttributesData()
                    .FirstOrDefault(a => a.AttributeType == typeof(LoadAssetAttribute));
                var originalPath = attrData?.ConstructorArguments[0].Value as string;

                yield return (attribute, originalPath);
            }
#endif
        }

        internal static void CopyAssetFiles()
        {
            foreach (var (attribute, originalPath) in FindAttributesOnFieldsWithOriginalPath())
            {
                var realPath = CalculateRealPath(attribute, originalPath);
                var destFileName = Path.Combine(ResourcesRoot, "Resources", realPath);
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

        private static string CalculateRealPath(LoadAssetAttribute attribute, string originalPath)
        {
            if (originalPath != null && originalPath.StartsWith("."))
            {
                var callerFilePath = attribute.CallerFilePath;
                var callerDirectory = Path.GetDirectoryName(callerFilePath);

                // Use Uri to resolve relative paths
                var baseUri = new Uri($"file:///{callerDirectory?.Replace('\\', '/')}/");
                var relativeUri = new Uri(baseUri, originalPath);
                var resolvedPath = relativeUri.LocalPath.TrimStart('/').Replace('\\', '/');

                // Get project root
                var projectRoot = Path.GetFullPath(Directory.GetCurrentDirectory()).Replace('\\', '/');
                if (!projectRoot.EndsWith("/"))
                {
                    projectRoot += "/";
                }

                // Convert to relative path from project root if it's absolute
                if (resolvedPath.Contains(":") && resolvedPath.StartsWith(projectRoot))
                {
                    return resolvedPath.Substring(projectRoot.Length);
                }

                // Already a relative path
                return resolvedPath;
            }

            // Fallback: use originalPath if available, otherwise use AssetPath
            return originalPath ?? attribute.AssetPath;
        }
    }
}
