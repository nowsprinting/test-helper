// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace TestHelper.Constraints
{
    /// <summary>
    /// An NUnit test constraint class to a <see cref="RectTransform"/> fully within another
    /// <see cref="RectTransform"/>'s screen rect.
    /// </summary>
    public class FullyWithinConstraint : TestHelperConstraint
    {
        private const float DefaultTolerance = 0.5f;
        private readonly RectTransform _container;
        private RectAxes _axes;
        private float _tolerance = DefaultTolerance;

        public FullyWithinConstraint(RectTransform container) : base(container)
        {
            _container = container;
        }

        /// <summary>
        /// Narrow the check to the horizontal axis.
        /// </summary>
        /// <returns>this</returns>
        public FullyWithinConstraint Horizontally()
        {
            _axes |= RectAxes.Horizontal;
            return this;
        }

        /// <summary>
        /// Narrow the check to the vertical axis.
        /// </summary>
        /// <returns>this</returns>
        public FullyWithinConstraint Vertically()
        {
            _axes |= RectAxes.Vertical;
            return this;
        }

        /// <summary>
        /// Set the tolerance in pixels. Negative values are clamped to 0. Default is 0.5f.
        /// </summary>
        /// <param name="tolerance">Tolerance in pixels.</param>
        /// <returns>this</returns>
        public FullyWithinConstraint Within(float tolerance)
        {
            _tolerance = Mathf.Max(0f, tolerance);
            return this;
        }

        /// <inheritdoc/>
        public override string Description
        {
            get
            {
                if (_container == null)
                {
                    return "RectTransform fully within null or destroyed container";
                }

                // Both explicitly narrowed axes (Horizontally().Vertically()) are treated the same as
                // unnarrowed: only a SINGLE narrowed axis gets called out in the description.
                var axisWord = _axes == RectAxes.Horizontal ? "horizontally "
                    : _axes == RectAxes.Vertical ? "vertically "
                    : string.Empty;
                var containerRect = ScreenRectHelper.GetScreenRect(_container);
                return
                    $"RectTransform {axisWord}fully within {ConstraintMessageFormatter.Quote(_container)} {ConstraintMessageFormatter.Format(containerRect)}";
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">The container passed to the constructor is null or
        /// destroyed, or <paramref name="actual"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="actual"/> cannot be resolved to a
        /// <see cref="RectTransform"/>.</exception>
        public override ConstraintResult ApplyTo(object actual)
        {
            if (_container == null)
            {
                // ReSharper disable once NotResolvedInText -- reports the constructor's own parameter name;
                // ApplyTo has no local/parameter called "container" for nameof() to bind to.
                throw new ArgumentNullException("container");
            }

            var rectTransform = RectTransformResolver.ResolveOrThrow(actual, nameof(actual));

            var elementRect = ScreenRectHelper.GetScreenRect(rectTransform);
            var containerRect = ScreenRectHelper.GetScreenRect(_container);

            // RectAxes.None is treated the same as Both by RectGeometry.GetOvershoot, so no conversion needed.
            var overshoot = RectGeometry.GetOvershoot(elementRect, containerRect, _axes, _tolerance);
            var elementText =
                $"{ConstraintMessageFormatter.Quote(rectTransform)} {ConstraintMessageFormatter.Format(elementRect)}";
            var message = overshoot == null ? elementText : $"{elementText} {overshoot}";

            return new ReportingConstraintResult(this, new ConstraintReport(message), overshoot == null);
        }
    }
}
