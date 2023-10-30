// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TestHelper.RuntimeInternals
{
    /// <summary>
    /// <c>GameView</c> control helper.
    /// </summary>
    public static class GameViewControlHelper
    {
        private static Type s_gameView;

        /// <summary>
        /// Focus <c>GameView</c> or <c>SimulatorWindow</c>.
        /// </summary>
        public static void Focus()
        {
#if UNITY_EDITOR
            if (s_gameView == null)
            {
                var assembly = Assembly.Load("UnityEditor.dll");
                var viewClass = Application.isBatchMode ? "UnityEditor.GameView" : "UnityEditor.PlayModeView";
                // Note: Freezes when getting SimulatorWindow in batchmode

                s_gameView = assembly.GetType(viewClass);
            }

            EditorWindow.GetWindow(s_gameView, false, null, true);
#endif
        }
    }
}
