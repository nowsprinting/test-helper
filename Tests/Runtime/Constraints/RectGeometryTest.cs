// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;
using UnityEngine;

namespace TestHelper.Constraints
{
    public class RectGeometryTest
    {
        [Test]
        public void GetOvershoot_InnerInsideOuter_ReturnsNull()
        {
            var outer = new Rect(0f, 0f, 100f, 100f);
            var inner = new Rect(10f, 10f, 50f, 50f);

            var actual = RectGeometry.GetOvershoot(inner, outer, RectAxes.Both, 0.5f);

            Assert.That(actual, Is.Null);
        }

        [TestCase(0.0f)]
        [TestCase(0.5f)]
        public void GetOvershoot_OvershootWithinTolerance_ReturnsNull(float overshoot)
        {
            var outer = new Rect(0f, 0f, 100f, 100f);
            var inner = new Rect(60f, 10f, 40f + overshoot, 50f); // right edge overshoots by `overshoot`

            var actual = RectGeometry.GetOvershoot(inner, outer, RectAxes.Both, 0.5f);

            Assert.That(actual, Is.Null);
        }

        [Test]
        public void GetOvershoot_ExceedsLeftEdge_ReturnsLeftEdgeOvershootMessage()
        {
            var outer = new Rect(0f, 0f, 100f, 100f);
            var inner = new Rect(-12f, 10f, 50f, 50f); // left edge overshoots by 12.0px

            var actual = RectGeometry.GetOvershoot(inner, outer, RectAxes.Both, 0.5f);

            Assert.That(actual, Is.EqualTo("exceeds the left edge by 12.0px"));
        }

        [Test]
        public void GetOvershoot_ExceedsRightEdge_ReturnsRightEdgeOvershootMessage()
        {
            var outer = new Rect(0f, 0f, 100f, 100f);
            var inner = new Rect(50f, 10f, 65f, 50f); // right edge overshoots by 15.0px

            var actual = RectGeometry.GetOvershoot(inner, outer, RectAxes.Both, 0.5f);

            Assert.That(actual, Is.EqualTo("exceeds the right edge by 15.0px"));
        }

        [Test]
        public void GetOvershoot_ExceedsTopEdge_ReturnsTopEdgeOvershootMessage()
        {
            var outer = new Rect(0f, 0f, 100f, 100f);
            var inner = new Rect(10f, 40f, 50f, 67f); // top edge overshoots by 7.0px

            var actual = RectGeometry.GetOvershoot(inner, outer, RectAxes.Both, 0.5f);

            Assert.That(actual, Is.EqualTo("exceeds the top edge by 7.0px"));
        }

        [Test]
        public void GetOvershoot_ExceedsBottomEdge_ReturnsBottomEdgeOvershootMessage()
        {
            var outer = new Rect(0f, 0f, 100f, 100f);
            var inner = new Rect(10f, -9f, 50f, 50f); // bottom edge overshoots by 9.0px

            var actual = RectGeometry.GetOvershoot(inner, outer, RectAxes.Both, 0.5f);

            Assert.That(actual, Is.EqualTo("exceeds the bottom edge by 9.0px"));
        }

        [Test]
        public void GetOvershoot_ExceedsMultipleEdges_ReturnsMessageForAllExceededEdges()
        {
            var outer = new Rect(0f, 0f, 100f, 100f);
            var inner = new Rect(-12f, 40f, 50f, 67f); // left edge by 12.0px, top edge by 7.0px

            var actual = RectGeometry.GetOvershoot(inner, outer, RectAxes.Both, 0.5f);

            Assert.That(actual, Is.EqualTo("exceeds the left edge by 12.0px and the top edge by 7.0px"));
        }

        [Test]
        public void GetOvershoot_HorizontalAxisOnly_ExceedsVerticalBoundsOnly_ReturnsNull()
        {
            var outer = new Rect(0f, 0f, 100f, 100f);
            var inner = new Rect(10f, -50f, 50f, 300f); // exceeds top and bottom edges, horizontally inside

            var actual = RectGeometry.GetOvershoot(inner, outer, RectAxes.Horizontal, 0.5f);

            Assert.That(actual, Is.Null);
        }

        [Test]
        public void GetOvershoot_VerticalAxisOnly_ExceedsHorizontalBoundsOnly_ReturnsNull()
        {
            var outer = new Rect(0f, 0f, 100f, 100f);
            var inner = new Rect(-50f, 10f, 300f, 50f); // exceeds left and right edges, vertically inside

            var actual = RectGeometry.GetOvershoot(inner, outer, RectAxes.Vertical, 0.5f);

            Assert.That(actual, Is.Null);
        }

        [Test]
        public void GetOverlapExtent_OverlappingRects_ReturnsPositiveExtentOnBothAxes()
        {
            var a = new Rect(0f, 0f, 100f, 100f);
            var b = new Rect(50f, 50f, 100f, 100f); // overlapping region is 50x50

            var actual = RectGeometry.GetOverlapExtent(a, b);

            Assert.That(actual, Is.EqualTo(new Vector2(50f, 50f)));
        }

        [Test]
        public void GetOverlapExtent_TouchingRects_ReturnsZeroExtentOnTouchingAxis()
        {
            var a = new Rect(0f, 0f, 100f, 100f);
            var b = new Rect(100f, 0f, 100f, 100f); // touching at x = 100, fully overlapping on y

            var actual = RectGeometry.GetOverlapExtent(a, b);

            Assert.That(actual.x, Is.EqualTo(0f));
        }

        [Test]
        public void GetOverlapExtent_SeparatedRects_ReturnsNegativeExtentOnSeparatedAxis()
        {
            var a = new Rect(0f, 0f, 50f, 50f);
            var b = new Rect(80f, 0f, 50f, 50f); // 30px gap on x, fully overlapping on y

            var actual = RectGeometry.GetOverlapExtent(a, b);

            Assert.That(actual.x, Is.EqualTo(-30f));
        }

        [Test]
        public void Overlaps_ExtentExceedsToleranceOnBothAxes_ReturnsTrue()
        {
            var a = new Rect(0f, 0f, 100f, 100f);
            var b = new Rect(90f, 90f, 100f, 100f); // overlap extent (10, 10)

            var actual = RectGeometry.Overlaps(a, b, 0.5f);

            Assert.That(actual, Is.True);
        }

        [TestCase(0.0f)]
        [TestCase(0.5f)]
        public void Overlaps_ExtentWithinToleranceOnHorizontalAxis_ReturnsFalse(float extent)
        {
            var a = new Rect(0f, 0f, 100f, 100f);
            var b = new Rect(100f - extent, 0f, 100f, 100f); // x overlap = `extent`, y fully overlapping

            var actual = RectGeometry.Overlaps(a, b, 0.5f);

            Assert.That(actual, Is.False);
        }

        [TestCase(0.0f)]
        [TestCase(0.5f)]
        public void Overlaps_ExtentWithinToleranceOnVerticalAxis_ReturnsFalse(float extent)
        {
            var a = new Rect(0f, 0f, 100f, 100f);
            var b = new Rect(0f, 100f - extent, 100f, 100f); // y overlap = `extent`, x fully overlapping

            var actual = RectGeometry.Overlaps(a, b, 0.5f);

            Assert.That(actual, Is.False);
        }

        [Test]
        public void Overlaps_SeparatedRects_ReturnsFalse()
        {
            var a = new Rect(0f, 0f, 50f, 50f);
            var b = new Rect(100f, 100f, 50f, 50f); // clearly separated on both axes

            var actual = RectGeometry.Overlaps(a, b, 0.5f);

            Assert.That(actual, Is.False);
        }
    }
}
