// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace TestHelper.Constraints
{
    /// <summary>
    /// Geometry predicates shared by the layout constraints: containment overshoot and overlap extent.
    /// </summary>
    internal static class RectGeometry
    {
        /// <summary>
        /// Returns null when <paramref name="inner"/> is contained in <paramref name="outer"/> on the given
        /// <paramref name="axes"/> within <paramref name="tolerance"/>; otherwise a description of every
        /// exceeded edge, e.g. "exceeds the left edge by 12.0px and the top edge by 7.0px".
        /// </summary>
        internal static string GetOvershoot(Rect inner, Rect outer, RectAxes axes, float tolerance)
        {
            // RectAxes.None means "not narrowed" and is treated the same as Both.
            var checkHorizontal = axes == RectAxes.None || (axes & RectAxes.Horizontal) != 0;
            var checkVertical = axes == RectAxes.None || (axes & RectAxes.Vertical) != 0;

            var clauses = new List<string>();

            if (checkHorizontal)
            {
                var left = outer.xMin - inner.xMin;
                if (left > tolerance)
                {
                    clauses.Add($"the left edge by {ConstraintMessageFormatter.Format(left)}px");
                }

                var right = inner.xMax - outer.xMax;
                if (right > tolerance)
                {
                    clauses.Add($"the right edge by {ConstraintMessageFormatter.Format(right)}px");
                }
            }

            if (checkVertical)
            {
                // Screen space is Y-up, so the "top" edge is the larger-Y (yMax) side.
                var top = inner.yMax - outer.yMax;
                if (top > tolerance)
                {
                    clauses.Add($"the top edge by {ConstraintMessageFormatter.Format(top)}px");
                }

                var bottom = outer.yMin - inner.yMin;
                if (bottom > tolerance)
                {
                    clauses.Add($"the bottom edge by {ConstraintMessageFormatter.Format(bottom)}px");
                }
            }

            return clauses.Count == 0 ? null : "exceeds " + string.Join(" and ", clauses);
        }

        /// <summary>
        /// Overlap extent per axis: min(aMax,bMax) - max(aMin,bMin). Zero or negative means touching/separated.
        /// </summary>
        internal static Vector2 GetOverlapExtent(Rect a, Rect b)
        {
            var x = Mathf.Min(a.xMax, b.xMax) - Mathf.Max(a.xMin, b.xMin);
            var y = Mathf.Min(a.yMax, b.yMax) - Mathf.Max(a.yMin, b.yMin);
            return new Vector2(x, y);
        }

        /// <summary>
        /// True when the overlap extent exceeds <paramref name="tolerance"/> on both axes. Does not use
        /// <see cref="Rect.Overlaps(Rect)"/>, which has no epsilon and would false-fail on sub-pixel error
        /// for adjacent, zero-gap layout cells.
        /// </summary>
        internal static bool Overlaps(Rect a, Rect b, float tolerance)
        {
            var extent = GetOverlapExtent(a, b);
            return extent.x > tolerance && extent.y > tolerance;
        }
    }
}
