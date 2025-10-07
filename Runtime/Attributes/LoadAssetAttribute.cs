// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TestHelper.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class LoadAssetAttribute : Attribute
    {
        internal string AssetPath { get; private set; }

        /// <summary>
        /// Load an asset file at the specified path into the field.
        /// Tests that use this attribute must call the <see cref="LoadAssets"/> static method from the <c>OneTimeSetUp</c> method.
        /// <p/>
        /// This attribute has the following benefits:
        /// <list type="bullet">
        ///     <item>The same code can be used for Edit Mode tests and Play Mode tests in Editor and on Player.</item>
        ///     <item>The asset file path can be specified as a relative path from the test class file.</item>
        /// </list>
        /// </summary>
        /// <param name="path">Asset file path.
        /// The path must starts with `Assets/` or `Packages/` or `.`.
        /// And package name using `name` instead of `displayName`, when asset file in the package (e.g., `Packages/com.nowsprinting.test-helper/Tests/Scenes/Scene.unity`).
        /// </param>
        /// <param name="callerFilePath">Test file path set by <see cref="CallerFilePathAttribute"/></param>
        /// <remarks>
        /// When running tests on the player, it temporarily copies asset files to the <c>Resources</c> folder by <see cref="TestHelper.Editor.TemporaryCopyAssetsForPlayer"/>.
        /// </remarks>
        public LoadAssetAttribute(string path, [CallerFilePath] string callerFilePath = null)
        {
            if (path.StartsWith("."))
            {
                AssetPath = GetAbsolutePath(path, callerFilePath);
            }
            else
            {
                AssetPath = path;
            }
        }

        internal static string GetAbsolutePath(string relativePath, string callerFilePath)
        {
            var callerDirectory = Path.GetDirectoryName(callerFilePath);
            // ReSharper disable once AssignNullToNotNullAttribute
            var absolutePath = Path.GetFullPath(Path.Combine(callerDirectory, relativePath));

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

            string ConvertToUnixPathSeparator(string path)
            {
                return path.Replace('\\', '/'); // Normalize path separator
            }
        }

        /// <summary>
        /// Loads the asset file specified by <see cref="LoadAssetAttribute"/> and sets it to the field.
        /// It is recommended to call this method from the <c>OneTimeSetUp</c> method.
        /// e.g.,
        /// <code>
        /// [OneTimeSetUp]
        /// public void OneTimeSetUp()
        /// {
        ///     LoadAssetAttribute.LoadAssets(this);
        /// }
        /// </code>
        /// </summary>
        public static void LoadAssets(object testClassInstance)
        {
            var type = testClassInstance.GetType();
            var fields = type.GetFields(
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.Static);
            foreach (var field in fields)
            {
                var attribute = GetCustomAttribute(field, typeof(LoadAssetAttribute)) as LoadAssetAttribute;
                if (attribute == null)
                {
                    continue;
                }
#if UNITY_EDITOR
                var asset = AssetDatabase.LoadAssetAtPath(attribute.AssetPath, field.FieldType);
#else
                var resourcePath = Path.Combine(
                    Path.GetDirectoryName(attribute.AssetPath) ?? string.Empty,
                    Path.GetFileNameWithoutExtension(attribute.AssetPath));
                var asset = Resources.Load(resourcePath, field.FieldType);
#endif
                if (asset == null)
                {
                    Debug.LogError($"Failed to load asset at path: {attribute.AssetPath} type: {field.FieldType}");
                    continue;
                }

                field.SetValue(testClassInstance, asset);
            }
        }
    }
}
