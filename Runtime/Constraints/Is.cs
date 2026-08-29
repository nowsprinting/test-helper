// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace TestHelper.Constraints
{
    /// <inheritdoc />
    // Not renamed: this class extends NUnit's own Is and must keep its name to read as Is.Destroyed,
    // Is.WithinScreen, etc., matching the NUnit constraint idiom (Is.EqualTo, Is.Not, ...).
    [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords")]
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
        public static WithinContainerConstraint WithinContainer(RectTransform container) =>
            new WithinContainerConstraint(container);

        /// <summary>
        /// Obsolete. Use <see cref="WithinContainer"/> instead.
        /// </summary>
        [Obsolete("Use WithinContainer instead.")]
        public static WithinContainerConstraint FullyWithin(RectTransform container) =>
            WithinContainer(container);

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
