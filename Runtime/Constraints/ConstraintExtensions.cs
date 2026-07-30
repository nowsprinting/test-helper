// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework.Constraints;
using UnityEngine;

namespace TestHelper.Constraints
{
    public static class ConstraintExtensions
    {
        /// <summary>
        /// Create constraint to destroyed <see cref="UnityEngine.Object"/>.
        /// When used with operators, use it in method style. e.g., `Is.Not.Destroyed()`
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
        /// When used with operators, use it in method style. e.g., `Is.Not.WithinScreen()`
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>constraint to <see cref="RectTransform"/> within the screen</returns>
        public static WithinScreenConstraint WithinScreen(this ConstraintExpression expression)
        {
            return default;
        }

        /// <summary>
        /// Create constraint to <see cref="RectTransform"/> fully within <paramref name="container"/>.
        /// When used with operators, use it in method style. e.g., `Is.Not.FullyWithin(container)`
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="container">Container to check containment against.</param>
        /// <returns>constraint to <see cref="RectTransform"/> fully within <paramref name="container"/></returns>
        public static FullyWithinConstraint FullyWithin(this ConstraintExpression expression, RectTransform container)
        {
            return default;
        }

        /// <summary>
        /// Create constraint to a collection of <see cref="RectTransform"/> where any pair overlaps.
        /// When used with operators, use it in method style. e.g., `Is.Not.Overlapping()`
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>constraint to a collection of <see cref="RectTransform"/> where any pair overlaps</returns>
        public static OverlappingConstraint Overlapping(this ConstraintExpression expression)
        {
            return default;
        }

        /// <summary>
        /// Create constraint to text overflowing its own <see cref="RectTransform"/>.
        /// When used with operators, use it in method style. e.g., `Is.Not.TextOverflowing()`
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>constraint to text overflowing its own <see cref="RectTransform"/></returns>
        public static TextOverflowingConstraint TextOverflowing(this ConstraintExpression expression)
        {
            return default;
        }
    }
}
