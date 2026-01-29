// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using TestHelper.RuntimeInternals;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TestHelper.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class LoadAssetAttribute : Attribute
    {
#if UNITY_EDITOR
        internal string AssetPath { get; private set; }
#endif
        internal string ResourcePath { get; private set; }

        /// <summary>
        /// Loads an asset file at the specified path into the field.
        /// Tests that use this attribute must call the <see cref="LoadAssets"/> static method from the <c>OneTimeSetUp</c> method.
        /// <p/>
        /// This attribute has the following benefits:
        /// <list type="bullet">
        ///     <item>The same code can be used for Edit Mode tests and Play Mode tests in Editor and on Player.</item>
        ///     <item>The asset file path can be specified as a relative path from the test class file.</item>
        /// </list>
        /// <p/>
        /// Loads asset with <see cref="AssetDatabase.LoadAssetAtPath(string,Type)"/> in the editor, and <see cref="Resources.Load(string,Type)"/> on the player.
        /// Asset settings such as image format will conform to the .meta file.
        /// </summary>
        /// <param name="path">Asset file path.
        /// The path must start with `Assets/` or `Packages/` or `.`.
        /// And package name using `name` instead of `displayName`, when asset file is in the package
        /// (e.g., `Packages/com.nowsprinting.test-helper/Tests/Scenes/Scene.unity`).
        /// </param>
        /// <param name="callerFilePath">Test file path set by <see cref="CallerFilePathAttribute"/></param>
        /// <remarks>
        /// When running tests on the player, it temporarily copies asset files to the <c>Resources</c> folder by <see cref="Editor.TemporaryCopyAssetsForPlayer"/>.
        /// </remarks>
        public LoadAssetAttribute(string path, [CallerFilePath] string callerFilePath = null)
        {
#if UNITY_EDITOR
            if (path != null && path.StartsWith("."))
            {
                AssetPath = PathHelper.ResolveUnityPath(path, callerFilePath);
            }
            else
            {
                AssetPath = PathHelper.ResolveUnityPath(path);
            }
#endif
            if (path != null && path.StartsWith("."))
            {
                ResourcePath = BuildResourcePath(path, callerFilePath);
            }
            else
            {
                ResourcePath = BuildResourcePath(path);
            }
        }

        private static string BuildResourcePath(string relativePath, string callerFilePath)
        {
            var callerDirectory = Path.GetDirectoryName(callerFilePath);
            var absolutePath = Path.GetFullPath(Path.Combine(callerDirectory!, relativePath));

            var assetsIndexOf = absolutePath.IndexOf(
                $"{Path.DirectorySeparatorChar}Assets{Path.DirectorySeparatorChar}",
                StringComparison.Ordinal);
            if (assetsIndexOf >= 0)
            {
                return BuildResourcePath(absolutePath.Substring(assetsIndexOf + 1));
            }

            // Next, look for Packages/ (ensure it's a directory, not part of another name like "LocalPackages")
            var packageIndexOf =
                absolutePath.IndexOf($"{Path.DirectorySeparatorChar}Packages{Path.DirectorySeparatorChar}",
                    StringComparison.Ordinal);
            if (packageIndexOf >= 0)
            {
                return BuildResourcePath(absolutePath.Substring(packageIndexOf + 1));
            }

            return BuildResourcePath(absolutePath);
        }

        private static string BuildResourcePath(string absolutePath)
        {
            // Convert to resource path (remove extension)
            var directoryName = Path.Join("com.nowsprinting.test-helper", Path.GetDirectoryName(absolutePath)); // TODO: Path.Join
            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(absolutePath);
            return string.IsNullOrEmpty(directoryName)
                ? filenameWithoutExtension
                : $"{directoryName.Replace('\\', '/')}/{filenameWithoutExtension}";
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
                if (asset == null)
                {
                    Debug.LogError($"Failed to load asset at path: {attribute.AssetPath} type: {field.FieldType}");
                    continue;
                }
#else
                var asset = Resources.Load(attribute.ResourcePath, field.FieldType);
                if (asset == null)
                {
                    Debug.LogError($"Failed to load asset at path: {attribute.ResourcePath} type: {field.FieldType}");
                    continue;
                }
#endif
                field.SetValue(testClassInstance, asset);
            }
        }
    }
}
