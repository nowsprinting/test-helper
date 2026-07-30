// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

#if ENABLE_TMP
using TMPro;
using UnityEngine;

namespace TestHelper.Constraints
{
    /// <summary>
    /// Detects text overflow on a TextMeshPro <see cref="TMP_Text"/> component.
    /// </summary>
    internal static class TmpTextOverflowProbe
    {
        internal static bool TryDetect(RectTransform rectTransform, out TextOverflowDetection detection)
        {
            var text = rectTransform.GetComponent<TMP_Text>();
            if (text == null)
            {
                detection = null;
                return false;
            }

            detection = new TextOverflowDetection { ComponentTypeName = "TMP_Text" };

            if (text.text.Length == 0)
            {
                // Nothing to overflow.
                return true;
            }

            // Under TextOverflowModes.Overflow, firstOverflowCharacterIndex is still populated to indicate
            // where text WOULD be cut, but no characters are actually hidden — only Truncate really drops
            // them, so only Truncate is treated as an overflow signal here.
            detection.CharactersTruncated =
                text.overflowMode == TextOverflowModes.Truncate && text.firstOverflowCharacterIndex >= 0;
            if (detection.CharactersTruncated)
            {
                var total = text.textInfo != null ? text.textInfo.characterCount : text.text.Length;
                detection.TruncationDetail =
                    $"characters from index {text.firstOverflowCharacterIndex} of {total} are not rendered (overflowMode: {text.overflowMode})";
            }

            if (text.enableAutoSizing)
            {
                // Auto Sizing shrinks the font to the rect, so comparing preferred height against the rect
                // would be meaningless.
                detection.SkipReason = "auto size enabled";
                return true;
            }

            detection.PreferredSize = new Vector2(text.preferredWidth, text.preferredHeight);
            detection.RectSize = rectTransform.rect.size;
            detection.HeightChecked = true;

            // TMP's preferred width is always measured without wrapping, and its wrapping mode cannot be
            // read across the supported Unity range without an obsolete API, so the width is never checked.
            detection.WidthChecked = false;
            detection.SkipReason = "TMP preferred width is measured without wrapping";

            return true;
        }
    }
}
#endif
