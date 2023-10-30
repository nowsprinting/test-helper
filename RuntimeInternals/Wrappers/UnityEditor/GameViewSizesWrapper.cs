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
    /// Wrapper class for <c>UnityEditor.GameViewSizes</c>.
    /// This class can be used from the runtime code because it does not depend on test-framework.
    /// </summary>
    public class GameViewSizesWrapper
    {
        private static readonly Assembly s_editorAssembly = Assembly.Load("UnityEditor.dll");
        private static readonly Type s_gameViewSizes = s_editorAssembly.GetType("UnityEditor.GameViewSizes");

        private static readonly PropertyInfo s_instanceProperty = typeof(ScriptableSingleton<>)
            .MakeGenericType(s_gameViewSizes)
            .GetProperty("instance", BindingFlags.Static | BindingFlags.Public);

        private static readonly PropertyInfo s_currentGroupMethod = s_gameViewSizes
            .GetProperty("currentGroup", BindingFlags.Instance | BindingFlags.Public);

        private readonly object _instance = s_instanceProperty.GetValue(null);

        public static GameViewSizesWrapper CreateInstance()
        {
            if (s_instanceProperty == null)
            {
                Debug.LogError("ScriptableSingleton<GameViewSizes>.instance property not found.");
                return null;
                // Note: Do not use Exception (and Assert). Because freezes async tests on UTF v1.3.4, See UUM-25085.
            }

            if (s_currentGroupMethod == null)
            {
                Debug.LogError("GameViewSizes.currentGroup method not found.");
                return null;
            }

            return new GameViewSizesWrapper();
        }

        public GameViewSizeGroupWrapper CurrentGroup()
        {
            var currentGroup = s_currentGroupMethod.GetValue(_instance);
            return GameViewSizeGroupWrapper.CreateInstance(currentGroup);
        }
    }
}
#endif
