// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

#if ENABLE_GRAPHICS_TEST_FRAMEWORK
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Graphics;

namespace TestHelper.RuntimeInternals.TestUtils
{
    internal static class VrtUtils
    {
        public static Texture2D LoadExpectedImage()
        {
            var path = Path.Combine(
                "Packages/com.nowsprinting.test-helper/Tests/VRT/ExpectedImages",
                $"{TestContext.CurrentContext.Test.Name}.png");
            return LoadImage(path);
        }

        public static Texture2D LoadImage(string path)
        {
            var bytes = File.ReadAllBytes(Path.GetFullPath(path));
            var texture = new Texture2D(Screen.width, Screen.height);
            texture.LoadImage(bytes);
            return texture;
        }

        public static ImageComparisonSettings CreateImageComparisonSettings()
        {
            return new ImageComparisonSettings
            {
                TargetWidth = Screen.width,
                TargetHeight = Screen.height,
                AverageCorrectnessThreshold = 0.0003f,
            };
        }
    }
}
#endif
