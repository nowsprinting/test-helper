// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
using System.IO;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace TestHelper.Utils
{
    /// <summary>
    /// Helper class for taking a screenshots.
    /// </summary>
    public static class ScreenshotHelper
    {
        private static string DefaultDirectoryPath()
        {
            return Path.Combine(Application.persistentDataPath, "TestHelper", "Screenshots");
        }

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
        /// <param name="directory">Directory to save screenshots relative to project path. Only effective in Editor.</param>
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
            if (superSize != 1 && stereoCaptureMode != ScreenCapture.StereoScreenCaptureMode.LeftEye)
            {
                Debug.LogError("superSize and stereoCaptureMode cannot be specified at the same time.");
                yield break;
            }

            if (Thread.CurrentThread.ManagedThreadId != 1)
            {
                Debug.LogError("Must be called from the main thread.");
                yield break;
                // Note: This is not the case since it is a coroutine.
            }

            if (Application.isEditor && directory != null)
            {
                directory = Path.GetFullPath(directory);
            }
            else
            {
                directory = DefaultDirectoryPath(); // Not apply specific directory when running on player
            }

            Directory.CreateDirectory(directory);
            filename ??= DefaultFilename();

            yield return new WaitForEndOfFrame(); // Required to take screenshots

            var texture = superSize != 1
                ? ScreenCapture.CaptureScreenshotAsTexture(superSize)
                : ScreenCapture.CaptureScreenshotAsTexture(stereoCaptureMode);

            var path = Path.Combine(directory, filename);
            var bytes = texture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            Debug.Log($"Save screenshot to {path}");
        }
    }
}
