// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEngine;

namespace TestHelper.Constraints
{
    /// <summary>
    /// Resolves an <c>actual</c> value passed to a layout constraint into a <see cref="RectTransform"/>.
    /// Accepts <see cref="RectTransform"/>, <see cref="GameObject"/>, and <see cref="Component"/>.
    /// </summary>
    internal static class RectTransformResolver
    {
        internal static bool TryResolve(object actual, out RectTransform rectTransform, out string failureReason)
        {
            rectTransform = default;
            failureReason = default;
            return default;
        }
    }
}
