// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using UnityEngine;

namespace TestHelper.Constraints
{
    /// <summary>
    /// Resolves an <c>actual</c> value passed to a layout constraint into a <see cref="RectTransform"/>.
    /// Accepts <see cref="RectTransform"/>, <see cref="GameObject"/>, and <see cref="Component"/>.
    /// </summary>
    internal static class RectTransformResolver
    {
        /// <summary>
        /// True when <paramref name="actual"/> is a type <see cref="TryResolve"/> can resolve directly to a
        /// single <see cref="RectTransform"/> (i.e. <see cref="RectTransform"/>, <see cref="GameObject"/>, or
        /// <see cref="Component"/>), ignoring the null/destroyed cases. Used to distinguish "a single element
        /// was passed" from "a collection was passed" before enumerating. Never throws.
        /// </summary>
        internal static bool IsResolvableSingle(object actual)
        {
            return actual is RectTransform || actual is GameObject || actual is Component;
        }

        /// <summary>
        /// Resolves <paramref name="actual"/> to a <see cref="RectTransform"/> for a constraint's
        /// <c>ApplyTo</c>. A resolution failure cannot be reported as an ordinary non-match: the constraint
        /// has nothing to evaluate, so silently returning "not matched" would make a negated constraint
        /// (e.g. <c>Is.Not.WithinScreen</c>) vacuously pass on a null or unusable <paramref name="actual"/>.
        /// </summary>
        /// <param name="actual">Value to resolve.</param>
        /// <param name="paramName">Name reported on failure; does not need to be a real parameter of the
        /// caller (e.g. <c>"element at index 2"</c> for a collection member).</param>
        /// <exception cref="ArgumentNullException"><paramref name="actual"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="actual"/> is destroyed, is not a
        /// <see cref="RectTransform"/>, <see cref="GameObject"/>, or <see cref="Component"/>, or its
        /// GameObject has no <see cref="RectTransform"/> component.</exception>
        internal static RectTransform ResolveOrThrow(object actual, string paramName)
        {
            if (TryResolve(actual, out var rectTransform, out var failureReason))
            {
                return rectTransform;
            }

            if (actual == null)
            {
                throw new ArgumentNullException(paramName);
            }

            throw new ArgumentException(failureReason, paramName);
        }

        /// <summary>
        /// Never throws; a resolution failure is reported through <paramref name="failureReason"/> instead.
        /// </summary>
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
            if (actual is UnityEngine.Object unityObject && !unityObject)
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
