// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEngine;

namespace TestHelper.Constraints
{
    /// <summary>
    /// Resolves an <c>actual</c> value passed to a layout constraint into a <see cref="RectTransform"/>.
    /// Accepts <see cref="RectTransform"/>, <see cref="GameObject"/>, and <see cref="Component"/>. Never
    /// throws; a resolution failure is reported through <paramref name="failureReason"/> instead.
    /// </summary>
    internal static class RectTransformResolver
    {
        internal static bool TryResolve(object actual, out RectTransform rectTransform, out string failureReason)
        {
            rectTransform = null;
            failureReason = null;

            if (actual == null)
            {
                failureReason = "null";
                return false;
            }

            // Checked before any type-specific branch: a destroyed RectTransform/GameObject/Component all
            // report identically here, and this guard runs before GetComponent, which would otherwise throw
            // MissingReferenceException on a destroyed object.
            if (actual is Object unityObject && !unityObject)
            {
                failureReason = "destroyed UnityEngine.Object";
                return false;
            }

            if (actual is RectTransform rectTransformActual)
            {
                rectTransform = rectTransformActual;
                return true;
            }

            GameObject gameObject;
            if (actual is GameObject gameObjectActual)
            {
                gameObject = gameObjectActual;
            }
            else if (actual is Component componentActual)
            {
                gameObject = componentActual.gameObject;
            }
            else
            {
                failureReason =
                    $"{ConstraintMessageFormatter.DescribeActual(actual)} is not a RectTransform, GameObject, or Component";
                return false;
            }

            var found = gameObject.GetComponent<RectTransform>();
            if (found == null)
            {
                failureReason = $"{ConstraintMessageFormatter.Quote(gameObject)} has no RectTransform component";
                return false;
            }

            rectTransform = found;
            return true;
        }
    }
}
