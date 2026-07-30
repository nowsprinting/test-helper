// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework.Constraints;
using UnityEngine;

namespace TestHelper.Constraints
{
    /// <summary>
    /// An NUnit test constraint class to a <see cref="RectTransform"/> fully within another
    /// <see cref="RectTransform"/>'s screen rect.
    /// </summary>
    public class FullyWithinConstraint : Constraint
    {
        private RectAxes _axes;
        private float _tolerance;

        public FullyWithinConstraint(RectTransform container) : base(container)
        {
        }

        /// <summary>
        /// Narrow the check to the horizontal axis.
        /// </summary>
        /// <returns>this</returns>
        public FullyWithinConstraint Horizontally()
        {
            return default;
        }

        /// <summary>
        /// Narrow the check to the vertical axis.
        /// </summary>
        /// <returns>this</returns>
        public FullyWithinConstraint Vertically()
        {
            return default;
        }

        /// <summary>
        /// Set the tolerance in pixels. Negative values are clamped to 0. Default is 0.5f.
        /// </summary>
        /// <param name="tolerance">Tolerance in pixels.</param>
        /// <returns>this</returns>
        public FullyWithinConstraint Within(float tolerance)
        {
            return default;
        }

        /// <inheritdoc/>
        public override string Description => default;

        /// <inheritdoc/>
        public override ConstraintResult ApplyTo(object actual)
        {
            return default;
        }
    }
}
