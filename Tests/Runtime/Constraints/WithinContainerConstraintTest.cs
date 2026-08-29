// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace TestHelper.Constraints
{
    public class WithinContainerConstraintTest
    {
        public enum ActualKind
        {
            RectTransform,
            GameObject,
            Component,
        }

        public enum ContainerState
        {
            Null,
            Destroyed,
        }

        private static readonly Vector2 ContainerPosition = new Vector2(10f, 10f);
        private static readonly Vector2 ContainerSize = new Vector2(200f, 150f);

        private static Rect ContainerScreenRect => new Rect(ContainerPosition, ContainerSize);

        private static RectTransform CreateContainer()
        {
            var canvasGameObject = new GameObject("Canvas", typeof(Canvas));
            canvasGameObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

            var gameObject = new GameObject("Viewport", typeof(RectTransform));
            var rectTransform = (RectTransform)gameObject.transform;
            rectTransform.SetParent(canvasGameObject.transform, worldPositionStays: false);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.pivot = Vector2.zero;
            rectTransform.anchoredPosition = ContainerPosition;
            rectTransform.sizeDelta = ContainerSize;
            return rectTransform;
        }

        private static RectTransform CreateElement(string name, RectTransform container, Vector2 localPosition,
            Vector2 size)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            var rectTransform = (RectTransform)gameObject.transform;
            rectTransform.SetParent(container, worldPositionStays: false);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.pivot = Vector2.zero;
            rectTransform.anchoredPosition = localPosition;
            rectTransform.sizeDelta = size;
            return rectTransform;
        }

        private static Rect ElementScreenRect(Vector2 localPosition, Vector2 size) =>
            new Rect(ContainerPosition + localPosition, size);

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

        private static RectTransform ContainerFor(ContainerState state)
        {
            if (state == ContainerState.Null)
            {
                return null;
            }

            var container = CreateContainer();
            GameObject.DestroyImmediate(container.gameObject);
            return container;
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsWithinContainer_ElementInsideContainer_Success()
        {
            var container = CreateContainer();
            var actual = CreateElement("Element", container, new Vector2(20f, 20f), new Vector2(100f, 80f));

            Assert.That(actual, Is.WithinContainer(container));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsWithinContainer_AcceptedActualTypes_Success([Values] ActualKind kind)
        {
            var container = CreateContainer();
            var rectTransform = CreateElement("Element", container, new Vector2(20f, 20f), new Vector2(100f, 80f));
            var actual = AsActual(rectTransform, kind);

            Assert.That(actual, Is.WithinContainer(container));
        }

        [Test]
        [CreateScene]
        public void IsWithinContainerAndNull_ElementInsideContainer_Failure()
        {
            // Left (WithinContainer) passes and right (Null) fails, so both sides are actually evaluated:
            // wiring And to OrOperator by mistake would make this pass instead.
            var container = CreateContainer();
            var actual = CreateElement("Element", container, new Vector2(20f, 20f), new Vector2(100f, 80f));

            Assert.That(() => { Assert.That(actual, Is.WithinContainer(container).And.Null); },
                Throws.TypeOf<AssertionException>());
        }

        [Test]
        [CreateScene]
        public void IsWithinContainerOrNotNull_ElementExceedsContainerRightEdge_Success()
        {
            // Left (WithinContainer) fails and right (Not.Null) passes, so both sides are actually evaluated:
            // wiring Or to AndOperator by mistake would make this fail instead.
            const float Overshoot = 30f;
            var container = CreateContainer();
            var localPosition = new Vector2(ContainerSize.x - 50f + Overshoot, 20f);
            var element = CreateElement("CardView", container, localPosition, new Vector2(50f, 60f));

            Assert.That(element, Is.WithinContainer(container).Or.Not.Null);
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsWithinContainer_ElementExceedsContainerRightEdge_Failure()
        {
            const float Overshoot = 30f;
            var container = CreateContainer();
            var localPosition = new Vector2(ContainerSize.x - 50f + Overshoot, 20f);
            var size = new Vector2(50f, 60f);
            var element = CreateElement("CardView", container, localPosition, size);

            Assert.That(() =>
            {
                Assert.That(element, Is.WithinContainer(container));
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: RectTransform fully within \"Viewport\" {ConstraintMessageFormatter.Format(ContainerScreenRect)}{Environment.NewLine}" +
                $"  But was:  \"CardView\" {ConstraintMessageFormatter.Format(ElementScreenRect(localPosition, size))} exceeds the right edge by {ConstraintMessageFormatter.Format(Overshoot)}px{Environment.NewLine}"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsWithinContainer_Horizontally_ElementExceedsVerticalBoundsOnly_Success()
        {
            var container = CreateContainer();
            var actual = CreateElement("Element", container, new Vector2(20f, -100f), new Vector2(100f, 500f));

            Assert.That(actual, Is.WithinContainer(container).Horizontally());
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsWithinContainer_Horizontally_ElementExceedsHorizontalBounds_Failure()
        {
            const float Overshoot = 30f;
            var container = CreateContainer();
            var localPosition = new Vector2(ContainerSize.x - 50f + Overshoot, 20f);
            var size = new Vector2(50f, 60f);
            var element = CreateElement("CardView", container, localPosition, size);

            Assert.That(() =>
            {
                Assert.That(element, Is.WithinContainer(container).Horizontally());
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: RectTransform horizontally fully within \"Viewport\" {ConstraintMessageFormatter.Format(ContainerScreenRect)}{Environment.NewLine}" +
                $"  But was:  \"CardView\" {ConstraintMessageFormatter.Format(ElementScreenRect(localPosition, size))} exceeds the right edge by {ConstraintMessageFormatter.Format(Overshoot)}px{Environment.NewLine}"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsWithinContainer_Vertically_ElementExceedsHorizontalBoundsOnly_Success()
        {
            var container = CreateContainer();
            var actual = CreateElement("Element", container, new Vector2(-100f, 20f), new Vector2(500f, 80f));

            Assert.That(actual, Is.WithinContainer(container).Vertically());
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsWithinContainer_Vertically_ElementExceedsVerticalBounds_Failure()
        {
            const float Overshoot = 15f;
            var container = CreateContainer();
            var localPosition = new Vector2(20f, ContainerSize.y - 60f + Overshoot);
            var size = new Vector2(50f, 60f);
            var element = CreateElement("CardView", container, localPosition, size);

            Assert.That(() =>
            {
                Assert.That(element, Is.WithinContainer(container).Vertically());
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: RectTransform vertically fully within \"Viewport\" {ConstraintMessageFormatter.Format(ContainerScreenRect)}{Environment.NewLine}" +
                $"  But was:  \"CardView\" {ConstraintMessageFormatter.Format(ElementScreenRect(localPosition, size))} exceeds the top edge by {ConstraintMessageFormatter.Format(Overshoot)}px{Environment.NewLine}"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsWithinContainer_HorizontallyAndVertically_ElementExceedsVerticalBounds_Failure()
        {
            const float Overshoot = 15f;
            var container = CreateContainer();
            var localPosition = new Vector2(20f, ContainerSize.y - 60f + Overshoot);
            var size = new Vector2(50f, 60f);
            var element = CreateElement("CardView", container, localPosition, size);

            Assert.That(() =>
            {
                Assert.That(element, Is.WithinContainer(container).Horizontally().Vertically());
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: RectTransform fully within \"Viewport\" {ConstraintMessageFormatter.Format(ContainerScreenRect)}{Environment.NewLine}" +
                $"  But was:  \"CardView\" {ConstraintMessageFormatter.Format(ElementScreenRect(localPosition, size))} exceeds the top edge by {ConstraintMessageFormatter.Format(Overshoot)}px{Environment.NewLine}"));
        }

        [TestCase(0.0f)]
        [TestCase(0.5f)]
        [CreateScene]
        public void IsWithinContainer_OvershootWithinDefaultTolerance_Success(float overshoot)
        {
            var container = CreateContainer();
            var actual = CreateElement("Element", container, new Vector2(ContainerSize.x - 50f + overshoot, 20f),
                new Vector2(50f, 60f));

            Assert.That(actual, Is.WithinContainer(container));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsWithinContainer_OvershootWithinSpecifiedTolerance_Success()
        {
            const float Overshoot = 1.5f;
            var container = CreateContainer();
            var actual = CreateElement("Element", container, new Vector2(ContainerSize.x - 50f + Overshoot, 20f),
                new Vector2(50f, 60f));

            Assert.That(actual, Is.WithinContainer(container).Within(2f));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsWithinContainer_NullOrDestroyedContainer_ThrowsArgumentNullException(
            [Values] ContainerState state)
        {
            var container = ContainerFor(state);
            var element = CreateElement("Element", CreateContainer(), Vector2.zero, Vector2.zero);

            Assert.That(() =>
            {
                Assert.That(element, Is.WithinContainer(container));
            }, Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("container"));
        }

        [Test]
        [CreateScene]
        public void IsWithinContainer_Null_ThrowsArgumentNullException()
        {
            var container = CreateContainer();

            Assert.That(() =>
            {
                Assert.That(null, Is.WithinContainer(container));
            }, Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("actual"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsWithinContainer_DestroyedGameObject_ThrowsArgumentException()
        {
            var container = CreateContainer();
            var element = CreateElement("Element", container, Vector2.zero, Vector2.zero);
            GameObject.DestroyImmediate(element.gameObject);

            Assert.That(() =>
            {
                Assert.That(element, Is.WithinContainer(container));
            }, Throws.TypeOf<ArgumentException>()
                .With.Property("ParamName").EqualTo("actual")
                .And.Message.Contains("destroyed UnityEngine.Object"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsWithinContainer_UnsupportedActualType_ThrowsArgumentException()
        {
            var container = CreateContainer();

            Assert.That(() =>
            {
                // Not a swapped actual/expected: this value IS the actual value under test, deliberately an
                // unsupported type, to exercise the "not a RectTransform, GameObject, or Component" failure path.
                object unsupportedActual = "not a RectTransform";
                Assert.That(unsupportedActual, Is.WithinContainer(container));
            }, Throws.TypeOf<ArgumentException>()
                .With.Property("ParamName").EqualTo("actual")
                .And.Message.Contains("is not a RectTransform, GameObject, or Component"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsWithinContainer_GameObjectWithoutRectTransform_ThrowsArgumentException()
        {
            var container = CreateContainer();
            var gameObject = new GameObject("PlainObject");

            Assert.That(() =>
            {
                Assert.That(gameObject, Is.WithinContainer(container));
            }, Throws.TypeOf<ArgumentException>()
                .With.Property("ParamName").EqualTo("actual")
                .And.Message.Contains("has no RectTransform component"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotWithinContainer_ElementExceedsContainer_Success()
        {
            const float Overshoot = 30f;
            var container = CreateContainer();
            var actual = CreateElement("Element", container, new Vector2(ContainerSize.x - 50f + Overshoot, 20f),
                new Vector2(50f, 60f));

            Assert.That(actual, Is.Not.WithinContainer(container));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotWithinContainer_ElementInsideContainer_Failure()
        {
            var container = CreateContainer();
            var localPosition = new Vector2(20f, 20f);
            var size = new Vector2(100f, 80f);
            var element = CreateElement("Element", container, localPosition, size);

            Assert.That(() =>
            {
                Assert.That(element, Is.Not.WithinContainer(container));
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: not RectTransform fully within \"Viewport\" {ConstraintMessageFormatter.Format(ContainerScreenRect)}{Environment.NewLine}" +
                $"  But was:  <\"Element\" {ConstraintMessageFormatter.Format(ElementScreenRect(localPosition, size))}>{Environment.NewLine}"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotWithinContainer_NullActual_ThrowsArgumentNullException()
        {
            var container = CreateContainer();

            Assert.That(() =>
            {
                Assert.That(null, Is.Not.WithinContainer(container));
            }, Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("actual"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotWithinContainer_NullContainer_ThrowsArgumentNullException()
        {
            var element = CreateElement("Element", CreateContainer(), Vector2.zero, Vector2.zero);

            Assert.That(() =>
            {
                Assert.That(element, Is.Not.WithinContainer(null));
            }, Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("container"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotWithinContainer_ChainedAxisAndToleranceModifiers_Failure()
        {
            var container = CreateContainer();
            var localPosition = new Vector2(20f, -100f); // vertically out of bounds, horizontally fine
            var size = new Vector2(100f, 500f);
            var element = CreateElement("Element", container, localPosition, size);

            Assert.That(() =>
            {
                Assert.That(element, Is.Not.WithinContainer(container).Horizontally().Within(2f));
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: not RectTransform horizontally fully within \"Viewport\" {ConstraintMessageFormatter.Format(ContainerScreenRect)}{Environment.NewLine}" +
                $"  But was:  <\"Element\" {ConstraintMessageFormatter.Format(ElementScreenRect(localPosition, size))}>{Environment.NewLine}"));
        }

        #region Obsoleted

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        [Obsolete("Use WithinContainer instead.")]
        public void IsFullyWithin_ElementExceedsContainerRightEdge_Failure()
        {
            const float Overshoot = 30f;
            var container = CreateContainer();
            var localPosition = new Vector2(ContainerSize.x - 50f + Overshoot, 20f);
            var size = new Vector2(50f, 60f);
            var element = CreateElement("CardView", container, localPosition, size);

            Assert.That(() =>
            {
                Assert.That(element, Is.FullyWithin(container));
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: RectTransform fully within \"Viewport\" {ConstraintMessageFormatter.Format(ContainerScreenRect)}{Environment.NewLine}" +
                $"  But was:  \"CardView\" {ConstraintMessageFormatter.Format(ElementScreenRect(localPosition, size))} exceeds the right edge by {ConstraintMessageFormatter.Format(Overshoot)}px{Environment.NewLine}"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        [Obsolete("Use WithinContainer instead.")]
        public void IsNotFullyWithin_ChainedAxisAndToleranceModifiers_Success()
        {
            const float Overshoot = 30f;
            var container = CreateContainer();
            var actual = CreateElement("Element", container, new Vector2(ContainerSize.x - 50f + Overshoot, 20f),
                new Vector2(50f, 60f));

            Assert.That(actual, Is.Not.FullyWithin(container).Horizontally().Within(2f));
        }

        #endregion
    }
}
