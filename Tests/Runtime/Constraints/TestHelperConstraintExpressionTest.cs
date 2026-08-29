// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine;

namespace TestHelper.Constraints
{
    // Exercises TestHelperConstraintExpression's own plumbing (Not, All, and the "no Builder yet" branch of
    // ConstraintExtensions.AppendOperator), using Destroyed as the left-hand side. Per-constraint coverage of
    // each constraint's own And/Or/With wiring lives in that constraint's own test class (e.g.
    // WithinScreenConstraintTest), alongside property-style coverage of `Is.Not.<X>` itself.
    [SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
    public class TestHelperConstraintExpressionTest
    {
        private static GameObject CreateDestroyedObject()
        {
            var gameObject = new GameObject();
            GameObject.DestroyImmediate(gameObject);
            return gameObject;
        }

        private static RectTransform CreateOnScreenElement()
        {
            var canvasGameObject = new GameObject("Canvas", typeof(Canvas));
            canvasGameObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

            var gameObject = new GameObject("Element", typeof(RectTransform));
            var rectTransform = (RectTransform)gameObject.transform;
            rectTransform.SetParent(canvasGameObject.transform, worldPositionStays: false);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.pivot = Vector2.zero;
            rectTransform.anchoredPosition = new Vector2(Screen.width / 4f, Screen.height / 4f);
            rectTransform.sizeDelta = new Vector2(Screen.width / 4f, Screen.height / 4f);
            return rectTransform;
        }

        private static RectTransform CreateOffScreenElement()
        {
            var canvasGameObject = new GameObject("Canvas", typeof(Canvas));
            canvasGameObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

            const float Width = 50f;
            const float Overshoot = 20f;
            var gameObject = new GameObject("Element", typeof(RectTransform));
            var rectTransform = (RectTransform)gameObject.transform;
            rectTransform.SetParent(canvasGameObject.transform, worldPositionStays: false);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.pivot = Vector2.zero;
            rectTransform.anchoredPosition = new Vector2(Screen.width - Width + Overshoot, 10f);
            rectTransform.sizeDelta = new Vector2(Width, 50f);
            return rectTransform;
        }

        [Test]
        [CreateScene]
        public void IsAllDestroyed_AllItemsDestroyed_Success()
        {
            var actual = new[] { CreateDestroyedObject(), CreateDestroyedObject() };

            Assert.That(actual, Is.All.Destroyed);
        }

        [Test]
        [CreateScene]
        public void IsAllNotDestroyed_NoItemsDestroyed_Success()
        {
            var actual = new[] { new GameObject("Foo"), new GameObject("Bar") };

            Assert.That(actual, Is.All.Not.Destroyed);
        }

        [Test]
        [CreateScene]
        public void IsNotDestroyedAndNotNull_AliveObject_Success()
        {
            var actual = new GameObject("Foo");

            Assert.That(actual, Is.Not.Destroyed.And.Not.Null);
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotDestroyedAndWithinScreen_AliveOnScreenElement_Success()
        {
            var actual = CreateOnScreenElement();

            Assert.That(actual, Is.Not.Destroyed.And.WithinScreen);
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotDestroyedAndWithinScreen_OffScreenElement_Failure()
        {
            var actual = CreateOffScreenElement();

            Assert.That(() =>
            {
                Assert.That(actual, Is.Not.Destroyed.And.WithinScreen);
            }, Throws.TypeOf<AssertionException>()
                .With.Message.Contains("not destroyed UnityEngine.Object")
                .And.Message.Contains("RectTransform within screen"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsDestroyedOrWithinScreen_OnScreenAliveElement_Success()
        {
            // Left (Destroyed) fails and right (WithinScreen) passes, so both sides are actually evaluated.
            // Not a destroyed actual: WithinScreenConstraint throws on a destroyed RectTransform, which would
            // make this pass only by luck of OrConstraint's short-circuit, not because Or itself works.
            var actual = CreateOnScreenElement();

            Assert.That(actual, Is.Destroyed.Or.WithinScreen);
        }

        [Test]
        [CreateScene]
        public void IsNotDestroyedWithNotNull_AliveObject_Success()
        {
            var actual = new GameObject("Foo");

            Assert.That(actual, Is.Not.Destroyed.With.Not.Null);
        }
    }
}
