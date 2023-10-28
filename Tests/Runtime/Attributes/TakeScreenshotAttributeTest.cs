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
    public class TakeScreenshotAttributeTest
    {
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
        [GameViewResolution(GameViewResolution.VGA)]
        [LoadScene("Packages/com.nowsprinting.test-helper/Tests/Scenes/ScreenshotTest.unity")]
        [TakeScreenshot]
        public void Attach_TakeScreenshotAndSaveToDefaultPath()
        {
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
        [GameViewResolution(GameViewResolution.VGA)]
        [LoadScene("Packages/com.nowsprinting.test-helper/Tests/Scenes/ScreenshotTest.unity")]
        [TakeScreenshot]
        public async Task AttachToAsyncTest_TakeScreenshotAndSaveToDefaultPath()
        {
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
        [GameViewResolution(GameViewResolution.VGA)]
        [LoadScene("Packages/com.nowsprinting.test-helper/Tests/Scenes/ScreenshotTest.unity")]
        [TakeScreenshot]
        public IEnumerator AttachToUnityTest_TakeScreenshotAndSaveToDefaultPath()
        {
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

        [Test, Order(0)]
        [GameViewResolution(GameViewResolution.VGA)]
        [LoadScene("Packages/com.nowsprinting.test-helper/Tests/Scenes/ScreenshotTest.unity")]
        [TakeScreenshot(filename: nameof(AttachWithFilename_TakeScreenshotAndSaveToSpecifyPath) + ".png")]
        public void AttachWithFilename_TakeScreenshotAndSaveToSpecifyPath()
        {
            // Take screenshot after running the test.
        }

        [Test, Order(1)]
        public void AttachWithFilename_TakeScreenshotAndSaveToSpecifyPath_AfterRunningTest_ExistFile()
        {
            var path = Path.Combine(
                _defaultOutputDirectory,
                $"{nameof(AttachWithFilename_TakeScreenshotAndSaveToSpecifyPath)}.png");
            Assert.That(path, Does.Exist);
        }
    }
}
