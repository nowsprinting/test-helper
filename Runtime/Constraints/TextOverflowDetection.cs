// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEngine;

namespace TestHelper.Constraints
{
    /// <summary>
    /// Detection result produced by a text-overflow probe (uGUI or TextMeshPro) and scored by
    /// <see cref="TextOverflowingConstraint"/>.
    /// </summary>
    internal class TextOverflowDetection
    {
        internal string ComponentTypeName;
        internal Vector2 PreferredSize;
        internal Vector2 RectSize;
        internal bool WidthChecked;
        internal bool HeightChecked;
        internal string SkipReason;
        internal bool CharactersTruncated;
        internal string TruncationDetail;
        internal bool NotLaidOut;
        internal bool SizeExceeded;
        internal Vector2 Excess;

        internal bool IsOverflowing => default;
    }
}
