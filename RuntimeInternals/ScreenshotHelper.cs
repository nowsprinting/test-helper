// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
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
        /// <p/>
        /// Limitations:
        /// <list type="bullet">
        ///     <item>Do not call from Edit Mode tests.</item>
        ///     <item>Must be called from main thread.</item>
        ///     <item><c>GameView</c> must be visible. Use the <c>FocusGameViewAttribute</c> or <c>GameViewResolutionAttribute</c> if running on batch mode.</item>
        ///     <item><c>UniTask</c> is required to be used from the async method. And also needs coroutineRunner (any <c>MonoBehaviour</c>) because this method uses <c>WaitForEndOfFrame</c> inside. See more information: <see href="https://github.com/Cysharp/UniTask#ienumeratortounitask-limitation"/></item>
        /// </list>
        /// </summary>
        /// <param name="directory">Directory to save screenshots.
        /// If omitted, the directory specified by command line argument "-testHelperScreenshotDirectory" is used.
        /// If the command line argument is also omitted, <c>Application.persistentDataPath</c> + "/TestHelper/Screenshots/" is used.</param>
        /// <param name="filename">Filename to store screenshot.
        /// If omitted, default filename is <c>TestContext.Test.Name</c> + ".png" when run in test context.
        /// Using <see cref="callerMemberName"/> when called outside a test context.</param>
        /// <param name="superSize">The factor to increase resolution with.</param>
        /// <param name="stereoCaptureMode">The eye texture to capture when stereo rendering is enabled.</param>
        /// <param name="logFilepath">Output filename to Debug.Log</param>
        /// <param name="namespaceToDirectory">Insert subdirectory named from test namespace if true and filename omitted.</param>
        /// <param name="callerMemberName">Used as the default file name when called outside a test context</param>
        /// <remarks>
        /// When using MacOS with Metal Graphics API, the following warning appears at runtime. It seems that we should just ignore it.
        /// <list type="bullet">
        ///     <item>Ignoring depth surface load action as it is memoryless</item>
        ///     <item>Ignoring depth surface store action as it is memoryless</item>
        /// </list>
        /// </remarks>
        /// <seealso href="https://stackoverflow.com/questions/66062201/unity-warning-ignoring-depth-surface-load-action-as-it-is-memoryless"/>
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
            if (TestContext.CurrentTestExecutionContext != null)
            {
                var properties = TestContext.CurrentContext.Test.Properties;
                properties.Add("Screenshot", path);
            }
#endif
        }

#if UNITY_2023_1_OR_NEWER
        /// <summary>
        /// Take a screenshot and save it to file.
        /// <p/>
        /// Limitations:
        /// <list type="bullet">
        ///     <item>Do not call from Edit Mode tests.</item>
        ///     <item>Must be called from main thread.</item>
        ///     <item><c>GameView</c> must be visible. Use the <c>FocusGameViewAttribute</c> or <c>GameViewResolutionAttribute</c> if running on batch mode.</item>
        /// </list>
        /// </summary>
        /// <param name="directory">Directory to save screenshots.
        /// If omitted, the directory specified by command line argument "-testHelperScreenshotDirectory" is used.
        /// If the command line argument is also omitted, <c>Application.persistentDataPath</c> + "/TestHelper/Screenshots/" is used.</param>
        /// <param name="filename">Filename to store screenshot.
        /// If omitted, default filename is <c>TestContext.Test.Name</c> + ".png" when run in test context.
        /// Using <see cref="callerMemberName"/> when called outside a test context.</param>
        /// <param name="namespaceToDirectory">Insert subdirectory named from test namespace if true and filename omitted.</param>
        /// <param name="scale">Save screenshot scale factor.</param>
        /// <param name="callerMemberName">Used as the default file name when called outside a test context</param>
        public static async Awaitable TakeScreenshotAsync(
            string directory = null,
            string filename = null,
            bool namespaceToDirectory = false,
            float scale = 1.0f,
            [CallerMemberName] string callerMemberName = null)
        {
            var png = await TakeScreenshotAsPngBytesAsync(scale);

            var path = GetSavePath(directory, filename, namespaceToDirectory, callerMemberName);
            await File.WriteAllBytesAsync(path, png);

#if UNITY_INCLUDE_TESTS
            if (TestContext.CurrentTestExecutionContext != null)
            {
                var properties = TestContext.CurrentContext.Test.Properties;
                properties.Add("Screenshot", path);
            }
#endif
        }

        /// <summary>
        /// Take a screenshot.
        /// <p/>
        /// Limitations:
        /// <list type="bullet">
        ///     <item>Do not call from Edit Mode tests.</item>
        ///     <item>Must be called from main thread.</item>
        ///     <item><c>GameView</c> must be visible. Use the <c>FocusGameViewAttribute</c> or <c>GameViewResolutionAttribute</c> if running on batch mode.</item>
        /// </list>
        /// </summary>
        /// <param name="scale">Save screenshot scale factor.</param>
        /// <returns>PNG image byte array.</returns>
        public static async Awaitable<byte[]> TakeScreenshotAsPngBytesAsync(float scale = 1.0f)
        {
            if (Camera.main)
            {
                Camera.main.forceIntoRenderTexture = true;
            }

            await Awaitable.EndOfFrameAsync(); // Required to take screenshots

            byte[] png;

            if (SystemInfo.supportsAsyncGPUReadback)
            {
                var capturedTexture = RenderTexture.GetTemporary(Screen.width, Screen.height);
                var format = capturedTexture.graphicsFormat;
                ScreenCapture.CaptureScreenshotIntoRenderTexture(capturedTexture);

                var width = (int)(Screen.width * scale);
                var height = (int)(Screen.height * scale);
                var scaledTexture = RenderTexture.GetTemporary(width, height);
                Graphics.Blit(capturedTexture, scaledTexture, s_blitScale, s_blitOffset); // y-axis flip if needed
                RenderTexture.ReleaseTemporary(capturedTexture);

                var request = await AsyncGPUReadback.RequestAsync(scaledTexture);
                RenderTexture.ReleaseTemporary(scaledTexture);

                using var imageBytes = request.GetData<byte>();
                var imageByteArray = imageBytes.ToArray();
                // Note: Reason for not using ArrayPool: NativeArray.CopyTo throws "ArgumentException: source and destination length must be the same" in Unity 6000.2.6f1.

                await Awaitable.BackgroundThreadAsync();
                png = ImageConversion.EncodeArrayToPNG(imageByteArray, format, (uint)width, (uint)height);
                await Awaitable.MainThreadAsync();
            }
            else
            {
                var texture = ScreenCapture.CaptureScreenshotAsTexture(1);
                png = texture.EncodeToPNG();
                Object.Destroy(texture);
            }

            return png;
        }

        private static readonly Vector2 s_blitScale = SystemInfo.graphicsUVStartsAtTop
            ? new Vector2(1, -1) // Flip Y-axis
            : new Vector2(1, 1);

        private static readonly Vector2 s_blitOffset = SystemInfo.graphicsUVStartsAtTop
            ? new Vector2(0, 1) // Offset Y-axis after flip
            : new Vector2(0, 0);
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
