// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEngine;

namespace TestHelper.Constraints
{
    /// <summary>
    /// Geometry predicates shared by the layout constraints: containment overshoot and overlap extent.
    /// </summary>
    internal static class RectGeometry
    {
        internal static string GetOvershoot(Rect inner, Rect outer, RectAxes axes, float tolerance)
        {
            return default;
        }

        internal static Vector2 GetOverlapExtent(Rect a, Rect b)
        {
            return default;
        }

        internal static bool Overlaps(Rect a, Rect b, float tolerance)
        {
            return default;
        }
    }
}
