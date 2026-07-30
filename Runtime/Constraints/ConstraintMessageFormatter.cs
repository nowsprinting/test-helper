// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Globalization;
using UnityEngine;

namespace TestHelper.Constraints
{
    /// <summary>
    /// Formats rects, vectors, and actual values for constraint failure messages using a fixed,
    /// culture-invariant, single-decimal representation.
    /// </summary>
    internal static class ConstraintMessageFormatter
    {
        internal static string Format(Rect rect)
        {
            return $"({Format(rect.x)}, {Format(rect.y)}, {Format(rect.width)}, {Format(rect.height)})";
        }

        /// <summary>
        /// Formats a rect with whole-number components, e.g. "(0, 0, 960, 540)" for the screen bounds.
        /// </summary>
        internal static string FormatIntegral(Rect rect)
        {
            return
                $"({FormatIntegral(rect.x)}, {FormatIntegral(rect.y)}, {FormatIntegral(rect.width)}, {FormatIntegral(rect.height)})";
        }

        internal static string Format(Vector2 vector)
        {
            return $"({Format(vector.x)}, {Format(vector.y)})";
        }

        internal static string Format(float value)
        {
            return value.ToString("F1", CultureInfo.InvariantCulture);
        }

        private static string FormatIntegral(float value)
        {
            return Mathf.RoundToInt(value).ToString(CultureInfo.InvariantCulture);
        }

        internal static string Quote(Object obj)
        {
            return $"\"{obj.name}\"";
        }

        /// <summary>
        /// Describes an actual value that could not be resolved to a supported type. Not a call to
        /// <c>MsgUtils.FormatValue</c> (internal in this NUnit build) or <c>ToString()</c> because the
        /// value could be arbitrarily large or locale-dependent; only its type name is reported.
        /// </summary>
        internal static string DescribeActual(object actual)
        {
            return actual == null ? "null" : $"<{actual.GetType().FullName}>";
        }
    }
}
