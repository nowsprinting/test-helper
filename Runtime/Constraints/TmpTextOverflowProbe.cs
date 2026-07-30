// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

#if ENABLE_TMP
using UnityEngine;

namespace TestHelper.Constraints
{
    /// <summary>
    /// Detects text overflow on a TextMeshPro <see cref="TMPro.TMP_Text"/> component.
    /// </summary>
    internal static class TmpTextOverflowProbe
    {
        internal static bool TryDetect(RectTransform rectTransform, out TextOverflowDetection detection)
        {
            detection = default;
            return default;
        }
    }
}
#endif
