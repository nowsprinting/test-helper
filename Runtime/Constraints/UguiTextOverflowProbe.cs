// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

#if ENABLE_UGUI
using UnityEngine;

namespace TestHelper.Constraints
{
    /// <summary>
    /// Detects text overflow on a uGUI <see cref="UnityEngine.UI.Text"/> component.
    /// </summary>
    internal static class UguiTextOverflowProbe
    {
        internal static bool TryDetect(RectTransform rectTransform, out TextOverflowDetection detection)
        {
            detection = default;
            return default;
        }
    }
}
#endif
