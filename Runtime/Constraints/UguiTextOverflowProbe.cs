// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

#if ENABLE_UGUI
using UnityEngine;
using UnityEngine.UI;

namespace TestHelper.Constraints
{
    /// <summary>
    /// Detects text overflow on a uGUI <see cref="Text"/> component.
    /// </summary>
    internal static class UguiTextOverflowProbe
    {
        internal static bool TryDetect(RectTransform rectTransform, out TextOverflowDetection detection)
        {
            var text = rectTransform.GetComponent<Text>();
            if (text == null)
            {
                detection = null;
                return false;
            }

            detection = new TextOverflowDetection();

            if (text.text.Length == 0)
            {
                // Nothing to overflow.
                return true;
            }

            // Read characterCount BEFORE preferredWidth/preferredHeight: those getters can populate the
            // text generator as a side effect, which would mask the "never laid out" state checked below.
            // Skipped under Truncate: a rect too small for even one line can legitimately generate zero
            // characters, which is a truncation result (handled below), not an unlaid-out one.
            var generator = text.cachedTextGenerator;
            if (text.verticalOverflow != VerticalWrapMode.Truncate && generator.characterCount == 0)
            {
                detection.NotLaidOut = true;
                return true;
            }

            // Truncation only removes characters under VerticalWrapMode.Truncate; under Overflow all
            // characters exist and merely spill, which only the size comparison below can detect.
            detection.CharactersTruncated = text.verticalOverflow == VerticalWrapMode.Truncate &&
                                            generator.characterCountVisible < text.text.Length;
            if (detection.CharactersTruncated)
            {
                detection.TruncationDetail =
                    $"only {generator.characterCountVisible} of {text.text.Length} characters are rendered";
            }

            if (text.resizeTextForBestFit)
            {
                // Best Fit shrinks the font to the rect, so comparing preferred size against the rect
                // would be meaningless; only the truncation check above still applies.
                detection.SkipReason = "best fit enabled";
                return true;
            }

            detection.PreferredSize = new Vector2(text.preferredWidth, text.preferredHeight);
            detection.RectSize = rectTransform.rect.size;
            detection.HeightChecked = true;

            // Text.preferredWidth reports the unwrapped width even when the text wraps correctly, so
            // comparing it against the rect width would false-fail correctly-wrapping text.
            detection.WidthChecked = text.horizontalOverflow == HorizontalWrapMode.Overflow;
            if (!detection.WidthChecked)
            {
                detection.SkipReason = "width check skipped (wrap mode)";
            }

            return true;
        }
    }
}
#endif
