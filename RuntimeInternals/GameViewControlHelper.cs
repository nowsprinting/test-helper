// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Reflection;
using TestHelper.RuntimeInternals.Wrappers.UnityEditor;
using UnityEditor;
using UnityEngine;

namespace TestHelper.RuntimeInternals
{
    /// <summary>
    /// <c>GameView</c> control helper.
    /// This class can be used from the runtime code because it does not depend on test-framework.
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

        /// <summary>
        /// Set <c>GameView</c> resolution.
        /// </summary>
        /// <param name="width">GameView width [px]</param>
        /// <param name="height">GameView height [px]</param>
        /// <param name="name">GameViewSize name</param>
        public static void SetResolution(uint width, uint height, string name)
        {
#if UNITY_2022_2_OR_NEWER
            SetResolutionUsingPlayModeWindow(width, height, name);
#else
            SetResolutionUsingReflection(width, height, name);
#endif
        }

        // ReSharper disable once UnusedMember.Local
        private static void SetResolutionUsingPlayModeWindow(uint width, uint height, string name)
        {
#if UNITY_EDITOR && UNITY_2022_2_OR_NEWER
            PlayModeWindow.SetViewType(PlayModeWindow.PlayModeViewTypes.GameView);
            PlayModeWindow.SetCustomRenderingResolution(width, height, name);
#endif
        }

        // ReSharper disable once UnusedMember.Local
        private static void SetResolutionUsingReflection(uint width, uint height, string name)
        {
#if UNITY_EDITOR
            var gameViewSizes = GameViewSizesWrapper.CreateInstance();
            if (gameViewSizes == null)
            {
                Debug.LogError("GameViewSizes instance creation failed.");
                return;
            }

            var gameViewSizeGroup = gameViewSizes.CurrentGroup();
            if (gameViewSizeGroup == null)
            {
                Debug.LogError("GameViewSizeGroup instance creation failed.");
                return;
            }

            var gameViewSize = GameViewSizeWrapper.CreateInstance((int)width, (int)height, name);
            if (gameViewSize == null)
            {
                Debug.LogError("GameViewSize instance creation failed.");
                return;
            }

            var index = gameViewSizeGroup.IndexOf(gameViewSize);
            if (index == -1)
            {
                gameViewSizeGroup.AddCustomSize(gameViewSize);
                index = gameViewSizeGroup.IndexOf(gameViewSize);
            }

            var gameView = GameViewWrapper.GetWindow();
            if (gameView == null)
            {
                Debug.LogError("GameView instance creation failed.");
                return;
            }

            gameView.SelectedSizeIndex(index);
#endif
        }
    }
}
