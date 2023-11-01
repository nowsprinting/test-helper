// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;

namespace TestHelper.Utils
{
    /// <summary>
    /// Helper class for taking a screenshots.
    /// </summary>
    [Obsolete("Use RuntimeInternals.ScreenshotHelper instead.")]
    public static class ScreenshotHelper
    {
        private static string DefaultFilename()
        {
            return $"{TestContext.CurrentTestExecutionContext.CurrentTest.Name}.png"
                .Replace('(', '_')
                .Replace(')', '_')
                .Replace(',', '-');
            // Note: Same as the file name created under ActualImages of the Graphics Tests Framework package.
        }

        /// <summary>
        /// Take a screenshot and save it to file.
        /// Default save path is $"{Application.persistentDataPath}/TestHelper/Screenshots/{CurrentTest.Name}.png".
        /// </summary>
        /// <remarks>
        /// Limitations:
        ///  - Do not call from Edit Mode tests.
        ///  - Must be called from main thread.
        ///  - <c>GameView</c> must be visible. Use <c>FocusGameViewAttribute</c> or <c>GameViewResolutionAttribute</c> if running on batch mode.
        ///  - Files with the same name will be overwritten. Please specify filename argument when calling over twice in one method.
        /// <br/>
        /// Using <c>ScreenCapture.CaptureScreenshotAsTexture</c> internally.
        /// </remarks>
        /// <param name="directory">Directory to save screenshots.</param>
        /// <param name="filename">Filename to store screenshot.</param>
        /// <param name="superSize">The factor to increase resolution with.</param>
        /// <param name="stereoCaptureMode">The eye texture to capture when stereo rendering is enabled.</param>
        public static IEnumerator TakeScreenshot(
            string directory = null,
            string filename = null,
            int superSize = 1,
            ScreenCapture.StereoScreenCaptureMode stereoCaptureMode = ScreenCapture.StereoScreenCaptureMode.LeftEye
        )
        {
            if (filename == null)
            {
                filename = DefaultFilename();
            }

            yield return RuntimeInternals.ScreenshotHelper.TakeScreenshot(
                directory, filename, superSize, stereoCaptureMode);
        }
    }
}
