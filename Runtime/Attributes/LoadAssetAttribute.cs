// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
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

        internal string CallerFilePath { get; private set; }

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
        /// When running tests on the player, it temporarily copies asset files to the <c>Resources</c> folder by <see cref="TestHelper.Editor.TemporaryCopyAssetsForPlayer"/>.
        /// </remarks>
        public LoadAssetAttribute(string path, [CallerFilePath] string callerFilePath = null)
        {
            CallerFilePath = callerFilePath;

            if (path.StartsWith("."))
            {
#if UNITY_EDITOR
                AssetPath = RuntimeInternals.PathHelper.ResolveUnityPath(path, callerFilePath);
#else
                // Will be resolved at runtime in GetResourcePath() for standalone player
                AssetPath = null;
#endif
            }
            else
            {
                AssetPath = path;
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
                var originalPath = GetOriginalPathFromAttribute(field);
                var resourcePath = GetResourcePath(attribute, originalPath);
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

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private static string GetOriginalPathFromAttribute(FieldInfo field)
        {
            var attrData = field.GetCustomAttributesData()
                .FirstOrDefault(a => a.AttributeType == typeof(LoadAssetAttribute));
            return attrData?.ConstructorArguments[0].Value as string;
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private static string GetResourcePath(LoadAssetAttribute attribute, string originalPath)
        {
            var callerFilePath = attribute.CallerFilePath;
            if (callerFilePath != null && originalPath != null && originalPath.StartsWith("."))
            {
                // Remove leading "./" from CallerFilePath if present
                callerFilePath = callerFilePath.TrimStart('.', '/', '\\');

                var callerDirectory = Path.GetDirectoryName(callerFilePath);

                // Use Uri to resolve relative paths without depending on file system
                var baseUri = new Uri($"file:///{callerDirectory?.Replace('\\', '/')}/");
                var relativeUri = new Uri(baseUri, originalPath);
                var realPath = relativeUri.LocalPath.TrimStart('/').Replace('\\', '/');

                // Convert to resource path (remove extension)
                var directoryName = Path.GetDirectoryName(realPath);
                var filenameWithoutExtension = Path.GetFileNameWithoutExtension(realPath);
                return string.IsNullOrEmpty(directoryName)
                    ? filenameWithoutExtension
                    : $"{directoryName.Replace('\\', '/')}/{filenameWithoutExtension}";
            }

            // Fallback to legacy behavior
            return GetLegacyResourcePath(attribute.AssetPath);
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private static string GetLegacyResourcePath(string assetPath)
        {
            if (assetPath == null)
            {
                return null;
            }

            var directoryName = Path.GetDirectoryName(assetPath);
            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(assetPath);
            return string.IsNullOrEmpty(directoryName)
                ? filenameWithoutExtension
                : $"{directoryName.Replace('\\', '/')}/{filenameWithoutExtension}";
        }
    }
}
