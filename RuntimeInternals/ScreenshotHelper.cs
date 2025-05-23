// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
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
        private static string DefaultDirectoryPath => CommandLineArgs.GetScreenshotDirectory();

        internal static string DefaultFilename(string callerMemberName)
        {
#if UNITY_INCLUDE_TESTS
            if (TestContext.CurrentTestExecutionContext != null)
            {
                return TestContext.CurrentTestExecutionContext.CurrentTest.Name
                    .Replace('(', '_')
                    .Replace(')', '_')
                    .Replace(',', '-')
                    .Replace("\"", "");
                // Note: Same as the file name created under ActualImages of the Graphics Tests Framework package.
            }
#endif
            return callerMemberName;
        }

        /// <summary>
        /// Take a screenshot and save it to file.
        /// Default save path is <c>Application.persistentDataPath</c> + "/TestHelper/Screenshots/" + <c>CurrentTest.Name</c> + ".png".
        /// </summary>
        /// <remarks>
        /// Limitations:
        /// <list type="bullet">
        ///     <item>Do not attach to Edit Mode tests.</item>
        ///     <item>Must be called from main thread.</item>
        ///     <item><c>GameView</c> must be visible. Use <c>FocusGameViewAttribute</c> or <c>GameViewResolutionAttribute</c> if running on batch mode.</item>
        ///     <item>Files with the same name will be overwritten. Please specify filename argument when calling over twice in one method.</item>
        ///     <item><c>UniTask</c> is required to be used from the async method. And also needs coroutineRunner (any <c>MonoBehaviour</c>) because <c>TakeScreenshot</c> method uses <c>WaitForEndOfFrame</c> inside. See more information: <see href="https://github.com/Cysharp/UniTask#ienumeratortounitask-limitation"/></item>
        /// </list>
        /// <br/>
        /// Using <c>ScreenCapture.CaptureScreenshotAsTexture</c> internally.
        /// </remarks>
        /// <param name="directory">Directory to save screenshots.
        /// If omitted, the directory specified by command line argument "-testHelperScreenshotDirectory" is used.
        /// If the command line argument is also omitted, <c>Application.persistentDataPath</c> + "/TestHelper/Screenshots/" is used.</param>
        /// <param name="filename">Filename to store screenshot.
        /// Default filename is <c>CurrentTest.Name</c> + ".png" when run in test-framework context.
        /// Using caller method name when run in runtime context.</param>
        /// <param name="superSize">The factor to increase resolution with.</param>
        /// <param name="stereoCaptureMode">The eye texture to capture when stereo rendering is enabled.</param>
        /// <param name="logFilepath">Output filename to Debug.Log</param>
        public static IEnumerator TakeScreenshot(
            string directory = null,
            string filename = null,
            int superSize = 1,
            ScreenCapture.StereoScreenCaptureMode stereoCaptureMode = ScreenCapture.StereoScreenCaptureMode.LeftEye,
            bool logFilepath = true,
            // ReSharper disable once InvalidXmlDocComment
            [CallerMemberName] string callerMemberName = null)
        {
            if (superSize != 1 && stereoCaptureMode != ScreenCapture.StereoScreenCaptureMode.LeftEye)
            {
                Debug.LogWarning("superSize and stereoCaptureMode cannot be specified at the same time.");
                yield break;
            }

            if (directory != null)
            {
                directory = Path.GetFullPath(directory);
            }
            else
            {
                directory = DefaultDirectoryPath; // Not apply specific directory when running on player
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

            if (logFilepath)
            {
                Debug.Log($"Save screenshot to {path}");
            }
        }
    }
}
