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
    [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor)]
    [IgnoreBatchMode("Open GameView in batch mode but can not be focused.")]
    public class FocusGameViewAttributeTest
    {
        [Test]
        [FocusGameView]
        public void Attach_GameViewHasFocus()
        {
            const string GameView = "UnityEditor.GameView";
            const string SimulatorWindow = "UnityEditor.DeviceSimulation.SimulatorWindow";
#if UNITY_EDITOR
            var focusedWindow = EditorWindow.focusedWindow;
            Assert.That(focusedWindow.GetType().FullName, Is.EqualTo(GameView).Or.EqualTo(SimulatorWindow));
#endif
        }
    }
}
