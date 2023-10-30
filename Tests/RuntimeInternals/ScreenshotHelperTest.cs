// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
using System.IO;
using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace TestHelper.RuntimeInternals
{
    [TestFixture]
    [GameViewResolution(GameViewResolution.VGA)]
    public class ScreenshotHelperTest
    {
        private const string TestScene = "Packages/com.nowsprinting.test-helper/Tests/Scenes/ScreenshotTest.unity";
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

            Assert.That(path, Does.Exist);
            Assert.That(File.ReadAllBytes(path), Has.Length.GreaterThan(FileSizeThreshold));
        }
    }
}
