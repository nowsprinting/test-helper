// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace TestHelper.RuntimeInternals
{
    [TestFixture]
    public class ScreenshotHelperTest
    {
        private const string TestScene = "Packages/com.nowsprinting.test-helper/Tests/Scenes/ScreenshotTest.unity";
        private const int FileSizeThreshold = 5441; // VGA size solid color file size
        private readonly string _defaultOutputDirectory = CommandLineArgs.GetScreenshotDirectory();

        private Text _text;

        [SetUp]
        public void SetUp()
        {
            var textObject = GameObject.Find("Text");
            Assume.That(textObject, Is.Not.Null);

            _text = textObject.GetComponent<Text>();
            _text.text = TestContext.CurrentTestExecutionContext.CurrentTest.Name;
        }

        [UnityTest]
        [LoadScene(TestScene)]
        public IEnumerator TakeScreenshot_SaveToDefaultPath()
        {
            var path = Path.Combine(_defaultOutputDirectory, $"{nameof(TakeScreenshot_SaveToDefaultPath)}.png");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            Assume.That(path, Does.Not.Exist);

            yield return ScreenshotHelper.TakeScreenshot(); // default filename is member name when internal

            Assert.That(new FileInfo(path), Has.Length.GreaterThanOrEqualTo(FileSizeThreshold));
        }

        [UnityTest]
        [LoadScene(TestScene)]
        public IEnumerator TakeScreenshot_Multiple_SaveToEachSpecifyPaths()
        {
            const string Filename1 = "TakeScreenshot_Multiple_SpecifyFilename1.png";
            const string Filename2 = "TakeScreenshot_Multiple_SpecifyFilename2.png";

            var path1 = Path.Combine(_defaultOutputDirectory, Filename1);
            if (File.Exists(path1))
            {
                File.Delete(path1);
            }

            var path2 = Path.Combine(_defaultOutputDirectory, Filename2);
            if (File.Exists(path2))
            {
                File.Delete(path2);
            }

            Assume.That(path1, Does.Not.Exist);
            Assume.That(path2, Does.Not.Exist);

            _text.text = Filename1;
            yield return ScreenshotHelper.TakeScreenshot(filename: Filename1);

            _text.text = Filename2;
            yield return ScreenshotHelper.TakeScreenshot(filename: Filename2);

            // Verify after calling twice
            Assert.That(path1, Does.Exist);
            Assert.That(path2, Does.Exist);
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

        [Test]
        [LoadScene(TestScene)]
        public async Task TakeScreenshot_FromAsyncTest()
        {
            var path = Path.Combine(_defaultOutputDirectory, $"{nameof(TakeScreenshot_FromAsyncTest)}.png");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            Assume.That(path, Does.Not.Exist);

            var coroutineRunner = new GameObject().AddComponent<CoroutineRunner>();
            await ScreenshotHelper.TakeScreenshot().ToUniTask(coroutineRunner);
            // Note: UniTask is required to be used from the async test.
            //   And also needs CoroutineRunner (any MonoBehaviour) because TakeScreenshot method uses WaitForEndOfFrame inside.
            //   See more information: https://github.com/Cysharp/UniTask#ienumeratortounitask-limitation

            Assert.That(path, Does.Exist);
        }

        private class CoroutineRunner : MonoBehaviour
        {
        }

        [UnityTest]
        [LoadScene(TestScene)]
        public IEnumerator TakeScreenshot_WithoutLogFilepath_SuppressLogging()
        {
            yield return ScreenshotHelper.TakeScreenshot(logFilepath: false);

            LogAssert.NoUnexpectedReceived(); // No output to Debug.Log
        }
    }
}
