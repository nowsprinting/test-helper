// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace TestHelper.Constraints
{
    /// <summary>
    /// Method-style entry points for this package's custom constraints, for chains that don't start at
    /// <see cref="Is.Not"/> or <see cref="Is.All"/> (e.g. <c>Has.None.Destroyed()</c>) or that land on NUnit's
    /// own <see cref="ResolvableConstraintExpression"/> (e.g. <c>Is.Not.Null.And.Destroyed()</c>). When a
    /// chain does start at <see cref="Is.Not"/>/<see cref="Is.All"/>, prefer the property style exposed by
    /// <see cref="TestHelperConstraintExpression"/> instead, e.g. <c>Is.Not.Destroyed</c>.
    /// </summary>
    public static class ConstraintExtensions
    {
        /// <summary>
        /// Create constraint to destroyed <see cref="UnityEngine.Object"/>.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>constraint to destroyed <see cref="UnityEngine.Object"/></returns>
        public static DestroyedConstraint Destroyed(this ConstraintExpression expression)
        {
            var constraint = new DestroyedConstraint();
            expression.Append(constraint);
            return constraint;
        }

        /// <summary>
        /// Create constraint to <see cref="RectTransform"/> within the screen.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>constraint to <see cref="RectTransform"/> within the screen</returns>
        public static WithinScreenConstraint WithinScreen(this ConstraintExpression expression)
        {
            var constraint = new WithinScreenConstraint();
            expression.Append(constraint);
            return constraint;
        }

        /// <summary>
        /// Create constraint to <see cref="RectTransform"/> fully within <paramref name="container"/>.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="container">Container to check containment against.</param>
        /// <returns>constraint to <see cref="RectTransform"/> fully within <paramref name="container"/></returns>
        public static WithinContainerConstraint WithinContainer(this ConstraintExpression expression,
            RectTransform container)
        {
            var constraint = new WithinContainerConstraint(container);
            expression.Append(constraint);
            return constraint;
        }

        /// <summary>
        /// Obsolete. Use <see cref="WithinContainer(ConstraintExpression, RectTransform)"/> instead.
        /// </summary>
        [Obsolete("Use WithinContainer instead.")]
        public static WithinContainerConstraint FullyWithin(this ConstraintExpression expression,
            RectTransform container) =>
            expression.WithinContainer(container);

        /// <summary>
        /// Create constraint to a collection of <see cref="RectTransform"/> where any pair overlaps.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>constraint to a collection of <see cref="RectTransform"/> where any pair overlaps</returns>
        public static OverlappingConstraint Overlapping(this ConstraintExpression expression)
        {
            var constraint = new OverlappingConstraint();
            expression.Append(constraint);
            return constraint;
        }

        /// <summary>
        /// Create constraint to text overflowing its own <see cref="RectTransform"/>.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>constraint to text overflowing its own <see cref="RectTransform"/></returns>
        public static TextOverflowingConstraint TextOverflowing(this ConstraintExpression expression)
        {
            var constraint = new TextOverflowingConstraint();
            expression.Append(constraint);
            return constraint;
        }
    }
}
