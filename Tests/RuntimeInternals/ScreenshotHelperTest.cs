// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using TestHelper.Attributes;
using TestHelper.RuntimeInternals.TestUtils;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
#if ENABLE_GRAPHICS_TEST_FRAMEWORK
using UnityEngine.TestTools.Graphics;
#endif
#if ENABLE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace TestHelper.RuntimeInternals
{
    [TestFixture]
    [GameViewResolution(GameViewResolution.VGA)]
    public class ScreenshotHelperTest
    {
        private const string TestScene = "../Scenes/ScreenshotTest.unity";
        private const int FileSizeThreshold = 5441; // VGA size solid color file size
        private readonly string _defaultOutputDirectory = CommandLineArgs.GetScreenshotDirectory();

        private Text _text;

        [SetUp]
        public void SetUp()
        {
            var textObject = GameObject.Find("Text");
            Assume.That(textObject, Is.Not.Null);

            _text = textObject.GetComponent<Text>();
            _text.text = TestContext.CurrentContext.Test.Name;
        }

        [UnityTest]
        [LoadScene(TestScene)]
        public IEnumerator TakeScreenshot_SaveToDefaultPath()
        {
            var path = Path.Combine(_defaultOutputDirectory, $"{TestContext.CurrentContext.Test.Name}.png");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            yield return ScreenshotHelper.TakeScreenshot();

            Assume.That(path, Does.Exist.IgnoreDirectories);
            Assert.That(new FileInfo(path), Has.Length.GreaterThanOrEqualTo(FileSizeThreshold));
        }

#if ENABLE_GRAPHICS_TEST_FRAMEWORK
        [UnityTest]
        [LoadScene(TestScene)]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor)]
        public IEnumerator TakeScreenshot_ImagesMatch()
        {
            var path = Path.Combine(_defaultOutputDirectory, $"{TestContext.CurrentContext.Test.Name}.png");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            yield return ScreenshotHelper.TakeScreenshot();

            var expected = VrtUtils.LoadExpectedImage();
            var actual = VrtUtils.LoadImage(path);
            var settings = VrtUtils.CreateImageComparisonSettings();
            ImageAssert.AreEqual(expected, actual, settings);
        }
#endif

        [UnityTest]
        [LoadScene(TestScene)]
        public IEnumerator TakeScreenshot_MultipleCall_SaveToEachFile()
        {
            var filename1 = $"{TestContext.CurrentContext.Test.Name}.png";
            var filename2 = $"{TestContext.CurrentContext.Test.Name}_1.png";

            var path1 = Path.Combine(_defaultOutputDirectory, filename1);
            if (File.Exists(path1))
            {
                File.Delete(path1);
            }

            var path2 = Path.Combine(_defaultOutputDirectory, filename2);
            if (File.Exists(path2))
            {
                File.Delete(path2);
            }

            _text.text = filename1;
            yield return ScreenshotHelper.TakeScreenshot(); // not specified

            _text.text = filename2;
            yield return ScreenshotHelper.TakeScreenshot(); // not specified

            // Verify after calling twice
            Assert.That(path1, Does.Exist.IgnoreDirectories);
            Assert.That(path2, Does.Exist.IgnoreDirectories);
        }

        [UnityTest]
        [LoadScene(TestScene)]
        public IEnumerator TakeScreenshot_SpecifySuperSizeAndStereoCaptureMode_NotWork()
        {
            yield return ScreenshotHelper.TakeScreenshot(
                superSize: 2,
                stereoCaptureMode: ScreenCapture.StereoScreenCaptureMode.BothEyes);
            LogAssert.Expect(LogType.Warning, "superSize and stereoCaptureMode cannot be specified at the same time.");
        }

        [UnityTest]
        [LoadScene(TestScene)]
        public IEnumerator TakeScreenshot_WithoutLogFilepath_SuppressLogging()
        {
            yield return ScreenshotHelper.TakeScreenshot(logFilepath: false);

            LogAssert.NoUnexpectedReceived(); // No output to Debug.Log
        }

#if ENABLE_UNITASK
        [Test]
        [LoadScene(TestScene)]
        public async Task TakeScreenshot_FromAsyncTest()
        {
            var path = Path.Combine(_defaultOutputDirectory, $"{TestContext.CurrentContext.Test.Name}.png");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var coroutineRunner = new GameObject().AddComponent<CoroutineRunner>();
            await ScreenshotHelper.TakeScreenshot().ToUniTask(coroutineRunner);
            // Note: UniTask is required to be used from the async test.
            //   And also needs CoroutineRunner (any MonoBehaviour) because TakeScreenshot method uses WaitForEndOfFrame inside.
            //   See more information: https://github.com/Cysharp/UniTask#ienumeratortounitask-limitation

            Assert.That(path, Does.Exist.IgnoreDirectories);

            GameObject.Destroy(coroutineRunner);
        }

        private class CoroutineRunner : MonoBehaviour
        {
        }
#endif

#if UNITY_2023_1_OR_NEWER
        [Test]
        [LoadScene(TestScene)]
        public async Task TakeScreenshotAsync_SaveToDefaultPath()
        {
            var path = Path.Combine(_defaultOutputDirectory, $"{TestContext.CurrentContext.Test.Name}.png");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            await ScreenshotHelper.TakeScreenshotAsync();

            Assume.That(path, Does.Exist.IgnoreDirectories);
            Assert.That(new FileInfo(path), Has.Length.GreaterThanOrEqualTo(FileSizeThreshold));
        }

#if ENABLE_GRAPHICS_TEST_FRAMEWORK
        [Test]
        [LoadScene(TestScene)]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor)]
        public async Task TakeScreenshotAsync_ImagesMatch()
        {
            var path = Path.Combine(_defaultOutputDirectory, $"{TestContext.CurrentContext.Test.Name}.png");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            await ScreenshotHelper.TakeScreenshotAsync();

            var expected = VrtUtils.LoadExpectedImage();
            var actual = VrtUtils.LoadImage(path);
            var settings = VrtUtils.CreateImageComparisonSettings();
            ImageAssert.AreEqual(expected, actual, settings);
        }
#endif
#endif
    }
}
