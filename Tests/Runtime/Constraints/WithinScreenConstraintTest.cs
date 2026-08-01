// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace TestHelper.Constraints
{
    [SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
    public class WithinScreenConstraintTest
    {
        public enum ActualKind
        {
            RectTransform,
            GameObject,
            Component,
        }

        public enum VisibilityFactor
        {
            ClippingRectMask2DAncestor,
            DisabledCanvas,
            CanvasGroupAlphaZero,
            InactiveInHierarchy,
        }

        private CultureInfo _originalCulture;

        [SetUp]
        public void SetUp()
        {
            _originalCulture = Thread.CurrentThread.CurrentCulture;
        }

        [TearDown]
        public void TearDown()
        {
            Thread.CurrentThread.CurrentCulture = _originalCulture;
        }

        private static RectTransform CreateElement(string name, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            var canvasGameObject = new GameObject("Canvas", typeof(Canvas));
            canvasGameObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

            var gameObject = new GameObject(name, typeof(RectTransform));
            var rectTransform = (RectTransform)gameObject.transform;
            rectTransform.SetParent(canvasGameObject.transform, worldPositionStays: false);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.pivot = Vector2.zero;
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = sizeDelta;
            return rectTransform;
        }

        private static object AsActual(RectTransform rectTransform, ActualKind kind)
        {
            switch (kind)
            {
                case ActualKind.RectTransform:
                    return rectTransform;
                case ActualKind.GameObject:
                    return rectTransform.gameObject;
                case ActualKind.Component:
                    return rectTransform.gameObject.AddComponent<Image>();
                default:
                    return rectTransform;
            }
        }

        private static void ApplyVisibilityFactor(RectTransform element, VisibilityFactor factor)
        {
            switch (factor)
            {
                case VisibilityFactor.ClippingRectMask2DAncestor:
                    element.parent.gameObject.AddComponent<RectMask2D>();
                    break;
                case VisibilityFactor.DisabledCanvas:
                    element.GetComponentInParent<Canvas>().enabled = false;
                    break;
                case VisibilityFactor.CanvasGroupAlphaZero:
                    element.gameObject.AddComponent<CanvasGroup>().alpha = 0f;
                    break;
                case VisibilityFactor.InactiveInHierarchy:
                    element.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }

        private static string ExpectedWithinScreenLine =>
            $"RectTransform within screen (0, 0, {Screen.width}, {Screen.height})";

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsWithinScreen_ElementInsideScreen_Success()
        {
            var actual = CreateElement("Element", new Vector2(Screen.width / 4f, Screen.height / 4f),
                new Vector2(Screen.width / 4f, Screen.height / 4f));

            Assert.That(actual, Is.WithinScreen);
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsWithinScreen_AcceptedActualTypes_Success([Values] ActualKind kind)
        {
            var rectTransform = CreateElement("Element", new Vector2(Screen.width / 4f, Screen.height / 4f),
                new Vector2(Screen.width / 4f, Screen.height / 4f));
            var actual = AsActual(rectTransform, kind);

            Assert.That(actual, Is.WithinScreen);
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsWithinScreen_ElementExceedsRightEdge_Failure()
        {
            const float Width = 50f;
            const float Overshoot = 20f;
            var element = CreateElement("CardView", new Vector2(Screen.width - Width + Overshoot, 10f),
                new Vector2(Width, 50f));
            var elementScreenRect = new Rect(Screen.width - Width + Overshoot, 10f, Width, 50f);

            Assert.That(() =>
            {
                Assert.That(element, Is.WithinScreen);
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: {ExpectedWithinScreenLine}{Environment.NewLine}" +
                $"  But was:  \"CardView\" {ConstraintMessageFormatter.Format(elementScreenRect)} exceeds the right edge by {ConstraintMessageFormatter.Format(Overshoot)}px{Environment.NewLine}"));
        }

        [Test]
        [CreateScene]
        public void IsWithinScreen_ElementExceedsMultipleEdges_Failure()
        {
            const float Width = 50f;
            const float Height = 50f;
            var anchoredPosition = new Vector2(-12f, Screen.height - (Height - 7f));
            var element = CreateElement("CardView", anchoredPosition, new Vector2(Width, Height));
            var elementScreenRect = new Rect(anchoredPosition.x, anchoredPosition.y, Width, Height);

            Assert.That(() =>
            {
                Assert.That(element, Is.WithinScreen);
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: {ExpectedWithinScreenLine}{Environment.NewLine}" +
                $"  But was:  \"CardView\" {ConstraintMessageFormatter.Format(elementScreenRect)} exceeds the left edge by {ConstraintMessageFormatter.Format(12f)}px" +
                $" and the top edge by {ConstraintMessageFormatter.Format(7f)}px{Environment.NewLine}"));
        }

        [TestCase(0.0f)]
        [TestCase(0.5f)]
        [CreateScene]
        [Category("Acceptance")]
        public void IsWithinScreen_OvershootWithinDefaultTolerance_Success(float overshoot)
        {
            const float Width = 50f;
            var actual = CreateElement("Element", new Vector2(Screen.width - Width + overshoot, 10f),
                new Vector2(Width, 50f));

            Assert.That(actual, Is.WithinScreen);
        }

        [Test]
        [CreateScene]
        public void IsWithinScreen_OvershootExceedsDefaultTolerance_Failure()
        {
            const float Width = 50f;
            const float Overshoot = 5f;
            var element = CreateElement("CardView", new Vector2(Screen.width - Width + Overshoot, 10f),
                new Vector2(Width, 50f));
            var elementScreenRect = new Rect(Screen.width - Width + Overshoot, 10f, Width, 50f);

            Assert.That(() =>
            {
                Assert.That(element, Is.WithinScreen);
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: {ExpectedWithinScreenLine}{Environment.NewLine}" +
                $"  But was:  \"CardView\" {ConstraintMessageFormatter.Format(elementScreenRect)} exceeds the right edge by {ConstraintMessageFormatter.Format(Overshoot)}px{Environment.NewLine}"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsWithinScreen_OvershootWithinSpecifiedTolerance_Success()
        {
            const float Width = 50f;
            const float Overshoot = 1.5f;
            var actual = CreateElement("Element", new Vector2(Screen.width - Width + Overshoot, 10f),
                new Vector2(Width, 50f));

            Assert.That(actual, Is.WithinScreen.Within(2f));
        }

        [Test]
        [CreateScene]
        public void IsWithinScreen_OvershootExceedsSpecifiedTolerance_Failure()
        {
            const float Width = 50f;
            const float Overshoot = 5f;
            var element = CreateElement("CardView", new Vector2(Screen.width - Width + Overshoot, 10f),
                new Vector2(Width, 50f));
            var elementScreenRect = new Rect(Screen.width - Width + Overshoot, 10f, Width, 50f);

            Assert.That(() =>
            {
                Assert.That(element, Is.WithinScreen.Within(2f));
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: {ExpectedWithinScreenLine}{Environment.NewLine}" +
                $"  But was:  \"CardView\" {ConstraintMessageFormatter.Format(elementScreenRect)} exceeds the right edge by {ConstraintMessageFormatter.Format(Overshoot)}px{Environment.NewLine}"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsWithinScreen_SubPixelOvershootWithNegativeTolerance_Failure()
        {
            const float Width = 50f;
            const float Overshoot = 0.1f;
            var element = CreateElement("CardView", new Vector2(Screen.width - Width + Overshoot, 10f),
                new Vector2(Width, 50f));
            var elementScreenRect = new Rect(Screen.width - Width + Overshoot, 10f, Width, 50f);

            Assert.That(() =>
            {
                Assert.That(element, Is.WithinScreen.Within(-5f));
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: {ExpectedWithinScreenLine}{Environment.NewLine}" +
                $"  But was:  \"CardView\" {ConstraintMessageFormatter.Format(elementScreenRect)} exceeds the right edge by {ConstraintMessageFormatter.Format(Overshoot)}px{Environment.NewLine}"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsWithinScreen_OutOfScreenElementWithNonGeometricVisibilityFactor_Failure(
            [Values] VisibilityFactor factor)
        {
            const float Width = 50f;
            const float Overshoot = 20f;
            var element = CreateElement("CardView", new Vector2(Screen.width - Width + Overshoot, 10f),
                new Vector2(Width, 50f));
            var elementScreenRect = new Rect(Screen.width - Width + Overshoot, 10f, Width, 50f);
            ApplyVisibilityFactor(element, factor);

            Assert.That(() =>
            {
                Assert.That(element, Is.WithinScreen);
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: {ExpectedWithinScreenLine}{Environment.NewLine}" +
                $"  But was:  \"CardView\" {ConstraintMessageFormatter.Format(elementScreenRect)} exceeds the right edge by {ConstraintMessageFormatter.Format(Overshoot)}px{Environment.NewLine}"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsWithinScreen_UnderCommaDecimalCulture_Failure()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            const float Width = 50f;
            const float Overshoot = 20.5f;
            var element = CreateElement("CardView", new Vector2(Screen.width - Width + Overshoot, 10f),
                new Vector2(Width, 50f));
            var elementScreenRect = new Rect(Screen.width - Width + Overshoot, 10f, Width, 50f);

            Assert.That(() =>
            {
                Assert.That(element, Is.WithinScreen);
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: {ExpectedWithinScreenLine}{Environment.NewLine}" +
                $"  But was:  \"CardView\" {ConstraintMessageFormatter.Format(elementScreenRect)} exceeds the right edge by {ConstraintMessageFormatter.Format(Overshoot)}px{Environment.NewLine}"));
        }

        [Test]
        [Category("Acceptance")]
        public void IsWithinScreen_Null_Failure()
        {
            Assert.That(() =>
            {
                Assert.That(null, Is.WithinScreen);
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: {ExpectedWithinScreenLine}{Environment.NewLine}" +
                $"  But was:  null{Environment.NewLine}"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsWithinScreen_DestroyedGameObject_Failure()
        {
            var element = CreateElement("CardView", Vector2.zero, Vector2.zero);
            GameObject.DestroyImmediate(element.gameObject);

            Assert.That(() =>
            {
                Assert.That(element, Is.WithinScreen);
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: {ExpectedWithinScreenLine}{Environment.NewLine}" +
                $"  But was:  destroyed UnityEngine.Object{Environment.NewLine}"));
        }

        [Test]
        [Category("Acceptance")]
        public void IsWithinScreen_UnsupportedActualType_Failure()
        {
            Assert.That(() =>
            {
                // Not a swapped actual/expected: this constant IS the actual value under test, deliberately an
                // unsupported type, to exercise the "not a RectTransform, GameObject, or Component" failure path.
                Assert.That("not a RectTransform", Is.WithinScreen);
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: {ExpectedWithinScreenLine}{Environment.NewLine}" +
                $"  But was:  <System.String> is not a RectTransform, GameObject, or Component{Environment.NewLine}"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsWithinScreen_GameObjectWithoutRectTransform_Failure()
        {
            var gameObject = new GameObject("PlainObject");

            Assert.That(() =>
            {
                Assert.That(gameObject, Is.WithinScreen);
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: {ExpectedWithinScreenLine}{Environment.NewLine}" +
                $"  But was:  \"PlainObject\" has no RectTransform component{Environment.NewLine}"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotWithinScreen_ElementExceedsScreen_Success()
        {
            const float Width = 50f;
            const float Overshoot = 20f;
            var actual = CreateElement("Element", new Vector2(Screen.width - Width + Overshoot, 10f),
                new Vector2(Width, 50f));

            Assert.That(actual, Is.Not.WithinScreen()); // Note: Use it in method style when with operators
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotWithinScreen_ElementInsideScreen_Failure()
        {
            var element = CreateElement("Element", new Vector2(Screen.width / 4f, Screen.height / 4f),
                new Vector2(Screen.width / 4f, Screen.height / 4f));
            var elementScreenRect =
                new Rect(Screen.width / 4f, Screen.height / 4f, Screen.width / 4f, Screen.height / 4f);

            Assert.That(() =>
            {
                Assert.That(element, Is.Not.WithinScreen()); // Note: Use it in method style when with operators
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: not {ExpectedWithinScreenLine}{Environment.NewLine}" +
                $"  But was:  <\"Element\" {ConstraintMessageFormatter.Format(elementScreenRect)}>{Environment.NewLine}"));
        }

        [Test]
        [Category("Acceptance")]
        public void IsNotWithinScreen_Null_Success()
        {
            Assert.That(null, Is.Not.WithinScreen()); // Note: Use it in method style when with operators
        }
    }
}
