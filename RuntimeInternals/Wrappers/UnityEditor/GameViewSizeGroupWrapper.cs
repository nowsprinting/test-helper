// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;

namespace TestHelper.RuntimeInternals.Wrappers.UnityEditor
{
    /// <summary>
    /// Wrapper class for <c>UnityEditor.GameViewSizeGroup</c>.
    /// This class can be used from the runtime code because it does not depend on test-framework.
    /// </summary>
    public class GameViewSizeGroupWrapper
    {
        private static readonly Assembly s_editorAssembly = Assembly.Load("UnityEditor.dll");
        private static readonly Type s_gameViewSizeGroup = s_editorAssembly.GetType("UnityEditor.GameViewSizeGroup");
        private static readonly MethodInfo s_getTotalCount = s_gameViewSizeGroup.GetMethod("GetTotalCount");
        private static readonly MethodInfo s_getGameViewSize = s_gameViewSizeGroup.GetMethod("GetGameViewSize");
        private static readonly MethodInfo s_addCustomSize = s_gameViewSizeGroup.GetMethod("AddCustomSize");
        private static readonly MethodInfo s_removeCustomSize = s_gameViewSizeGroup.GetMethod("RemoveCustomSize");

        private readonly object _instance;

        private GameViewSizeGroupWrapper(object gameViewSizeGroup)
        {
            _instance = gameViewSizeGroup;
        }

        public static GameViewSizeGroupWrapper CreateInstance(object gameViewSizeGroup)
        {
            if (s_gameViewSizeGroup == null)
            {
                Debug.LogError("GameViewSizeGroup type not found.");
                return null;
                // Note: Do not use Exception (and Assert). Because freezes async tests on UTF v1.3.4, See UUM-25085.
            }

            if (s_getTotalCount == null)
            {
                Debug.LogError("GetTotalCount method not found.");
                return null;
            }

            if (s_getGameViewSize == null)
            {
                Debug.LogError("GetGameViewSize method not found.");
                return null;
            }

            if (s_addCustomSize == null)
            {
                Debug.LogError("AddCustomSize method not found.");
                return null;
            }

            if (s_removeCustomSize == null)
            {
                Debug.LogError("RemoveCustomSize method not found.");
                return null;
            }

            return new GameViewSizeGroupWrapper(gameViewSizeGroup);
        }

        public int GetTotalCount()
        {
            var res = s_getTotalCount.Invoke(_instance, null);
            return (int)res;
        }

        public GameViewSizeWrapper GetGameViewSize(int index)
        {
            var res = s_getGameViewSize.Invoke(_instance, new object[] { index });
            return new GameViewSizeWrapper(res);
        }

        public void AddCustomSize(GameViewSizeWrapper size)
        {
            s_addCustomSize.Invoke(_instance, new object[] { size.GetInnerInstance() });
        }

        public void RemoveCustomSize(int index)
        {
            s_removeCustomSize.Invoke(_instance, new object[] { index });
        }

        /// <summary>
        /// Found same type, width, and height.
        /// </summary>
        public int IndexOf(GameViewSizeWrapper size)
        {
            for (var i = 0; i < GetTotalCount(); i++)
            {
                var gameViewSize = GetGameViewSize(i);
                if (gameViewSize.Equals(size))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
#endif
