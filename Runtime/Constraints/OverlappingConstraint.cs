// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
using NUnit.Framework.Constraints;

namespace TestHelper.Constraints
{
    /// <summary>
    /// An NUnit test constraint class to a collection of <see cref="UnityEngine.RectTransform"/> where any
    /// pair overlaps.
    /// </summary>
    public class OverlappingConstraint : Constraint
    {
        private float _tolerance;

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
        /// <exception cref="System.ArgumentNullException"><paramref name="ignoredGroup"/> is null.</exception>
        public OverlappingConstraint Ignoring(IEnumerable ignoredGroup)
        {
            return default;
        }

        /// <summary>
        /// Set the tolerance in pixels. Negative values are clamped to 0. Default is 0.5f.
        /// </summary>
        /// <param name="tolerance">Tolerance in pixels.</param>
        /// <returns>this</returns>
        public OverlappingConstraint Within(float tolerance)
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
