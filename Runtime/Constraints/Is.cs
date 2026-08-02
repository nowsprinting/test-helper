// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEngine;

namespace TestHelper.Constraints
{
    /// <inheritdoc />
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Is : UnityEngine.TestTools.Constraints.Is
    {
        /// <summary>
        /// Negates the constraint that follows. Hides <see cref="NUnit.Framework.Is.Not"/> so this package's
        /// custom constraints (<see cref="TestHelperConstraintExpression"/>) can follow in property style,
        /// e.g., <c>Is.Not.Destroyed</c>.
        /// </summary>
        public new static TestHelperConstraintExpression Not => new TestHelperConstraintExpression().Not;

        /// <summary>
        /// Requires the constraint that follows to be satisfied by every item of a collection. Hides
        /// <see cref="NUnit.Framework.Is.All"/> for the same reason as <see cref="Not"/>.
        /// </summary>
        public new static TestHelperConstraintExpression All => new TestHelperConstraintExpression().All;

        /// <summary>
        /// Create constraint to destroyed GameObject.
        /// </summary>
        public static DestroyedConstraint Destroyed => new DestroyedConstraint();

        /// <summary>
        /// Create constraint to <see cref="RectTransform"/> within the screen.
        /// </summary>
        public static WithinScreenConstraint WithinScreen => new WithinScreenConstraint();

        /// <summary>
        /// Create constraint to <see cref="RectTransform"/> fully within <paramref name="container"/>.
        /// </summary>
        /// <param name="container">Container to check containment against.</param>
        public static FullyWithinConstraint FullyWithin(RectTransform container) =>
            new FullyWithinConstraint(container);

        /// <summary>
        /// Create constraint to a collection of <see cref="RectTransform"/> where any pair overlaps.
        /// </summary>
        public static OverlappingConstraint Overlapping => new OverlappingConstraint();

        /// <summary>
        /// Create constraint to text overflowing its own <see cref="RectTransform"/>.
        /// </summary>
        public static TextOverflowingConstraint TextOverflowing => new TextOverflowingConstraint();
    }
}
