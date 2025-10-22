// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

#if ENABLE_GRAPHICS_TEST_FRAMEWORK
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Graphics;

namespace TestHelper.RuntimeInternals.TestUtils
{
    internal static class VrtUtils
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static void ConvertTexture2dFieldsToARGB32(object testClassInstance)
        {
            var type = testClassInstance.GetType();
            var fields = type.GetFields(
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.Static);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(Texture2D))
                {
                    var src = (Texture2D)field.GetValue(testClassInstance);
                    if (src.format != TextureFormat.ARGB32)
                    {
                        var pixels = src.GetPixels32();
                        var dst = new Texture2D(src.width, src.height, TextureFormat.ARGB32, false);
                        dst.SetPixels32(pixels);
                        dst.Apply();
                        field.SetValue(testClassInstance, dst);
                    }
                }
            }
        }

        public static Texture2D LoadExpectedImage()
        {
            var path = Path.Combine(
                "Packages/com.nowsprinting.test-helper/Tests/Images",
                $"{TestContext.CurrentContext.Test.Name}.png");
            return LoadImage(path);
        }

        public static Texture2D LoadImage(string path)
        {
            var bytes = File.ReadAllBytes(Path.GetFullPath(path));
            var texture = new Texture2D(0, 0);
            texture.LoadImage(bytes); // load png as ARGB32 format
            return texture;
        }

        public static ImageComparisonSettings GetImageComparisonSettings()
        {
            return new ImageComparisonSettings
            {
                TargetWidth = Screen.width,
                TargetHeight = Screen.height,
                AverageCorrectnessThreshold = 0.0005f,
            };
        }
    }
}
#endif
