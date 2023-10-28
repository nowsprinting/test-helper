// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
using System.IO;
using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace TestHelper.Utils
{
    [TestFixture]
    [GameViewResolution(GameViewResolution.VGA)]
    public class ScreenshotHelperTest
    {
        private const int FileSizeThreshold = 5441; // VGA size solid color file size

        private readonly string _defaultOutputDirectory =
            Path.Combine(Application.persistentDataPath, "TestHelper", "Screenshots");

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
        [LoadScene("Packages/com.nowsprinting.test-helper/Tests/Scenes/ScreenshotTest.unity")]
        public IEnumerator TakeScreenshot_SaveToDefaultPath()
        {
            yield return ScreenshotHelper.TakeScreenshot();

            var path = Path.Combine(_defaultOutputDirectory, $"{nameof(TakeScreenshot_SaveToDefaultPath)}.png");
            Assert.That(path, Does.Exist);
            Assert.That(File.ReadAllBytes(path), Has.Length.GreaterThan(FileSizeThreshold));
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor)]
        [LoadScene("Packages/com.nowsprinting.test-helper/Tests/Scenes/ScreenshotTest.unity")]
        public IEnumerator TakeScreenshot_SpecifyDirectoryInEditor_SaveToSpecifyPath()
        {
            const string RelativePath = "Logs/Screenshots";
            yield return ScreenshotHelper.TakeScreenshot(directory: RelativePath);

            var path = Path.Combine(Path.GetFullPath(RelativePath),
                $"{nameof(TakeScreenshot_SpecifyDirectoryInEditor_SaveToSpecifyPath)}.png");
            Assert.That(path, Does.Exist);
        }

        [UnityTest]
        [UnityPlatform(exclude =
            new[] { RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor })]
        [LoadScene("Packages/com.nowsprinting.test-helper/Tests/Scenes/ScreenshotTest.unity")]
        public IEnumerator TakeScreenshot_SpecifyDirectoryOnPlayer_SaveToDefaultPath()
        {
            const string RelativePath = "Logs/Screenshots";
            yield return ScreenshotHelper.TakeScreenshot(directory: RelativePath);

            var path = Path.Combine(_defaultOutputDirectory,
                $"{nameof(TakeScreenshot_SpecifyDirectoryOnPlayer_SaveToDefaultPath)}.png");
            Assert.That(path, Does.Exist);
        }

        [UnityTest]
        [LoadScene("Packages/com.nowsprinting.test-helper/Tests/Scenes/ScreenshotTest.unity")]
        public IEnumerator TakeScreenshot_SpecifyFilename_SaveToSpecifyPath()
        {
            const string Filename1 = "SpecifyFilename1.png";
            const string Filename2 = "SpecifyFilename2.png";

            _text.text = Filename1;
            yield return ScreenshotHelper.TakeScreenshot(filename: Filename1);

            _text.text = Filename2;
            yield return ScreenshotHelper.TakeScreenshot(filename: Filename2);

            Assert.That(Path.Combine(_defaultOutputDirectory, Filename1), Does.Exist);
            Assert.That(Path.Combine(_defaultOutputDirectory, Filename2), Does.Exist);
        }

        [UnityTest]
        [LoadScene("Packages/com.nowsprinting.test-helper/Tests/Scenes/ScreenshotTest.unity")]
        public IEnumerator TakeScreenshot_SpecifySuperSize_SaveSuperSizeFile()
        {
            yield return ScreenshotHelper.TakeScreenshot(superSize: 2);

            var path = Path.Combine(_defaultOutputDirectory,
                $"{nameof(TakeScreenshot_SpecifySuperSize_SaveSuperSizeFile)}.png");
            Assert.That(path, Does.Exist);
            // Please visually check the file.
        }

        [UnityTest]
        [LoadScene("Packages/com.nowsprinting.test-helper/Tests/Scenes/ScreenshotTest.unity")]
        public IEnumerator TakeScreenshot_SpecifyStereoCaptureMode_SaveStereoFile()
        {
            yield return ScreenshotHelper.TakeScreenshot(
                stereoCaptureMode: ScreenCapture.StereoScreenCaptureMode.BothEyes);

            var path = Path.Combine(_defaultOutputDirectory,
                $"{nameof(TakeScreenshot_SpecifyStereoCaptureMode_SaveStereoFile)}.png");
            Assert.That(path, Does.Exist);
            // Require stereo rendering settings.
            // See: https://docs.unity3d.com/Manual/SinglePassStereoRendering.html
        }

        [UnityTest]
        public IEnumerator TakeScreenshot_SpecifySuperSizeAndStereoCaptureMode_NotWork()
        {
            yield return ScreenshotHelper.TakeScreenshot(
                superSize: 2,
                stereoCaptureMode: ScreenCapture.StereoScreenCaptureMode.BothEyes);
            LogAssert.Expect(LogType.Error, "superSize and stereoCaptureMode cannot be specified at the same time.");
        }

        [UnityTest]
        [LoadScene("Packages/com.nowsprinting.test-helper/Tests/Scenes/ScreenshotTest.unity")]
        public IEnumerator TakeScreenshot_Parameterized_SaveAllFiles(
            [Values(0, 1)] int i,
            [Values(2, 3)] int j)
        {
            yield return ScreenshotHelper.TakeScreenshot();

            var path = Path.Combine(_defaultOutputDirectory,
                $"{nameof(TakeScreenshot_Parameterized_SaveAllFiles)}_{i}-{j}_.png");
            Assert.That(path, Does.Exist);
        }
    }
}
