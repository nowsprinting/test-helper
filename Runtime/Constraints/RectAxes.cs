// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;

namespace TestHelper.Constraints
{
    /// <summary>
    /// Axes considered when checking whether a <see cref="UnityEngine.RectTransform"/> is fully within another.
    /// </summary>
    [Flags]
    internal enum RectAxes
    {
        /// <summary>Not narrowed. Treated the same as <see cref="Both"/>.</summary>
        None = 0,

        /// <summary>Check the horizontal axis only.</summary>
        Horizontal = 1,

        /// <summary>Check the vertical axis only.</summary>
        Vertical = 2,

        /// <summary>Check both axes.</summary>
        Both = Horizontal | Vertical,
    }
}
