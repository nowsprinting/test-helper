// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using UnityEngine;
#if UNITY_EDITOR
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
        private static Type s_gameView;
        private static MethodInfo s_setCustomResolution;

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
            if (s_setCustomResolution == null)
            {
                var assembly = Assembly.Load("UnityEditor.dll");
                s_gameView = assembly.GetType("UnityEditor.GameView");
                if (s_gameView == null)
                {
                    Debug.LogError("GameView type not found.");
                    return;
                }

                s_setCustomResolution =
                    s_gameView.GetMethod("SetCustomResolution", BindingFlags.Instance | BindingFlags.NonPublic);
                if (s_setCustomResolution == null)
                {
                    Debug.LogError("SetCustomResolution method not found.");
                    return;
                }
            }

            var gameView = EditorWindow.GetWindow(s_gameView, false, null, true);
            s_setCustomResolution.Invoke(gameView, new object[] { new Vector2(_width, _height), _name });
#endif
        }
    }
}
