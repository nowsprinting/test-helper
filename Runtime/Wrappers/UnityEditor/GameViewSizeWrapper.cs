// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;

namespace TestHelper.Wrappers.UnityEditor
{
    /// <summary>
    /// Wrapper for <c>UnityEditor.GameViewSize</c>.
    /// </summary>
    public class GameViewSizeWrapper
    {
        private static readonly Assembly s_editorAssembly = Assembly.Load("UnityEditor.dll");

        private static readonly Type s_gameViewSize = s_editorAssembly.GetType("UnityEditor.GameViewSize");

        private static readonly FieldInfo s_gameViewSizeGameViewSizeType =
            s_gameViewSize.GetField("m_SizeType", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo s_gameViewSizeWidth =
            s_gameViewSize.GetField("m_Width", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo s_gameViewSizeHeight =
            s_gameViewSize.GetField("m_Height", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly Type s_gameViewSizeType = s_editorAssembly.GetType("UnityEditor.GameViewSizeType");
        private static readonly object s_typeFixedResolution = Enum.Parse(s_gameViewSizeType, "FixedResolution");

        private static readonly ConstructorInfo s_gameViewSizeConstructor = s_gameViewSize.GetConstructor(new[]
        {
            s_gameViewSizeType, typeof(int), typeof(int), typeof(string)
        });

        private readonly object _instance;
        private readonly int _width;
        private readonly int _height;

        internal GameViewSizeWrapper(object instance)
        {
            _instance = instance;
            if (_instance == null)
            {
                return;
            }

            _width = (int)s_gameViewSizeWidth.GetValue(_instance);
            _height = (int)s_gameViewSizeHeight.GetValue(_instance);
        }

        internal object GetInnerInstance()
        {
            return _instance;
        }

        public static GameViewSizeWrapper CreateInstance(int width, int height, string baseText)
        {
            if (s_gameViewSize == null)
            {
                Debug.LogError("GameViewSize type not found.");
                return null;
                // Note: Do not use Exception (and Assert). Because freezes async tests on UTF v1.3.4, See UUM-25085.
            }

            if (s_gameViewSizeGameViewSizeType == null)
            {
                Debug.LogError("GameViewSize.m_SizeType field not found.");
                return null;
            }

            if (s_gameViewSizeWidth == null)
            {
                Debug.LogError("GameViewSize.m_Width field not found.");
                return null;
            }

            if (s_gameViewSizeHeight == null)
            {
                Debug.LogError("GameViewSize.m_Height field not found.");
                return null;
            }

            if (s_typeFixedResolution == null)
            {
                Debug.LogError("GameViewSizeType.FixedResolution not found.");
                return null;
            }

            if (s_gameViewSizeConstructor == null)
            {
                Debug.LogError("GameViewSize constructor not found.");
                return null;
            }

            var instance = s_gameViewSizeConstructor.Invoke(new[] { s_typeFixedResolution, width, height, baseText });
            if (instance == null)
            {
                Debug.LogError("GameViewSize instance creation failed.");
                return null;
            }

            return new GameViewSizeWrapper(instance);
        }

        public string DisplayText()
        {
            var displayTextProperty = s_gameViewSize.GetProperty("displayText");
            if (displayTextProperty == null)
            {
                throw new NullReferenceException("GameViewSize.displayText property not found.");
            }

            var displayText = displayTextProperty.GetValue(_instance);
            return displayText as string;
        }

        /// <summary>
        /// Same type, width, and height.
        /// </summary>
        public bool Equals(GameViewSizeWrapper other)
        {
            if (other?.GetInnerInstance() == null)
            {
                return false;
            }

            var otherType = s_gameViewSizeGameViewSizeType.GetValue(other.GetInnerInstance());
            if (!otherType.Equals(s_typeFixedResolution))
            {
                return false;
            }

            var otherWidth = (int)s_gameViewSizeWidth.GetValue(other.GetInnerInstance());
            var otherHeight = (int)s_gameViewSizeHeight.GetValue(other.GetInnerInstance());
            return otherWidth == _width && otherHeight == _height;
        }
    }
}
#endif
