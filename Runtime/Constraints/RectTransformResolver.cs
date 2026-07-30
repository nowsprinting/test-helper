// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework.Constraints;
using UnityEngine;

namespace TestHelper.Constraints
{
    /// <summary>
    /// Resolves an <c>actual</c> value passed to a layout constraint into a <see cref="RectTransform"/>.
    /// Accepts <see cref="RectTransform"/>, <see cref="GameObject"/>, and <see cref="Component"/>. Never
    /// throws; a resolution failure is reported through an output failure-reason string instead.
    /// </summary>
    internal static class RectTransformResolver
    {
        /// <summary>
        /// True when <paramref name="actual"/> is a type <see cref="TryResolve"/> can resolve directly to a
        /// single <see cref="RectTransform"/> (i.e. <see cref="RectTransform"/>, <see cref="GameObject"/>, or
        /// <see cref="Component"/>), ignoring the null/destroyed cases. Used to distinguish "a single element
        /// was passed" from "a collection was passed" before enumerating.
        /// </summary>
        internal static bool IsResolvableSingle(object actual)
        {
            return actual is RectTransform || actual is GameObject || actual is Component;
        }

        /// <summary>
        /// Resolves <paramref name="actual"/> for a constraint's <c>ApplyTo</c>, handling the null-actual and
        /// resolution-failure cases uniformly. Returns null on success (with <paramref name="rectTransform"/>
        /// set); otherwise the ready failure <see cref="ConstraintResult"/> to return from <c>ApplyTo</c>.
        /// </summary>
        internal static ConstraintResult TryResolveOrFail(object actual, IConstraint constraint,
            out RectTransform rectTransform)
        {
            if (actual == null)
            {
                rectTransform = null;
                return new ReportingConstraintResult(constraint, null, false);
            }

            if (!TryResolve(actual, out rectTransform, out var failureReason))
            {
                return new ReportingConstraintResult(constraint, new ConstraintReport(failureReason), false);
            }

            return null;
        }

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
