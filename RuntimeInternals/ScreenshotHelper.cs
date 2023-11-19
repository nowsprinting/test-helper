// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
#endif

namespace TestHelper.RuntimeInternals
{
    /// <summary>
    /// Helper class for taking a screenshots.
    /// This class can be used from the runtime code because it does not depend on test-framework.
    /// </summary>
    public static class ScreenshotHelper
    {
        private static string DefaultDirectoryPath()
        {
            return Path.Combine(Application.persistentDataPath, "TestHelper", "Screenshots");
        }

        private static string DefaultFilename(string callerMemberName)
        {
#if UNITY_INCLUDE_TESTS
            return TestContext.CurrentTestExecutionContext.CurrentTest.Name
                .Replace('(', '_')
                .Replace(')', '_')
                .Replace(',', '-');
            // Note: Same as the file name created under ActualImages of the Graphics Tests Framework package.
#else
            return callerMemberName;
#endif
        }

        /// <summary>
        /// Take a screenshot and save it to file.
        /// Default save path is <c>Application.persistentDataPath</c> + "/TestHelper/Screenshots/" + <c>CurrentTest.Name</c> + ".png".
        /// </summary>
        /// <remarks>
        /// Limitations:
        ///  - Do not call from Edit Mode tests.
        ///  - Must be called from main thread.
        ///  - <c>GameView</c> must be visible. Use <c>FocusGameViewAttribute</c> or <c>GameViewResolutionAttribute</c> if running on batch mode.
        ///  - Files with the same name will be overwritten. Please specify filename argument when calling over twice in one method.
        ///  - UniTask is required to be used from the async method. And also needs coroutineRunner (any MonoBehaviour) because TakeScreenshot method uses WaitForEndOfFrame inside. See more information: https://github.com/Cysharp/UniTask#ienumeratortounitask-limitation
        /// <br/>
        /// Using <c>ScreenCapture.CaptureScreenshotAsTexture</c> internally.
        /// </remarks>
        /// <param name="directory">Directory to save screenshots. Default save path is <c>Application.persistentDataPath</c> + "/TestHelper/Screenshots/".</param>
        /// <param name="filename">Filename to store screenshot.
        /// Default filename is <c>CurrentTest.Name</c> + ".png" when run in test-framework context.
        /// Using caller method name when run in runtime context.</param>
        /// <param name="superSize">The factor to increase resolution with.</param>
        /// <param name="stereoCaptureMode">The eye texture to capture when stereo rendering is enabled.</param>
        public static IEnumerator TakeScreenshot(
            string directory = null,
            string filename = null,
            int superSize = 1,
            ScreenCapture.StereoScreenCaptureMode stereoCaptureMode = ScreenCapture.StereoScreenCaptureMode.LeftEye,
            // ReSharper disable once InvalidXmlDocComment
            [CallerMemberName] string callerMemberName = null)
        {
            if (superSize != 1 && stereoCaptureMode != ScreenCapture.StereoScreenCaptureMode.LeftEye)
            {
                Debug.LogWarning("superSize and stereoCaptureMode cannot be specified at the same time.");
                yield break;
            }

            if (Thread.CurrentThread.ManagedThreadId != 1)
            {
                Debug.LogWarning("Must be called from the main thread.");
                yield break;
                // Note: This is not the case since it is a coroutine.
            }

            if (directory != null)
            {
                directory = Path.GetFullPath(directory);
            }
            else
            {
                directory = DefaultDirectoryPath(); // Not apply specific directory when running on player
            }

            Directory.CreateDirectory(directory);

            if (filename == null)
            {
                filename = DefaultFilename(callerMemberName);
            }

            if (!filename.EndsWith(".png"))
            {
                filename += ".png";
            }

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
