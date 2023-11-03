// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace TestHelper.Attributes
{
    [TestFixture]
    [GameViewResolution(GameViewResolution.VGA)]
    public class TakeScreenshotAttributeTest
    {
        private const string TestScene = "Packages/com.nowsprinting.test-helper/Tests/Scenes/ScreenshotTest.unity";
        private const int FileSizeThreshold = 5441; // VGA size solid color file size
        private const int FileSizeThreshold2X = 100 * 1024; // Normal size is 80 to 90KB

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

        [Test, Order(0)]
        [LoadScene(TestScene)]
        [TakeScreenshot]
        public void Attach_SaveScreenshotToDefaultPath()
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                $"{nameof(Attach_SaveScreenshotToDefaultPath)}.png");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            Assume.That(path, Does.Not.Exist);

            // Take screenshot after running the test.
        }

        [Test, Order(1)]
        public void Attach_SaveScreenshotToDefaultPath_ExistFile()
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                $"{nameof(Attach_SaveScreenshotToDefaultPath)}.png");
            Assert.That(path, Does.Exist);
            Assert.That(File.ReadAllBytes(path), Has.Length.GreaterThan(FileSizeThreshold));
        }

        [Test, Order(0)]
        [LoadScene(TestScene)]
        [TakeScreenshot]
        public async Task AttachToAsyncTest_SaveScreenshotToDefaultPath()
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                $"{nameof(AttachToAsyncTest_SaveScreenshotToDefaultPath)}.png");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            Assume.That(path, Does.Not.Exist);

            await Task.Yield();
            // Take screenshot after running the test.
        }

        [Test, Order(1)]
        public void AttachToAsyncTest_SaveScreenshotToDefaultPath_ExistFile()
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                $"{nameof(AttachToAsyncTest_SaveScreenshotToDefaultPath)}.png");
            Assert.That(path, Does.Exist);
            Assert.That(File.ReadAllBytes(path), Has.Length.GreaterThan(FileSizeThreshold));
        }

        [UnityTest, Order(0)]
        [LoadScene(TestScene)]
        [TakeScreenshot]
        public IEnumerator AttachToUnityTest_SaveScreenshotToDefaultPath()
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                $"{nameof(AttachToUnityTest_SaveScreenshotToDefaultPath)}.png");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            Assume.That(path, Does.Not.Exist);

            yield return null;
            // Take screenshot after running the test.
        }

        [Test, Order(1)]
        public void AttachToUnityTest_SaveScreenshotToDefaultPath_ExistFile()
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                $"{nameof(AttachToUnityTest_SaveScreenshotToDefaultPath)}.png");
            Assert.That(path, Does.Exist);
            Assert.That(File.ReadAllBytes(path), Has.Length.GreaterThan(FileSizeThreshold));
        }

        [Test, Order(0)]
        [LoadScene(TestScene)]
        [TakeScreenshot]
        public void AttachToParameterizedTest_SaveAllScreenshots(
            [Values(0, 1)] int i,
            [Values(2, 3)] int j)
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                $"{nameof(AttachToParameterizedTest_SaveAllScreenshots)}_{i}-{j}_.png");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            Assume.That(path, Does.Not.Exist);

            // Take screenshot after running the test.
        }

        [Test, Order(1)]
        public void AttachToParameterizedTest_SaveAllScreenshots_ExistFile(
            [Values(0, 1)] int i,
            [Values(2, 3)] int j)
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                $"{nameof(AttachToParameterizedTest_SaveAllScreenshots)}_{i}-{j}_.png");
            Assert.That(path, Does.Exist);
        }

        private const string SpecifyRelativeDirectory = "Logs/TestHelper/Screenshots";

        [Test, Order(0)]
        [LoadScene(TestScene)]
        [TakeScreenshot(directory: SpecifyRelativeDirectory)]
        public void AttachWithDirectory_SaveScreenshotToSpecifyPath()
        {
            var path = Path.Combine(
                Path.GetFullPath(SpecifyRelativeDirectory),
                $"{nameof(AttachWithDirectory_SaveScreenshotToSpecifyPath)}.png");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            Assume.That(path, Does.Not.Exist);

            // Take screenshot after running the test.
        }

        [Test, Order(1)]
        public void AttachWithDirectory_SaveScreenshotToSpecifyPath_ExistFile()
        {
            var path = Path.Combine(
                Path.GetFullPath(SpecifyRelativeDirectory),
                $"{nameof(AttachWithDirectory_SaveScreenshotToSpecifyPath)}.png");
            Assert.That(path, Does.Exist);
        }

        private const string SpecifyFilename =
            nameof(AttachWithFilename_SaveScreenshotToSpecifyPath) + "_Specified.png";

        [Test, Order(0)]
        [LoadScene(TestScene)]
        [TakeScreenshot(filename: SpecifyFilename)]
        public void AttachWithFilename_SaveScreenshotToSpecifyPath()
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                SpecifyFilename);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            Assume.That(path, Does.Not.Exist);

            // Take screenshot after running the test.
        }

        [Test, Order(1)]
        public void AttachWithFilename_SaveScreenshotToSpecifyPath_ExistFile()
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                SpecifyFilename);
            Assert.That(path, Does.Exist);
        }

        [Test, Order(0)]
        [LoadScene(TestScene)]
        [TakeScreenshot(superSize: 2)]
        public void AttachWithSuperSize_SaveSuperSizeScreenshot()
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                $"{nameof(AttachWithSuperSize_SaveSuperSizeScreenshot)}.png");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            Assume.That(path, Does.Not.Exist);

            // Take screenshot after running the test.
        }

        [Test, Order(1)]
        public void AttachWithSuperSize_SaveSuperSizeScreenshot_ExistFile()
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                $"{nameof(AttachWithSuperSize_SaveSuperSizeScreenshot)}.png");
            Assert.That(path, Does.Exist);
            Assert.That(File.ReadAllBytes(path), Has.Length.GreaterThan(FileSizeThreshold2X));
        }

        [Test, Order(0)]
        [LoadScene(TestScene)]
        [TakeScreenshot(stereoCaptureMode: ScreenCapture.StereoScreenCaptureMode.BothEyes)]
        public void AttachWithStereoCaptureMode_SaveStereoScreenshot()
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                $"{nameof(AttachWithStereoCaptureMode_SaveStereoScreenshot)}.png");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            Assume.That(path, Does.Not.Exist);

            // Take screenshot after running the test.
        }

        [Test, Order(1)]
        public void AttachWithStereoCaptureMode_SaveStereoScreenshot_ExistFile()
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                $"{nameof(AttachWithStereoCaptureMode_SaveStereoScreenshot)}.png");
            Assert.That(path, Does.Exist);
            // Is it a stereo screenshot? See for yourself! Be a witness!!
            // Note: Require stereo rendering settings.
            // See: https://docs.unity3d.com/Manual/SinglePassStereoRendering.html
        }

        private class GizmoDemo : MonoBehaviour
        {
            private void OnDrawGizmos()
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position, 0.2f);
            }
        }

        [Test, Order(0)]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor)]
        [LoadScene(TestScene)]
        [TakeScreenshot(gizmos: true)]
        public void AttachWithGizmos_TakeScreenshotWithGizmos()
        {
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.AddComponent<GizmoDemo>();

            // Take screenshot after running the test.
        }

        [Test, Order(1)]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor)]
        public void AttachWithGizmos_TakeScreenshotWithGizmos_ExistFile()
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                $"{nameof(AttachWithGizmos_TakeScreenshotWithGizmos)}.png");
            Assert.That(path, Does.Exist);
            // Is Gizmos shown in the screenshot? See for yourself! Be a witness!!
        }
    }
}
