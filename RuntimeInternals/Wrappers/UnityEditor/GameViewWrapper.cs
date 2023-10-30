// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TestHelper.RuntimeInternals.Wrappers.UnityEditor
{
    /// <summary>
    /// Wrapper class for <c>UnityEditor.GameView</c>.
    /// This class can be used from the runtime code because it does not depend on test-framework.
    /// </summary>
    public class GameViewWrapper
    {
        private static readonly Assembly s_editorAssembly = Assembly.Load("UnityEditor.dll");
        private static readonly Type s_gameView = s_editorAssembly.GetType("UnityEditor.GameView");

        private static readonly PropertyInfo s_selectedSizeIndex = s_gameView
            .GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        private static readonly FieldInfo s_gizmos = s_gameView
            .GetField("m_Gizmos", BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly EditorWindow _instance;

        private GameViewWrapper(EditorWindow instance)
        {
            _instance = instance;
        }

        public static GameViewWrapper GetWindow(bool focus = true)
        {
            if (s_gameView == null)
            {
                Debug.LogError("GameView type not found.");
                return null;
                // Note: Do not use Exception (and Assert). Because freezes async tests on UTF v1.3.4, See UUM-25085.
            }

            if (s_selectedSizeIndex == null)
            {
                Debug.LogError("GameView.selectedSizeIndex property not found.");
                return null;
            }

            if (s_gizmos == null)
            {
                Debug.LogError("GameView.m_Gizmos field not found.");
                return null;
            }

            var gameView = EditorWindow.GetWindow(s_gameView, false, null, focus);
            if (gameView == null)
            {
                Debug.LogError("EditorWindow.GetWindow(GameView) failed.");
                return null;
            }

            return new GameViewWrapper(gameView);
        }

        public int SelectedSizeIndex()
        {
            return (int)s_selectedSizeIndex.GetValue(_instance);
        }

        public void SelectedSizeIndex(int index)
        {
            s_selectedSizeIndex.SetValue(_instance, index);
        }

        public bool GetGizmos()
        {
            return (bool)s_gizmos.GetValue(_instance);
        }

        public void SetGizmos(bool show)
        {
            s_gizmos.SetValue(_instance, show);
        }
    }
}
#endif
