// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TestHelper.Attributes
{
    [TestFixture]
    [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor)]
    public class FocusGameViewAttributeTest
    {
        [Test]
        [IgnoreBatchMode("Open GameView in batchmode but can not get window.")]
        [FocusGameView]
        public void Attach_GameViewHasFocus()
        {
            const string GameView = "UnityEditor.GameView";
            const string SimulatorWindow = "UnityEditor.DeviceSimulation.SimulatorWindow";
#if UNITY_EDITOR
            var focusedWindow = EditorWindow.focusedWindow;
            Assert.That(focusedWindow, Is.Not.Null);
            Assert.That(focusedWindow.GetType().FullName, Is.EqualTo(GameView).Or.EqualTo(SimulatorWindow));
#endif
        }

        [Test]
        [IgnoreWindowMode("For batchmode test.")]
        [FocusGameView]
        public void Attach_KeepBatchmode()
        {
            Assert.That(Application.isBatchMode, Is.True);
        }

        [Test]
        [FocusGameView]
        public async Task AttachToAsyncTest_Normally()
        {
            await Task.Yield();
        }

        [UnityTest]
        [FocusGameView]
        public IEnumerator AttachToUnityTest_Normally()
        {
            yield return null;
        }
    }
}
