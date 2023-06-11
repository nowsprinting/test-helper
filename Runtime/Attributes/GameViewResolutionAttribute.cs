// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using UnityEngine;
#if UNITY_EDITOR
using TestHelper.Wrappers.UnityEditor;
using UnityEditor;
#endif

namespace TestHelper.Attributes
{
    /// <summary>
    /// Set <c>GameView</c> resolution before SetUp test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public class GameViewResolutionAttribute : NUnitAttribute, IApplyToContext
    {
        private readonly uint _width;
        private readonly uint _height;
        private readonly string _name;

        /// <summary>
        /// Set <c>GameView</c> resolution before SetUp test.
        /// </summary>
        /// <param name="width">GameView width [px]</param>
        /// <param name="height">GameView height [px]</param>
        /// <param name="name">GameViewSize name</param>
        public GameViewResolutionAttribute(uint width, uint height, string name)
        {
            _width = width;
            _height = height;
            _name = name;
        }

        public void ApplyToContext(ITestExecutionContext context)
        {
#if UNITY_2022_2_OR_NEWER
            SetResolutionUsingPlayModeWindow();
#else
            SetResolution();
#endif
        }

        private void SetResolutionUsingPlayModeWindow()
        {
#if UNITY_EDITOR && UNITY_2022_2_OR_NEWER
            PlayModeWindow.SetViewType(PlayModeWindow.PlayModeViewTypes.GameView);
            PlayModeWindow.SetCustomRenderingResolution(_width, _height, _name);
#endif
        }

        private void SetResolution()
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

            var gameViewSize = GameViewSizeWrapper.CreateInstance((int)_width, (int)_height, _name);
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
