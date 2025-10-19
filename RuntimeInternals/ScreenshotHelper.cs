// Copyright (c) 2023-2025 Koji Hasegawa.
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
        /// <summary>
        /// Take a screenshot and save it to file.
        /// </summary>
        /// <remarks>
        /// Limitations:
        /// <list type="bullet">
        ///     <item>Do not call form Edit Mode tests.</item>
        ///     <item>Must be called from main thread.</item>
        ///     <item><c>GameView</c> must be visible. Use <c>FocusGameViewAttribute</c> or <c>GameViewResolutionAttribute</c> if running on batch mode.</item>
        ///     <item><c>UniTask</c> is required to be used from the async method. And also needs coroutineRunner (any <c>MonoBehaviour</c>) because <c>TakeScreenshot</c> method uses <c>WaitForEndOfFrame</c> inside. See more information: <see href="https://github.com/Cysharp/UniTask#ienumeratortounitask-limitation"/></item>
        /// </list>
        /// </remarks>
        /// <param name="directory">Directory to save screenshots.
        /// If omitted, the directory specified by command line argument "-testHelperScreenshotDirectory" is used.
        /// If the command line argument is also omitted, <c>Application.persistentDataPath</c> + "/TestHelper/Screenshots/" is used.</param>
        /// <param name="filename">Filename to store screenshot.
        /// If omitted, default filename is <c>TestContext.Test.Name</c> + ".png" when run in test context.
        /// Using <see cref="callerMemberName"/> when called outside a test context.</param>
        /// <param name="superSize">The factor to increase resolution with.</param>
        /// <param name="stereoCaptureMode">The eye texture to capture when stereo rendering is enabled.</param>
        /// <param name="logFilepath">Output filename to Debug.Log</param>
        /// <param name="namespaceToDirectory">Insert subdirectory named from test namespace if true.</param>
        /// <param name="callerMemberName">Used as the default file name when called outside a test context</param>
        public static IEnumerator TakeScreenshot(
            string directory = null,
            string filename = null,
            int superSize = 1,
            ScreenCapture.StereoScreenCaptureMode stereoCaptureMode = ScreenCapture.StereoScreenCaptureMode.LeftEye,
            bool logFilepath = true,
            bool namespaceToDirectory = false,
            [CallerMemberName] string callerMemberName = null)
        {
            if (superSize != 1 && stereoCaptureMode != ScreenCapture.StereoScreenCaptureMode.LeftEye)
            {
                Debug.LogWarning("superSize and stereoCaptureMode cannot be specified at the same time.");
                yield break;
            }

            var path = GetSavePath(directory, filename, namespaceToDirectory, callerMemberName);

            yield return new WaitForEndOfFrame(); // Required to take screenshots

            var texture = superSize != 1
                ? ScreenCapture.CaptureScreenshotAsTexture(superSize)
                : ScreenCapture.CaptureScreenshotAsTexture(stereoCaptureMode);

            var bytes = texture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);

            Object.Destroy(texture);

            if (logFilepath)
            {
                Debug.Log($"Save screenshot to {path}");
            }

#if UNITY_INCLUDE_TESTS
            if (TestContext.CurrentContext != null)
            {
                var properties = TestContext.CurrentContext.Test.Properties;
                properties.Add("Screenshot", path);
            }
#endif
        }

#if UNITY_2023_1_OR_NEWER
        /// <summary>
        /// Take a screenshot and save it to file.
        /// </summary>
        /// <remarks>
        /// Limitations:
        /// <list type="bullet">
        ///     <item>Do not call form Edit Mode tests.</item>
        ///     <item>Must be called from main thread.</item>
        ///     <item><c>GameView</c> must be visible. Use <c>FocusGameViewAttribute</c> or <c>GameViewResolutionAttribute</c> if running on batch mode.</item>
        /// </list>
        /// </remarks>
        /// <param name="directory">Directory to save screenshots.
        /// If omitted, the directory specified by command line argument "-testHelperScreenshotDirectory" is used.
        /// If the command line argument is also omitted, <c>Application.persistentDataPath</c> + "/TestHelper/Screenshots/" is used.</param>
        /// <param name="filename">Filename to store screenshot.
        /// If omitted, default filename is <c>TestContext.Test.Name</c> + ".png" when run in test context.
        /// Using <see cref="callerMemberName"/> when called outside a test context.</param>
        /// <param name="superSize">The factor to increase resolution with.</param>
        /// <param name="stereoCaptureMode">The eye texture to capture when stereo rendering is enabled.</param>
        /// <param name="namespaceToDirectory">Insert subdirectory named from test namespace if true.</param>
        /// <param name="callerMemberName">Used as the default file name when called outside a test context</param>
        public static async Awaitable TakeScreenshotAsync(
            string directory = null,
            string filename = null,
            int superSize = 1,
            ScreenCapture.StereoScreenCaptureMode stereoCaptureMode = ScreenCapture.StereoScreenCaptureMode.LeftEye,
            bool namespaceToDirectory = false,
            [CallerMemberName] string callerMemberName = null)
        {
            if (superSize != 1 && stereoCaptureMode != ScreenCapture.StereoScreenCaptureMode.LeftEye)
            {
                Debug.LogWarning("superSize and stereoCaptureMode cannot be specified at the same time.");
                return;
            }

            var path = GetSavePath(directory, filename, namespaceToDirectory, callerMemberName);

            await Awaitable.EndOfFrameAsync(); // Required to take screenshots

            var texture = superSize != 1
                ? ScreenCapture.CaptureScreenshotAsTexture(superSize)
                : ScreenCapture.CaptureScreenshotAsTexture(stereoCaptureMode);

            var bytes = texture.EncodeToPNG();
            await File.WriteAllBytesAsync(path, bytes);

            Object.Destroy(texture);

#if UNITY_INCLUDE_TESTS
            if (TestContext.CurrentContext != null)
            {
                var properties = TestContext.CurrentContext.Test.Properties;
                properties.Add("Screenshot", path);
            }
#endif
        }
#endif

        private static string GetSavePath(string directory, string filename, bool namespaceToDirectory,
            string callerMemberName)
        {
            if (directory != null)
            {
                directory = Path.GetFullPath(directory);
            }
            else
            {
                directory = CommandLineArgs.GetScreenshotDirectory();
            }

            Directory.CreateDirectory(directory);

            string path;
            if (filename == null)
            {
                path = PathHelper.CreateFilePath(
                    baseDirectory: directory,
                    extension: "png",
                    namespaceToDirectory: namespaceToDirectory,
                    callerMemberName: callerMemberName);
            }
            else
            {
                path = Path.Combine(directory, filename);
                if (!path.EndsWith(".png"))
                {
                    path += ".png";
                }
            }

            return path;
        }
    }
}
