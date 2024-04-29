// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEngine;
#if UNITY_EDITOR
using TestHelper.RuntimeInternals.Wrappers.UnityEditor;
using UnityEditor;
#endif

namespace TestHelper.RuntimeInternals
{
    /// <summary>
    /// <c>GameView</c> control helper.
    /// This class can be used from the runtime code because it does not depend on test-framework.
    /// </summary>
    public static class GameViewControlHelper
    {
        /// <summary>
        /// Focus <c>GameView</c> or <c>SimulatorWindow</c>.
        /// </summary>
        public static void Focus()
        {
#if UNITY_EDITOR
#if UNITY_2022_2_OR_NEWER
            PlayModeWindow.SetViewType(PlayModeWindow.PlayModeViewTypes.GameView);
#else
            GameViewWrapper.GetWindow();
#endif
#endif
        }

        /// <summary>
        /// Get Gizmos show/ hide status on <c>GameView</c>.
        /// </summary>
        /// <returns>True: show Gizmos, False: hide Gizmos.</returns>
        public static bool GetGizmos()
        {
            var gizmos = false;
#if UNITY_EDITOR
#if UNITY_2022_2_OR_NEWER
            PlayModeWindow.SetViewType(PlayModeWindow.PlayModeViewTypes.GameView);
#endif
            var gameViewWrapper = GameViewWrapper.GetWindow(false);
            if (gameViewWrapper != null)
            {
                gizmos = gameViewWrapper.GetGizmos();
            }
#endif
            return gizmos;
        }

        /// <summary>
        /// Show/ hide Gizmos on <c>GameView</c>.
        /// </summary>
        /// <param name="show">True: show Gizmos, False: hide Gizmos.</param>
        public static void SetGizmos(bool show)
        {
#if UNITY_EDITOR
#if UNITY_2022_2_OR_NEWER
            PlayModeWindow.SetViewType(PlayModeWindow.PlayModeViewTypes.GameView);
#endif
            var gameViewWrapper = GameViewWrapper.GetWindow(false);
            if (gameViewWrapper != null)
            {
                gameViewWrapper.SetGizmos(show);
            }
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
