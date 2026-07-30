// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework.Constraints;

namespace TestHelper.Constraints
{
    /// <summary>
    /// An NUnit test constraint class to text overflowing its own <see cref="UnityEngine.RectTransform"/>.
    /// Supports uGUI <see cref="UnityEngine.UI.Text"/> and TextMeshPro <see cref="TMPro.TMP_Text"/>.
    /// </summary>
    public class TextOverflowingConstraint : Constraint
    {
        private float _tolerance;

        public TextOverflowingConstraint(params object[] args) : base(args)
        {
        }

        /// <summary>
        /// Set the tolerance in pixels. Negative values are clamped to 0. Default is 0.5f.
        /// </summary>
        /// <param name="tolerance">Tolerance in pixels.</param>
        /// <returns>this</returns>
        public TextOverflowingConstraint Within(float tolerance)
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
