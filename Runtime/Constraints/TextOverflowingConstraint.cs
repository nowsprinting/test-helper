// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace TestHelper.Constraints
{
    /// <summary>
    /// An NUnit test constraint class to text overflowing its own <see cref="UnityEngine.RectTransform"/>.
    /// Supports uGUI <see cref="UnityEngine.UI.Text"/> and TextMeshPro <see cref="TMPro.TMP_Text"/> (uGUI is
    /// tried first; whichever is found on the target's own GameObject is used).
    /// </summary>
    public class TextOverflowingConstraint : Constraint
    {
        private const float DefaultTolerance = 0.5f;
        private float _tolerance = DefaultTolerance;

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
            _tolerance = Mathf.Max(0f, tolerance);
            return this;
        }

        /// <inheritdoc/>
        public override string Description => "text overflowing its RectTransform";

        /// <inheritdoc/>
        public override ConstraintResult ApplyTo(object actual)
        {
            var failure = RectTransformResolver.TryResolveOrFail(actual, this, out var rectTransform);
            if (failure != null)
            {
                return failure;
            }

            var detection = Detect(rectTransform);
            if (detection == null)
            {
                var message = $"{ConstraintMessageFormatter.Quote(rectTransform)} has no Text or TMP_Text component";
                return new ReportingConstraintResult(this, new ConstraintReport(message), false);
            }

            EvaluateSize(detection);

            var elementName = ConstraintMessageFormatter.Quote(rectTransform);
            var reportMessage = detection.IsOverflowing
                ? BuildOverflowingMessage(elementName, detection)
                : BuildNotOverflowingMessage(elementName, detection);

            return new ReportingConstraintResult(this, new ConstraintReport(reportMessage), detection.IsOverflowing);
        }

        private static TextOverflowDetection Detect(RectTransform rectTransform)
        {
            TextOverflowDetection detection = null;
#if ENABLE_UGUI
            if (detection == null)
            {
                UguiTextOverflowProbe.TryDetect(rectTransform, out detection);
            }
#endif
#if ENABLE_TMP
            if (detection == null)
            {
                TmpTextOverflowProbe.TryDetect(rectTransform, out detection);
            }
#endif
            return detection;
        }

        private void EvaluateSize(TextOverflowDetection detection)
        {
            if (!detection.WidthChecked && !detection.HeightChecked)
            {
                return;
            }

            var excessX = detection.WidthChecked
                ? Mathf.Max(0f, detection.PreferredSize.x - detection.RectSize.x)
                : 0f;
            var excessY = detection.HeightChecked
                ? Mathf.Max(0f, detection.PreferredSize.y - detection.RectSize.y)
                : 0f;
            var exceedsX = detection.WidthChecked && excessX > _tolerance;
            var exceedsY = detection.HeightChecked && excessY > _tolerance;

            detection.SizeExceeded = exceedsX || exceedsY;
            detection.Excess = new Vector2(excessX, excessY);
        }

        private static string BuildOverflowingMessage(string elementName, TextOverflowDetection detection)
        {
            var clauses = new List<string>();
            if (detection.SizeExceeded)
            {
                clauses.Add(
                    $"preferred size {ConstraintMessageFormatter.Format(detection.PreferredSize)} exceeds rect {ConstraintMessageFormatter.Format(detection.RectSize)} by {ConstraintMessageFormatter.Format(detection.Excess)}");
            }

            if (detection.CharactersTruncated)
            {
                clauses.Add(detection.TruncationDetail);
            }

            if (detection.NotLaidOut)
            {
                clauses.Add("has not been laid out; call Canvas.ForceUpdateCanvases() before asserting");
            }

            return $"{elementName} {string.Join("; ", clauses)}";
        }

        private static string BuildNotOverflowingMessage(string elementName, TextOverflowDetection detection)
        {
            var notes = new List<string>();
            if (detection.WidthChecked || detection.HeightChecked)
            {
                notes.Add(
                    $"preferred size {ConstraintMessageFormatter.Format(detection.PreferredSize)} within rect {ConstraintMessageFormatter.Format(detection.RectSize)}");
            }

            if (!string.IsNullOrEmpty(detection.SkipReason))
            {
                notes.Add($"({detection.SkipReason})");
            }

            if (notes.Count == 0)
            {
                notes.Add("fits its rect");
            }

            return $"{elementName} {string.Join(" ", notes)}";
        }
    }
}
