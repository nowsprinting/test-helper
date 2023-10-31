// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace TestHelper.RuntimeInternals
{
    [TestFixture]
    [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor)]
    public class ScreenshotHelperTest
    {
        private const string TestScene = "Packages/com.nowsprinting.test-helper/Tests/Scenes/ScreenshotTest.unity";
        private const int FileSizeThreshold = 5441; // VGA size solid color file size

        private readonly string _defaultOutputDirectory =
            Path.Combine(Application.persistentDataPath, "TestHelper", "Screenshots");

        private Text _text;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
#if UNITY_EDITOR
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(
                TestScene,
                new LoadSceneParameters(LoadSceneMode.Single));
#endif
            var textObject = GameObject.Find("Text");
            Assume.That(textObject, Is.Not.Null);

            _text = textObject.GetComponent<Text>();
            _text.text = TestContext.CurrentTestExecutionContext.CurrentTest.Name;
        }

        [UnityTest]
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

        [Test]
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
            //   And also needs coroutineRunner (any MonoBehaviour) because TakeScreenshot method uses WaitForEndOfFrame inside.
            //   See more information: https://github.com/Cysharp/UniTask#ienumeratortounitask-limitation

            Assert.That(path, Does.Exist);
        }

        private class CoroutineRunner : MonoBehaviour
        {
        }
    }
}
