// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace TestHelper.Constraints
{
    [SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
    public class OverlappingConstraintTest
    {
        public enum ActualKind
        {
            RectTransform,
            GameObject,
            Component,
        }

        private static Canvas CreateCanvas()
        {
            var canvasGameObject = new GameObject("Canvas", typeof(Canvas));
            canvasGameObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            return canvasGameObject.GetComponent<Canvas>();
        }

        private static RectTransform CreateElement(Transform parent, string name, Vector2 anchoredPosition,
            Vector2 sizeDelta)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            var rectTransform = (RectTransform)gameObject.transform;
            rectTransform.SetParent(parent, worldPositionStays: false);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.pivot = Vector2.zero;
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = sizeDelta;
            return rectTransform;
        }

        private static RectTransform[] CreateElements(Transform parent, int count)
        {
            if (count == 0)
            {
                return Array.Empty<RectTransform>();
            }

            return new[] { CreateElement(parent, "TestCard (0)", Vector2.zero, new Vector2(50f, 50f)) };
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

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotOverlapping_NoPairOverlaps_Success()
        {
            var canvas = CreateCanvas();
            var actual = new[]
            {
                CreateElement(canvas.transform, "TestCard (0)", new Vector2(0f, 0f), new Vector2(50f, 50f)),
                CreateElement(canvas.transform, "TestCard (1)", new Vector2(60f, 0f), new Vector2(50f, 50f)),
                CreateElement(canvas.transform, "TestCard (2)", new Vector2(120f, 0f), new Vector2(50f, 50f)),
            };

            Assert.That(actual, Is.Not.Overlapping);
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotOverlapping_AcceptedActualElementTypes_Success([Values] ActualKind kind)
        {
            var canvas = CreateCanvas();
            var element0 = CreateElement(canvas.transform, "TestCard (0)", new Vector2(0f, 0f), new Vector2(50f, 50f));
            var element1 = CreateElement(canvas.transform, "TestCard (1)", new Vector2(60f, 0f), new Vector2(50f, 50f));
            var actual = new[] { AsActual(element0, kind), AsActual(element1, kind) };

            Assert.That(actual, Is.Not.Overlapping);
        }

        [Test]
        [CreateScene]
        public void IsOverlappingAndEmpty_TwoElementsOverlap_Failure()
        {
            // Left (Overlapping) passes and right (Empty) fails, so both sides are actually evaluated:
            // wiring And to OrOperator by mistake would make this pass instead.
            var canvas = CreateCanvas();
            var element0 = CreateElement(canvas.transform, "TestCard (0)", new Vector2(120f, 40f),
                new Vector2(100f, 100f));
            var element1 = CreateElement(canvas.transform, "TestCard (1)", new Vector2(200f, 40f),
                new Vector2(100f, 100f));
            var actual = new[] { element0, element1 };

            Assert.That(() => { Assert.That(actual, Is.Overlapping.And.Empty); },
                Throws.TypeOf<AssertionException>());
        }

        [Test]
        [CreateScene]
        public void IsOverlappingOrNotEmpty_NoPairOverlaps_Success()
        {
            // Left (Overlapping) fails and right (Not.Empty) passes, so both sides are actually evaluated:
            // wiring Or to AndOperator by mistake would make this fail instead.
            var canvas = CreateCanvas();
            var element0 = CreateElement(canvas.transform, "TestCard (0)", new Vector2(0f, 0f), new Vector2(50f, 50f));
            var element1 = CreateElement(canvas.transform, "TestCard (1)", new Vector2(60f, 0f), new Vector2(50f, 50f));
            var actual = new[] { element0, element1 };

            Assert.That(actual, Is.Overlapping.Or.Not.Empty);
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotOverlapping_TwoElementsOverlap_Failure()
        {
            var canvas = CreateCanvas();
            var element0 = CreateElement(canvas.transform, "TestCard (3)", new Vector2(120f, 40f),
                new Vector2(100f, 100f));
            var element1 = CreateElement(canvas.transform, "TestCard (4)", new Vector2(200f, 40f),
                new Vector2(100f, 100f));
            var actual = new[] { element0, element1 };

            Assert.That(() =>
            {
                Assert.That(actual, Is.Not.Overlapping);
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: not any pair of RectTransforms overlapping{Environment.NewLine}" +
                $"  But was:  <\"TestCard (3)\" {ConstraintMessageFormatter.Format(new Rect(120f, 40f, 100f, 100f))} overlaps \"TestCard (4)\" {ConstraintMessageFormatter.Format(new Rect(200f, 40f, 100f, 100f))}>{Environment.NewLine}"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotOverlapping_ThreePairsOverlap_Failure()
        {
            var canvas = CreateCanvas();
            var element0 = CreateElement(canvas.transform, "TestCard (0)", new Vector2(0f, 0f),
                new Vector2(100f, 100f));
            var element1 = CreateElement(canvas.transform, "TestCard (1)", new Vector2(20f, 20f),
                new Vector2(100f, 100f));
            var element2 = CreateElement(canvas.transform, "TestCard (2)", new Vector2(40f, 40f),
                new Vector2(100f, 100f));
            var actual = new[] { element0, element1, element2 };

            Assert.That(() =>
            {
                Assert.That(actual, Is.Not.Overlapping);
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: not any pair of RectTransforms overlapping{Environment.NewLine}" +
                $"  But was:  <\"TestCard (0)\" {ConstraintMessageFormatter.Format(new Rect(0f, 0f, 100f, 100f))} overlaps \"TestCard (1)\" {ConstraintMessageFormatter.Format(new Rect(20f, 20f, 100f, 100f))} (and 2 more overlapping pairs)>{Environment.NewLine}"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsOverlapping_AnyPairOverlaps_Success()
        {
            var canvas = CreateCanvas();
            var actual = new[]
            {
                CreateElement(canvas.transform, "TestCard (0)", new Vector2(0f, 0f), new Vector2(100f, 100f)),
                CreateElement(canvas.transform, "TestCard (1)", new Vector2(20f, 20f), new Vector2(100f, 100f)),
            };

            Assert.That(actual, Is.Overlapping);
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsOverlapping_NoPairOverlaps_Failure()
        {
            var canvas = CreateCanvas();
            var actual = new[]
            {
                CreateElement(canvas.transform, "TestCard (0)", new Vector2(0f, 0f), new Vector2(50f, 50f)),
                CreateElement(canvas.transform, "TestCard (1)", new Vector2(60f, 0f), new Vector2(50f, 50f)),
            };

            Assert.That(() =>
            {
                Assert.That(actual, Is.Overlapping);
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: any pair of RectTransforms overlapping{Environment.NewLine}" +
                $"  But was:  no overlapping pair among {actual.Length} RectTransforms{Environment.NewLine}"));
        }

        [TestCase(0.0f)]
        [TestCase(0.5f)]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotOverlapping_OverlapExtentWithinDefaultTolerance_Success(float overlapExtent)
        {
            var canvas = CreateCanvas();
            var actual = new[]
            {
                CreateElement(canvas.transform, "TestCard (0)", new Vector2(0f, 0f), new Vector2(100f, 100f)),
                CreateElement(canvas.transform, "TestCard (1)", new Vector2(100f - overlapExtent, 0f),
                    new Vector2(100f, 100f)),
            };

            Assert.That(actual, Is.Not.Overlapping);
        }

        [Test]
        [CreateScene]
        public void IsNotOverlapping_OverlapExtentExceedsDefaultTolerance_Failure()
        {
            const float OverlapExtent = 5f;
            var canvas = CreateCanvas();
            var element0 = CreateElement(canvas.transform, "TestCard (0)", new Vector2(0f, 0f),
                new Vector2(100f, 100f));
            var element1 = CreateElement(canvas.transform, "TestCard (1)", new Vector2(100f - OverlapExtent, 0f),
                new Vector2(100f, 100f));
            var actual = new[] { element0, element1 };

            Assert.That(() =>
            {
                Assert.That(actual, Is.Not.Overlapping);
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: not any pair of RectTransforms overlapping{Environment.NewLine}" +
                $"  But was:  <\"TestCard (0)\" {ConstraintMessageFormatter.Format(new Rect(0f, 0f, 100f, 100f))} overlaps \"TestCard (1)\" {ConstraintMessageFormatter.Format(new Rect(100f - OverlapExtent, 0f, 100f, 100f))}>{Environment.NewLine}"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotOverlapping_OverlapExtentWithinSpecifiedTolerance_Success()
        {
            const float OverlapExtent = 1.5f;
            var canvas = CreateCanvas();
            var actual = new[]
            {
                CreateElement(canvas.transform, "TestCard (0)", new Vector2(0f, 0f), new Vector2(100f, 100f)),
                CreateElement(canvas.transform, "TestCard (1)", new Vector2(100f - OverlapExtent, 0f),
                    new Vector2(100f, 100f)),
            };

            Assert.That(actual, Is.Not.Overlapping.Within(2f));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotOverlapping_OverlapOnSingleAxisOnly_Success()
        {
            var canvas = CreateCanvas();
            var actual = new[]
            {
                CreateElement(canvas.transform, "TestCard (0)", new Vector2(0f, 0f), new Vector2(100f, 50f)),
                CreateElement(canvas.transform, "TestCard (1)", new Vector2(50f, 100f), new Vector2(100f, 50f)),
            };

            Assert.That(actual, Is.Not.Overlapping);
        }

        [TestCase(0)]
        [TestCase(1)]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotOverlapping_FewerThanTwoMembers_ThrowsArgumentException(int memberCount)
        {
            var canvas = CreateCanvas();
            var actual = CreateElements(canvas.transform, memberCount);

            Assert.That(() =>
            {
                Assert.That(actual, Is.Not.Overlapping);
            }, Throws.TypeOf<ArgumentException>()
                .With.Property("ParamName").EqualTo("actual")
                .And.Message.Contains($"collection has {memberCount} element")
                .And.Message.Contains("at least 2"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotOverlapping_IgnoringGroupContainingBothOverlappingMembers_Success()
        {
            var canvas = CreateCanvas();
            var element0 = CreateElement(canvas.transform, "TestCard (0)", new Vector2(0f, 0f),
                new Vector2(100f, 100f));
            var element1 = CreateElement(canvas.transform, "TestCard (1)", new Vector2(20f, 20f),
                new Vector2(100f, 100f));
            var actual = new[] { element0, element1 };
            var ignoredGroup = new[] { element0, element1 };

            Assert.That(actual, Is.Not.Overlapping.Ignoring(ignoredGroup));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotOverlapping_IgnoringGroupContainingOneOverlappingMember_Failure()
        {
            var canvas = CreateCanvas();
            var element0 = CreateElement(canvas.transform, "TestCard (0)", new Vector2(0f, 0f),
                new Vector2(100f, 100f));
            var element1 = CreateElement(canvas.transform, "TestCard (1)", new Vector2(20f, 20f),
                new Vector2(100f, 100f));
            var actual = new[] { element0, element1 };
            var ignoredGroup = new[] { element0 };

            Assert.That(() =>
            {
                Assert.That(actual, Is.Not.Overlapping.Ignoring(ignoredGroup));
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: not any pair of RectTransforms overlapping{Environment.NewLine}" +
                $"  But was:  <\"TestCard (0)\" {ConstraintMessageFormatter.Format(new Rect(0f, 0f, 100f, 100f))} overlaps \"TestCard (1)\" {ConstraintMessageFormatter.Format(new Rect(20f, 20f, 100f, 100f))}>{Environment.NewLine}"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotOverlapping_MultipleIgnoredGroups_Success()
        {
            var canvas = CreateCanvas();
            var element0 = CreateElement(canvas.transform, "TestCard (0)", new Vector2(0f, 0f),
                new Vector2(100f, 100f));
            var element1 = CreateElement(canvas.transform, "TestCard (1)", new Vector2(20f, 20f),
                new Vector2(100f, 100f));
            var element2 = CreateElement(canvas.transform, "TestCard (2)", new Vector2(300f, 0f),
                new Vector2(100f, 100f));
            var element3 = CreateElement(canvas.transform, "TestCard (3)", new Vector2(320f, 20f),
                new Vector2(100f, 100f));
            var actual = new[] { element0, element1, element2, element3 };
            var groupA = new[] { element0, element1 };
            var groupB = new[] { element2, element3 };

            Assert.That(actual, Is.Not.Overlapping.Ignoring(groupA).Ignoring(groupB));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotOverlapping_ChainedIgnoringAndWithinModifiers_Success()
        {
            var canvas = CreateCanvas();
            var element0 = CreateElement(canvas.transform, "TestCard (0)", new Vector2(0f, 0f),
                new Vector2(100f, 100f));
            var element1 = CreateElement(canvas.transform, "TestCard (1)", new Vector2(20f, 20f),
                new Vector2(100f, 100f));
            var element2 = CreateElement(canvas.transform, "TestCard (2)", new Vector2(300f, 0f),
                new Vector2(100f, 100f));
            var element3 = CreateElement(canvas.transform, "TestCard (3)", new Vector2(398.5f, 0f),
                new Vector2(100f, 100f));
            var actual = new[] { element0, element1, element2, element3 };
            var ignoredGroup = new[] { element0, element1 };

            Assert.That(actual, Is.Not.Overlapping.Ignoring(ignoredGroup).Within(2f));
        }

        [Test]
        [Category("Acceptance")]
        public void Ignoring_Null_ThrowsArgumentNullException()
        {
            Assert.That(() =>
            {
                Is.Overlapping.Ignoring(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsOverlapping_IgnoredGroupContainsNull_ThrowsArgumentNullException()
        {
            var canvas = CreateCanvas();
            var element0 = CreateElement(canvas.transform, "TestCard (0)", new Vector2(0f, 0f),
                new Vector2(50f, 50f));
            var element1 = CreateElement(canvas.transform, "TestCard (1)", new Vector2(60f, 0f),
                new Vector2(50f, 50f));
            var actual = new[] { element0, element1 };
            var ignoredGroup = new[] { element0, null };

            Assert.That(() =>
            {
                Assert.That(actual, Is.Overlapping.Ignoring(ignoredGroup));
            }, Throws.TypeOf<ArgumentNullException>().With.Property("ParamName")
                .EqualTo("ignored group member at index 1"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsOverlapping_SingleElementActual_ThrowsArgumentException([Values] ActualKind kind)
        {
            var canvas = CreateCanvas();
            var rectTransform = CreateElement(canvas.transform, "Element", Vector2.zero, new Vector2(50f, 50f));
            var actual = AsActual(rectTransform, kind);

            Assert.That(() =>
            {
                Assert.That(actual, Is.Overlapping);
            }, Throws.TypeOf<ArgumentException>()
                .With.Property("ParamName").EqualTo("actual")
                .And.Message.Contains("is a single RectTransform, not a collection"));
        }

        [TestCase(0)]
        [TestCase(1)]
        [CreateScene]
        [Category("Acceptance")]
        public void IsOverlapping_FewerThanTwoMembers_ThrowsArgumentException(int memberCount)
        {
            var canvas = CreateCanvas();
            var actual = CreateElements(canvas.transform, memberCount);

            Assert.That(() =>
            {
                Assert.That(actual, Is.Overlapping);
            }, Throws.TypeOf<ArgumentException>()
                .With.Property("ParamName").EqualTo("actual")
                .And.Message.Contains($"collection has {memberCount} element")
                .And.Message.Contains("at least 2"));
        }

        [Test]
        [Category("Acceptance")]
        public void IsOverlapping_NonCollectionActual_ThrowsArgumentException()
        {
            Assert.That(() =>
            {
                // Not a swapped actual/expected: this constant IS the actual value under test, deliberately a
                // non-collection, to exercise the "not a collection of RectTransforms" failure path.
#pragma warning disable NUnit2007
                Assert.That("not a collection", Is.Overlapping);
#pragma warning restore NUnit2007
            }, Throws.TypeOf<ArgumentException>()
                .With.Property("ParamName").EqualTo("actual")
                .And.Message.Contains("is not a collection of RectTransforms"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsOverlapping_CollectionContainsElementWithoutRectTransform_ThrowsArgumentException()
        {
            var canvas = CreateCanvas();
            var element0 = CreateElement(canvas.transform, "TestCard (0)", Vector2.zero, new Vector2(50f, 50f));
            var plainObject = new GameObject("PlainObject");
            var actual = new object[] { element0, plainObject };

            Assert.That(() =>
            {
                Assert.That(actual, Is.Overlapping);
            }, Throws.TypeOf<ArgumentException>()
                .With.Property("ParamName").EqualTo("element at index 1")
                .And.Message.Contains("has no RectTransform component"));
        }

        [Test]
        [CreateScene]
        public void IsOverlapping_CollectionContainsNullElement_ThrowsArgumentNullException()
        {
            var canvas = CreateCanvas();
            var element0 = CreateElement(canvas.transform, "TestCard (0)", Vector2.zero, new Vector2(50f, 50f));
            var actual = new[] { element0, null };

            Assert.That(() =>
            {
                Assert.That(actual, Is.Overlapping);
            }, Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("element at index 1"));
        }

        [Test]
        [Category("Acceptance")]
        public void IsOverlapping_Null_ThrowsArgumentNullException()
        {
            Assert.That(() =>
            {
                Assert.That(null, Is.Overlapping);
            }, Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("actual"));
        }

        [Test]
        [Category("Acceptance")]
        public void IsNotOverlapping_Null_ThrowsArgumentNullException()
        {
            Assert.That(() =>
            {
                Assert.That(null, Is.Not.Overlapping);
            }, Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("actual"));
        }
    }
}
