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
        /// Create constraint to destroyed GameObject.
        /// </summary>
        public static DestroyedConstraint Destroyed => new DestroyedConstraint();

        /// <summary>
        /// Create constraint to <see cref="RectTransform"/> within the screen.
        /// </summary>
        public static WithinScreenConstraint WithinScreen => default;

        /// <summary>
        /// Create constraint to <see cref="RectTransform"/> fully within <paramref name="container"/>.
        /// </summary>
        /// <param name="container">Container to check containment against.</param>
        public static FullyWithinConstraint FullyWithin(RectTransform container) => default;

        /// <summary>
        /// Create constraint to a collection of <see cref="RectTransform"/> where any pair overlaps.
        /// </summary>
        public static OverlappingConstraint Overlapping => default;

        /// <summary>
        /// Create constraint to text overflowing its own <see cref="RectTransform"/>.
        /// </summary>
        public static TextOverflowingConstraint TextOverflowing => default;
    }
}
