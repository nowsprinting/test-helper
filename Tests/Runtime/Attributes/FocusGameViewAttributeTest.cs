// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TestHelper.Attributes
{
    [TestFixture]
    public class FocusGameViewAttributeTest
    {
        [Test]
        [FocusGameView]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor)]
        [IgnoreBatchMode("Open GameView in batchmode but can not be focused.")]
        public void Attach_GameViewHasFocus()
        {
            const string GameView = "UnityEditor.GameView";
            const string SimulatorWindow = "UnityEditor.DeviceSimulation.SimulatorWindow";
#if UNITY_EDITOR
            var focusedWindow = EditorWindow.focusedWindow;
            Assert.That(focusedWindow.GetType().FullName, Is.EqualTo(GameView).Or.EqualTo(SimulatorWindow));
#endif
        }

        [Test]
        [FocusGameView]
        [IgnoreWindowMode("For batchmode test.")]
        public void Attach_KeepBatchmode()
        {
            Assert.That(Application.isBatchMode, Is.True);
        }
    }
}
