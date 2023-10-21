// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using UnityEngine;
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif

namespace TestHelper.Attributes
{
    /// <summary>
    /// Focus <c>GameView</c> or <c>SimulatorWindow</c> before run test.
    ///
    /// Example usage: Tests that use <c>InputEventTrace</c> of the Input System package (com.unity.inputsystem).
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public class FocusGameViewAttribute : NUnitAttribute, IApplyToContext
    {
        private static Type s_gameView;

        /// <inheritdoc />
        public void ApplyToContext(ITestExecutionContext context)
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
