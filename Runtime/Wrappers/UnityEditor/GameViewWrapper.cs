// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TestHelper.Wrappers.UnityEditor
{
    public class GameViewWrapper
    {
        private static readonly Assembly s_editorAssembly = Assembly.Load("UnityEditor.dll");
        private static readonly Type s_gameView = s_editorAssembly.GetType("UnityEditor.GameView");

        private static readonly PropertyInfo s_selectedSizeIndex = s_gameView
            .GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly EditorWindow _instance;

        private GameViewWrapper(EditorWindow instance)
        {
            _instance = instance;
        }

        public static GameViewWrapper GetWindow()
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

            var gameView = EditorWindow.GetWindow(s_gameView, false, null, true);
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
    }
}
#endif
