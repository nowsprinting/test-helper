// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
#if ENABLE_GRAPHICS_TEST_FRAMEWORK
using TestHelper.RuntimeInternals.TestUtils;
using UnityEngine.TestTools.Graphics;
#endif
#if ENABLE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace TestHelper.RuntimeInternals
{
    [TestFixture]
    public class ScreenshotHelperTest
    {
        private const string TestScene = "../Scenes/ScreenshotTest.unity";
        private readonly string _defaultOutputDirectory = CommandLineArgs.GetScreenshotDirectory();

        private Text _text;

#if ENABLE_GRAPHICS_TEST_FRAMEWORK
        [LoadAsset("../Images/OSXEditor/" + nameof(TakeScreenshot_ImagesMatch) + ".png")] // VGA
        private Texture2D _expectedTakeScreenshotImagesMatch;
        // Note: In the image inspector window, set the following:
        //      - Advanced > Non-Power of 2: None
        //      - Advanced > Read/Write: on

#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
        [LoadAsset("../Images/OSXPlayer/" + nameof(TakeScreenshot_ImagesMatch) + ".png")] // Full HD
#elif UNITY_STANDALONE_LINUX
        [LoadAsset("../Images/OSXEditor/" + nameof(TakeScreenshot_ImagesMatch) + ".png")]   // VGA
#elif UNITY_ANDROID
        [LoadAsset("../Images/Android/Pixel6a/" + nameof(TakeScreenshot_ImagesMatch) + ".png")] // 1080x2400
#endif
        private Texture2D _expectedTakeScreenshotImagesMatchOnPlayer;

        [LoadAsset("../Images/OSXEditor/" + nameof(TakeScreenshotAsync_ImagesMatch) + ".png")] // VGA
        private Texture2D _expectedTakeScreenshotAsyncImagesMatch;

#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
        [LoadAsset("../Images/OSXPlayer/" + nameof(TakeScreenshotAsync_ImagesMatch) + ".png")] // Full HD
#elif UNITY_STANDALONE_LINUX
        [LoadAsset("../Images/OSXEditor/" + nameof(TakeScreenshotAsync_ImagesMatch) + ".png")]  // VGA
#elif UNITY_ANDROID
        [LoadAsset("../Images/Android/Pixel6a/" + nameof(TakeScreenshotAsync_ImagesMatch) + ".png")] // 1080x2400
#endif
        private Texture2D _expectedTakeScreenshotAsyncImagesMatchOnPlayer;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            LoadAssetAttribute.LoadAssets(this);           // Must call this method to load assets.
            VrtUtils.ConvertTexture2dFieldsToARGB32(this); // convert to same as actual texture format
        }
#endif

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

            Assert.That(path, Does.Exist.IgnoreDirectories);
#if !UNITY_ANDROID
            Assert.That(new FileInfo(path), Has.Length.GreaterThanOrEqualTo(0));
#endif
        }

#if ENABLE_GRAPHICS_TEST_FRAMEWORK
        [UnityTest]
        [GameViewResolution(GameViewResolution.VGA)]
        [GizmosShowOnGameView(false)]
        [LoadScene(TestScene)]
        public IEnumerator TakeScreenshot_ImagesMatch()
        {
            var path = Path.Combine(_defaultOutputDirectory, $"{TestContext.CurrentContext.Test.Name}.png");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            yield return ScreenshotHelper.TakeScreenshot();

            var expected = Application.isEditor
                ? _expectedTakeScreenshotImagesMatch
                : _expectedTakeScreenshotImagesMatchOnPlayer;
            var actual = VrtUtils.LoadImage(path);
            var settings = VrtUtils.GetImageComparisonSettings();
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

            // tear down
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

            Assert.That(path, Does.Exist.IgnoreDirectories);
#if !UNITY_ANDROID
            Assert.That(new FileInfo(path), Has.Length.GreaterThanOrEqualTo(0));
#endif
        }

        [Test]
        [LoadScene(TestScene)]
        public async Task TakeScreenshotAsync_ContinuousCall_SaveToDefaultPath()
        {
            var paths = new string[5];
            for (var i = 0; i < paths.Length; i++)
            {
                paths[i] = Path.Combine(_defaultOutputDirectory, $"{TestContext.CurrentContext.Test.Name}.png");
                if (File.Exists(paths[i]))
                {
                    File.Delete(paths[i]);
                }
            }

            for (var i = 0; i < 5; i++)
            {
                _text.text = $"{TestContext.CurrentContext.Test.Name}_{i}.png";
                ScreenshotHelper.TakeScreenshotAsync().AsUniTask().Forget();
                await UniTask.NextFrame();
            }

            foreach (var path in paths)
            {
                Assert.That(path, Does.Exist.IgnoreDirectories);
            }
        }

#if ENABLE_GRAPHICS_TEST_FRAMEWORK
        [Test]
        [GameViewResolution(GameViewResolution.VGA)]
        [GizmosShowOnGameView(false)]
        [LoadScene(TestScene)]
        public async Task TakeScreenshotAsync_ImagesMatch()
        {
            var path = Path.Combine(_defaultOutputDirectory, $"{TestContext.CurrentContext.Test.Name}.png");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            await ScreenshotHelper.TakeScreenshotAsync();

            var expected = Application.isEditor
                ? _expectedTakeScreenshotAsyncImagesMatch
                : _expectedTakeScreenshotAsyncImagesMatchOnPlayer;
            var actual = VrtUtils.LoadImage(path);
            var settings = VrtUtils.GetImageComparisonSettings();
            ImageAssert.AreEqual(expected, actual, settings);
        }
#endif
#endif
    }
}
