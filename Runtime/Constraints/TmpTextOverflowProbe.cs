// Copyright (c) 2023-2026 Koji Hasegawa.
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

            detection = new TextOverflowDetection();

            if (text.text.Length == 0)
            {
                // Nothing to overflow.
                return true;
            }

            // Under TextOverflowModes.Overflow, firstOverflowCharacterIndex is still populated to indicate
            // where text WOULD be cut, but no characters are actually hidden — only Truncate and Ellipsis
            // really drop them (Ellipsis replaces the tail with "…"), so only those two are treated as an
            // overflow signal here. Masking is excluded: it is a deliberate presentation mode combined with
            // RectMask2D; Page/Linked/ScrollRect deliberately show the continuation elsewhere.
            detection.CharactersTruncated =
                (text.overflowMode == TextOverflowModes.Truncate ||
                 text.overflowMode == TextOverflowModes.Ellipsis)
                && text.firstOverflowCharacterIndex >= 0;
            if (detection.CharactersTruncated)
            {
                detection.TruncationDetail =
                    $"characters from index {text.firstOverflowCharacterIndex} of {text.text.Length} are not rendered (overflowMode: {text.overflowMode})";
            }

            // The rendered values below come from textInfo, which is only populated by mesh generation.
            // Checked AFTER the truncation check above, not before: Truncate/Ellipsis in a rect too small
            // for even one line legitimately generates zero characters, which is a truncation result
            // (firstOverflowCharacterIndex >= 0 only after generation; it stays -1 while never laid out),
            // not an unlaid-out one. Known edge: text consisting solely of rich-text tags parses to zero
            // characters and reports NotLaidOut; acceptable, as there is nothing renderable to check anyway.
            if (!detection.CharactersTruncated
                && (text.textInfo == null || text.textInfo.characterCount == 0))
            {
                detection.NotLaidOut = true;
                return true;
            }

            if (text.enableAutoSizing)
            {
                // Auto Sizing shrinks the font to the rect, so the rendered size would trivially fit and
                // comparing it would be meaningless.
                detection.SkipReason = "auto size enabled";
                return true;
            }

            // renderedWidth/renderedHeight reflect the actual post-wrap layout (unlike preferredWidth, which
            // is always measured without wrapping), so both axes can be checked without reading the wrapping
            // mode — which cannot be read across the supported Unity range without an obsolete API.
            detection.MeasuredSize = new Vector2(text.renderedWidth, text.renderedHeight);

            // TMP_Text.margin shrinks the effective text area inside the rect; comparing against the raw
            // rect size would under-report overflow by the margin amount. Negative adjusted size (margins
            // larger than the rect) is left unclamped: the rendered size exceeds it, which correctly reads
            // as overflow.
            var margin = text.margin; // x=left, y=top, z=right, w=bottom
            detection.RectSize = new Vector2(
                rectTransform.rect.width - (margin.x + margin.z),
                rectTransform.rect.height - (margin.y + margin.w));
            detection.WidthChecked = true;
            detection.HeightChecked = true;

            return true;
        }
    }
}
#endif
