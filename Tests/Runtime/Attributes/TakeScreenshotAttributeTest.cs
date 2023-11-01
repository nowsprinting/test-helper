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
        public void Attach_TakeScreenshotAndSaveToDefaultPath()
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                $"{nameof(Attach_TakeScreenshotAndSaveToDefaultPath)}.png");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            Assume.That(path, Does.Not.Exist);

            // Take screenshot after running the test.
        }

        [Test, Order(1)]
        public void Attach_TakeScreenshotAndSaveToDefaultPath_AfterRunningTest_ExistFile()
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                $"{nameof(Attach_TakeScreenshotAndSaveToDefaultPath)}.png");
            Assert.That(path, Does.Exist);
        }

        [Test, Order(0)]
        [LoadScene(TestScene)]
        [TakeScreenshot]
        public async Task AttachToAsyncTest_TakeScreenshotAndSaveToDefaultPath()
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                $"{nameof(AttachToAsyncTest_TakeScreenshotAndSaveToDefaultPath)}.png");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            Assume.That(path, Does.Not.Exist);

            await Task.Yield();
            // Take screenshot after running the test.
        }

        [Test, Order(1)]
        public void AttachToAsyncTest_TakeScreenshotAndSaveToDefaultPath_AfterRunningTest_ExistFile()
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                $"{nameof(AttachToAsyncTest_TakeScreenshotAndSaveToDefaultPath)}.png");
            Assert.That(path, Does.Exist);
        }

        [UnityTest, Order(0)]
        [LoadScene(TestScene)]
        [TakeScreenshot]
        public IEnumerator AttachToUnityTest_TakeScreenshotAndSaveToDefaultPath()
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                $"{nameof(AttachToUnityTest_TakeScreenshotAndSaveToDefaultPath)}.png");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            Assume.That(path, Does.Not.Exist);

            yield return null;
            // Take screenshot after running the test.
        }

        [Test, Order(1)]
        public void AttachToUnityTest_TakeScreenshotAndSaveToDefaultPath_AfterRunningTest_ExistFile()
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                $"{nameof(AttachToUnityTest_TakeScreenshotAndSaveToDefaultPath)}.png");
            Assert.That(path, Does.Exist);
        }

        private const string SpecifyFilename =
            nameof(AttachWithFilename_TakeScreenshotAndSaveToSpecifyPath) + "_Specified.png";

        [Test, Order(0)]
        [LoadScene(TestScene)]
        [TakeScreenshot(filename: SpecifyFilename)]
        public void AttachWithFilename_TakeScreenshotAndSaveToSpecifyPath()
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
        public void AttachWithFilename_TakeScreenshotAndSaveToSpecifyPath_AfterRunningTest_ExistFile()
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                SpecifyFilename);
            Assert.That(path, Does.Exist);
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
        public void AttachWithGizmos_TakeScreenshotWithGizmos_AfterRunningTest_ExistFile()
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                $"{nameof(AttachWithGizmos_TakeScreenshotWithGizmos)}.png");
            Assert.That(path, Does.Exist);
        }
    }
}
