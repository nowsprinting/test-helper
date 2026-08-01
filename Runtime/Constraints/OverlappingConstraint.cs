// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace TestHelper.Constraints
{
    /// <summary>
    /// An NUnit test constraint class to a collection of <see cref="UnityEngine.RectTransform"/> where any
    /// pair overlaps.
    /// </summary>
    public class OverlappingConstraint : Constraint
    {
        private const float DefaultTolerance = 0.5f;
        private readonly List<IEnumerable> _ignoredGroups = new List<IEnumerable>();
        private float _tolerance = DefaultTolerance;

        public OverlappingConstraint(params object[] args) : base(args)
        {
        }

        /// <summary>
        /// Exclude pairs whose both members belong to <paramref name="ignoredGroup"/> from the check.
        /// Members are still checked against elements outside the group. Can be called more than once to
        /// register multiple groups.
        /// </summary>
        /// <param name="ignoredGroup">Group of elements whose internal pairs are excluded.</param>
        /// <returns>this</returns>
        /// <exception cref="ArgumentNullException"><paramref name="ignoredGroup"/> is null.</exception>
        public OverlappingConstraint Ignoring(IEnumerable ignoredGroup)
        {
            if (ignoredGroup == null)
            {
                throw new ArgumentNullException(nameof(ignoredGroup));
            }

            _ignoredGroups.Add(ignoredGroup);
            return this;
        }

        /// <summary>
        /// Set the tolerance in pixels. Negative values are clamped to 0. Default is 0.5f.
        /// </summary>
        /// <param name="tolerance">Tolerance in pixels.</param>
        /// <returns>this</returns>
        public OverlappingConstraint Within(float tolerance)
        {
            _tolerance = Mathf.Max(0f, tolerance);
            return this;
        }

        /// <inheritdoc/>
        public override string Description => "any pair of RectTransforms overlapping";

        /// <inheritdoc/>
        public override ConstraintResult ApplyTo(object actual)
        {
            if (actual == null)
            {
                return new ReportingConstraintResult(this, null, false);
            }

            // Checked before the general IEnumerable branch: RectTransform (a Transform) enumerates its
            // children, so a single element passed directly would otherwise be misread as a collection.
            if (RectTransformResolver.IsResolvableSingle(actual))
            {
                RectTransformResolver.TryResolve(actual, out var singleRectTransform, out var singleFailureReason);
                var message = singleRectTransform != null
                    ? $"{ConstraintMessageFormatter.Quote(singleRectTransform)} is a single RectTransform, not a collection"
                    : singleFailureReason;
                return new ReportingConstraintResult(this, new ConstraintReport(message), false);
            }

            // string implements IEnumerable<char>; special-cased so it doesn't get misread as a collection
            // of per-character "elements".
            if (!(actual is IEnumerable) || actual is string)
            {
                var message =
                    $"{ConstraintMessageFormatter.DescribeActual(actual)} is not a collection of RectTransforms";
                return new ReportingConstraintResult(this, new ConstraintReport(message), false);
            }

            if (!TryResolveAll((IEnumerable)actual, "element", out var elements, out var elementsFailure))
            {
                return elementsFailure;
            }

            var ignoredSets = new List<HashSet<RectTransform>>();
            foreach (var rawGroup in _ignoredGroups)
            {
                if (!TryResolveAll(rawGroup, "ignored group member", out var groupMembers, out var groupFailure))
                {
                    return groupFailure;
                }

                ignoredSets.Add(new HashSet<RectTransform>(groupMembers));
            }

            // Computed once per element (not once per pair) so an element isn't re-projected to screen
            // space for every other element it's paired against.
            var rects = new Rect[elements.Count];
            for (var i = 0; i < elements.Count; i++)
            {
                rects[i] = ScreenRectHelper.GetScreenRect(elements[i]);
            }

            var overlappingPairs = new List<OverlappingPair>();
            for (var i = 0; i < elements.Count; i++)
            {
                for (var j = i + 1; j < elements.Count; j++)
                {
                    var a = elements[i];
                    var b = elements[j];
                    if (ignoredSets.Exists(set => set.Contains(a) && set.Contains(b)))
                    {
                        continue;
                    }

                    if (RectGeometry.Overlaps(rects[i], rects[j], _tolerance))
                    {
                        overlappingPairs.Add(new OverlappingPair(a, b, rects[i], rects[j]));
                    }
                }
            }

            if (overlappingPairs.Count > 0)
            {
                var first = overlappingPairs[0];
                var pairMessage =
                    $"{ConstraintMessageFormatter.Quote(first.A)} {ConstraintMessageFormatter.Format(first.RectA)}" +
                    $" overlaps {ConstraintMessageFormatter.Quote(first.B)} {ConstraintMessageFormatter.Format(first.RectB)}";
                if (overlappingPairs.Count > 1)
                {
                    pairMessage += $" (and {overlappingPairs.Count - 1} more overlapping pairs)";
                }

                return new ReportingConstraintResult(this, new ConstraintReport(pairMessage), true);
            }

            var noOverlapMessage = $"no overlapping pair among {elements.Count} RectTransforms";
            return new ReportingConstraintResult(this, new ConstraintReport(noOverlapMessage), false);
        }

        /// <summary>
        /// Resolves every item in <paramref name="source"/> to a <see cref="RectTransform"/>. On the first
        /// unresolvable item, returns false with <paramref name="failure"/> set to a ready failure result
        /// whose message is prefixed with <paramref name="label"/> and the item's index (shared by both the
        /// checked-elements loop and each ignored-group loop, which differ only in that prefix).
        /// </summary>
        private bool TryResolveAll(IEnumerable source, string label, out List<RectTransform> resolved,
            out ConstraintResult failure)
        {
            resolved = new List<RectTransform>();
            var index = 0;
            foreach (var item in source)
            {
                if (!RectTransformResolver.TryResolve(item, out var rectTransform, out var failureReason))
                {
                    var message = $"{label} at index {index}: {failureReason}";
                    failure = new ReportingConstraintResult(this, new ConstraintReport(message), false);
                    return false;
                }

                resolved.Add(rectTransform);
                index++;
            }

            failure = null;
            return true;
        }

        private readonly struct OverlappingPair
        {
            internal readonly RectTransform A;
            internal readonly RectTransform B;
            internal readonly Rect RectA;
            internal readonly Rect RectB;

            internal OverlappingPair(RectTransform a, RectTransform b, Rect rectA, Rect rectB)
            {
                A = a;
                B = b;
                RectA = rectA;
                RectB = rectB;
            }
        }
    }
}
