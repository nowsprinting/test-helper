// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace TestHelper.Constraints
{
    /// <summary>
    /// An NUnit test constraint class to a <see cref="UnityEngine.RectTransform"/> within the screen.
    /// </summary>
    public class WithinScreenConstraint : TestHelperConstraint
    {
        private const float DefaultTolerance = 0.5f;
        private float _tolerance = DefaultTolerance;

        public WithinScreenConstraint(params object[] args) : base(args)
        {
        }

        /// <summary>
        /// Set the tolerance in pixels. Negative values are clamped to 0. Default is 0.5f.
        /// </summary>
        /// <param name="tolerance">Tolerance in pixels.</param>
        /// <returns>this</returns>
        public WithinScreenConstraint Within(float tolerance)
        {
            _tolerance = Math.Max(0f, tolerance);
            return this;
        }

        /// <inheritdoc/>
        public override string Description =>
            $"RectTransform within screen {ConstraintMessageFormatter.FormatIntegral(ScreenBounds)}";

        private static Rect ScreenBounds => new Rect(0f, 0f, Screen.width, Screen.height);

        /// <inheritdoc/>
        /// <exception cref="System.ArgumentNullException"><paramref name="actual"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="actual"/> cannot be resolved to a
        /// <see cref="RectTransform"/>.</exception>
        public override ConstraintResult ApplyTo(object actual)
        {
            var rectTransform = RectTransformResolver.ResolveOrThrow(actual, nameof(actual));

            var screenRect = ScreenRectHelper.GetScreenRect(rectTransform);
            var overshoot = RectGeometry.GetOvershoot(screenRect, ScreenBounds, RectAxes.Both, _tolerance);
            var elementText =
                $"{ConstraintMessageFormatter.Quote(rectTransform)} {ConstraintMessageFormatter.Format(screenRect)}";
            var message = overshoot == null ? elementText : $"{elementText} {overshoot}";

            return new ReportingConstraintResult(this, new ConstraintReport(message), overshoot == null);
        }
    }
}
