// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using NUnit.Framework;
using TestHelper.Attributes;
#if ENABLE_TMP
using TMPro;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace TestHelper.Constraints
{
    [SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
    public class TextOverflowingConstraintTest
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

        private static Text CreateUguiText(Transform parent, string name, string text, Vector2 size,
            HorizontalWrapMode horizontalOverflow = HorizontalWrapMode.Overflow,
            VerticalWrapMode verticalOverflow = VerticalWrapMode.Overflow,
            bool resizeTextForBestFit = false)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            var rectTransform = (RectTransform)gameObject.transform;
            rectTransform.SetParent(parent, worldPositionStays: false);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.pivot = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = size;

            var uguiText = gameObject.GetComponent<Text>();
            uguiText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            uguiText.text = text;
            uguiText.fontSize = 20;
            uguiText.horizontalOverflow = horizontalOverflow;
            uguiText.verticalOverflow = verticalOverflow;
            uguiText.resizeTextForBestFit = resizeTextForBestFit;
            uguiText.resizeTextMinSize = 10;
            uguiText.resizeTextMaxSize = 40;
            return uguiText;
        }

#if ENABLE_TMP
        private static TMP_Text CreateTmpText(Transform parent, string name, string text, Vector2 size,
            TextWrappingModes wrappingMode = TextWrappingModes.Normal, bool enableAutoSizing = false,
            TextOverflowModes overflowMode = TextOverflowModes.Overflow)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            var rectTransform = (RectTransform)gameObject.transform;
            rectTransform.SetParent(parent, worldPositionStays: false);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.pivot = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = size;

            var tmpText = gameObject.GetComponent<TextMeshProUGUI>();
            tmpText.font = TMP_Settings.defaultFontAsset;
            tmpText.text = text;
            tmpText.fontSize = 20;
            tmpText.textWrappingMode = wrappingMode;
            tmpText.enableAutoSizing = enableAutoSizing;
            tmpText.overflowMode = overflowMode;
            return tmpText;
        }
#endif

        private static object AsActual(RectTransform rectTransform, ActualKind kind)
        {
            switch (kind)
            {
                case ActualKind.RectTransform:
                    return rectTransform;
                case ActualKind.GameObject:
                    return rectTransform.gameObject;
                case ActualKind.Component:
                    return rectTransform.GetComponent<Text>();
                default:
                    return rectTransform;
            }
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_UguiTextFitsRect_Success()
        {
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label", "Hi", new Vector2(300f, 100f));
            Assume.That(uguiText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing());
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_AcceptedActualTypes_Success([Values] ActualKind kind)
        {
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label", "Hi", new Vector2(300f, 100f));
            Assume.That(uguiText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            var actual = AsActual(uguiText.GetComponent<RectTransform>(), kind);

            Assert.That(actual, Is.Not.TextOverflowing());
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsTextOverflowing_UguiTextFitsRect_Failure()
        {
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label", "Hi", new Vector2(300f, 100f));
            Assume.That(uguiText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(() =>
            {
                Assert.That(uguiText.GetComponent<RectTransform>(), Is.TextOverflowing);
            }, Throws.TypeOf<AssertionException>()
                .With.Message.Contains("Expected: text overflowing its RectTransform")
                .And.Message.Contains("\"Label\"")
                .And.Message.Contains("within rect"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_UguiPreferredSizeExceedsRect_Failure()
        {
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label", "Overflowing text content",
                new Vector2(5f, 5f));
            Assume.That(uguiText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(() =>
            {
                Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing());
            }, Throws.TypeOf<AssertionException>()
                .With.Message.Contains("Expected: not text overflowing its RectTransform")
                .And.Message.Contains("\"Label\"")
                .And.Message.Contains("exceeds rect"));
        }

        [TestCase(0.0f)]
        [TestCase(0.5f)]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_UguiExcessWithinDefaultTolerance_Success(float excess)
        {
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label", "Measure", new Vector2(1000f, 200f));
            Assume.That(uguiText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            var preferredWidth = uguiText.preferredWidth;
            var rectTransform = uguiText.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(preferredWidth - excess, 200f);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(rectTransform, Is.Not.TextOverflowing());
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_UguiExcessWithinSpecifiedTolerance_Success()
        {
            const float Excess = 1.5f;
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label", "Measure", new Vector2(1000f, 200f));
            Assume.That(uguiText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            var preferredWidth = uguiText.preferredWidth;
            var rectTransform = uguiText.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(preferredWidth - Excess, 200f);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(rectTransform, Is.Not.TextOverflowing().Within(2f));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_UguiBestFitEnabledAndPreferredSizeExceedsRect_Success()
        {
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label", "Very long overflowing label text",
                new Vector2(20f, 20f), resizeTextForBestFit: true);
            Assume.That(uguiText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing());
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsTextOverflowing_UguiBestFitEnabled_Failure()
        {
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label", "Very long overflowing label text",
                new Vector2(20f, 20f), resizeTextForBestFit: true);
            Assume.That(uguiText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(() =>
            {
                Assert.That(uguiText.GetComponent<RectTransform>(), Is.TextOverflowing);
            }, Throws.TypeOf<AssertionException>()
                .With.Message.Contains("\"Label\"")
                .And.Message.Contains("best fit"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_UguiWrapAndUnwrappedPreferredWidthExceedsRectWidth_Success()
        {
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label", "This is a long sentence that will wrap",
                new Vector2(100f, 300f), HorizontalWrapMode.Wrap);
            Assume.That(uguiText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing());
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsTextOverflowing_UguiWrapAndTextFitsRect_Failure()
        {
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label", "This is a long sentence that will wrap",
                new Vector2(100f, 300f), HorizontalWrapMode.Wrap);
            Assume.That(uguiText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(() =>
            {
                Assert.That(uguiText.GetComponent<RectTransform>(), Is.TextOverflowing);
            }, Throws.TypeOf<AssertionException>()
                .With.Message.Contains("\"Label\"")
                .And.Message.Contains("width")
                .And.Message.Contains("skipped"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_UguiWrapAndPreferredHeightExceedsRectHeight_Failure()
        {
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label", "This is a long sentence that will wrap",
                new Vector2(100f, 10f), HorizontalWrapMode.Wrap);
            Assume.That(uguiText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(() =>
            {
                Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing());
            }, Throws.TypeOf<AssertionException>()
                .With.Message.Contains("\"Label\"")
                .And.Message.Contains("exceeds rect"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_UguiHorizontalOverflowAndPreferredWidthExceedsRectWidth_Failure()
        {
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label", "Overflowing text content",
                new Vector2(5f, 200f));
            Assume.That(uguiText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(() =>
            {
                Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing());
            }, Throws.TypeOf<AssertionException>()
                .With.Message.Contains("\"Label\"")
                .And.Message.Contains("exceeds rect"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_UguiTruncateAndCharactersTruncated_Failure()
        {
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label",
                "This text will not fully fit and gets truncated",
                new Vector2(150f, 20f), HorizontalWrapMode.Wrap, VerticalWrapMode.Truncate);
            Assume.That(uguiText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(() =>
            {
                Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing());
            }, Throws.TypeOf<AssertionException>()
                .With.Message.Contains("\"Label\"")
                .And.Message.Contains("are rendered"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_UguiTruncateAndLineAfterEmbeddedNewlineNotRendered_Failure()
        {
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label", "Overflowing sentence\nSecond line",
                new Vector2(130f, 55f), HorizontalWrapMode.Wrap, VerticalWrapMode.Truncate);
            Assume.That(uguiText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(() =>
            {
                Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing());
            }, Throws.TypeOf<AssertionException>()
                .With.Message.Contains("\"Label\"")
                .And.Message.Contains("are rendered"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_UguiVerticalOverflowAndTextSpillsBeyondRect_Failure()
        {
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label", "Single line text",
                new Vector2(300f, 5f));
            Assume.That(uguiText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(() =>
            {
                Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing());
            }, Throws.TypeOf<AssertionException>()
                .With.Message.Contains("\"Label\"")
                .And.Message.Contains("exceeds rect")
                .And.Message.Not.Contains("are rendered"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotTextOverflowing_UguiTextNotLaidOut_Failure()
        {
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label", "Some text", new Vector2(200f, 50f));
            Assume.That(uguiText.font, Is.Not.Null);
            // Note: intentionally skip Canvas.ForceUpdateCanvases()/frame wait so the TextGenerator never populates

            Assert.That(() =>
            {
                Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing());
            }, Throws.TypeOf<AssertionException>()
                .With.Message.Contains("\"Label\"")
                .And.Message.Contains("has not been laid out; call Canvas.ForceUpdateCanvases() before asserting")
                .And.Message.Not.Contains("are rendered"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_UguiEmptyText_Success()
        {
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label", string.Empty, new Vector2(5f, 5f));
            Assume.That(uguiText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing());
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_ScaledTextFitsLocalRect_Success()
        {
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label", "Hi", new Vector2(300f, 100f));
            Assume.That(uguiText.font, Is.Not.Null);
            uguiText.GetComponent<RectTransform>().localScale = new Vector3(3f, 3f, 1f);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing());
        }

#if ENABLE_TMP
        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_TmpTextFitsRect_Success()
        {
            var canvas = CreateCanvas();
            var tmpText = CreateTmpText(canvas.transform, "Label", "Hi", new Vector2(300f, 100f));
            Assume.That(tmpText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(tmpText.GetComponent<RectTransform>(), Is.Not.TextOverflowing());
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_TmpPreferredHeightExceedsRectHeight_Failure()
        {
            var canvas = CreateCanvas();
            var tmpText = CreateTmpText(canvas.transform, "Label",
                "This is a long sentence that will wrap over lines", new Vector2(100f, 10f));
            Assume.That(tmpText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(() =>
            {
                Assert.That(tmpText.GetComponent<RectTransform>(), Is.Not.TextOverflowing());
            }, Throws.TypeOf<AssertionException>()
                .With.Message.Contains("\"Label\"")
                .And.Message.Contains("exceeds rect")
                .And.Message.Not.Contains("are not rendered"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_TmpAutoSizingEnabledAndPreferredHeightExceedsRectHeight_Success()
        {
            var canvas = CreateCanvas();
            var tmpText = CreateTmpText(canvas.transform, "Label",
                "This is a long sentence that will wrap over lines", new Vector2(100f, 10f),
                enableAutoSizing: true);
            Assume.That(tmpText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(tmpText.GetComponent<RectTransform>(), Is.Not.TextOverflowing());
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_TmpPreferredWidthExceedsRectWidth_Success()
        {
            var canvas = CreateCanvas();
            var tmpText = CreateTmpText(canvas.transform, "Label", "A very very very long single line of text",
                new Vector2(5f, 100f), TextWrappingModes.NoWrap);
            Assume.That(tmpText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(tmpText.GetComponent<RectTransform>(), Is.Not.TextOverflowing());
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_TmpCharactersNotRendered_Failure()
        {
            var canvas = CreateCanvas();
            var tmpText = CreateTmpText(canvas.transform, "Label",
                "This text will not fully fit and gets truncated", new Vector2(150f, 20f),
                TextWrappingModes.Normal, false, TextOverflowModes.Truncate);
            Assume.That(tmpText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(() =>
            {
                Assert.That(tmpText.GetComponent<RectTransform>(), Is.Not.TextOverflowing());
            }, Throws.TypeOf<AssertionException>()
                .With.Message.Contains("\"Label\"")
                .And.Message.Contains("are not rendered (overflowMode: Truncate)"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_TmpEmptyText_Success()
        {
            var canvas = CreateCanvas();
            var tmpText = CreateTmpText(canvas.transform, "Label", string.Empty, new Vector2(5f, 5f));
            Assume.That(tmpText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(tmpText.GetComponent<RectTransform>(), Is.Not.TextOverflowing());
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotTextOverflowing_TmpTextNotLaidOut_Success()
        {
            var canvas = CreateCanvas();
            var tmpText = CreateTmpText(canvas.transform, "Label", "Some text", new Vector2(200f, 50f));
            Assume.That(tmpText.font, Is.Not.Null);
            // Note: intentionally skip Canvas.ForceUpdateCanvases()/frame wait, unlike the uGUI equivalent
            // test: TMP_Text.preferredWidth/preferredHeight compute the layout on demand rather than
            // depending on a prior OnPopulateMesh pass, so there is no "not laid out" state to detect here.

            Assert.That(tmpText.GetComponent<RectTransform>(), Is.Not.TextOverflowing());
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_BothUguiTextAndTmpTextPresentAndOnlyTmpOverflows_Success()
        {
            // Note: a uGUI Text and a TMP_Text cannot coexist on the same GameObject (both derive from
            // Graphic, and a GameObject allows only one Graphic component), so the TMP element that would
            // overflow is placed on a sibling GameObject instead. This still exercises the same precedence
            // guarantee (GetComponent looks at the target's own GameObject only, so a nearby, unrelated,
            // overflowing TMP element must not affect the uGUI Text's own fitting result).
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label", "Hi", new Vector2(300f, 100f));
            Assume.That(uguiText.font, Is.Not.Null);
            var tmpText = CreateTmpText(canvas.transform, "OtherLabel",
                "A very very very long single line of overflowing text content that exceeds the rect",
                new Vector2(5f, 5f), TextWrappingModes.NoWrap);
            Assume.That(tmpText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing());
        }
#endif

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsTextOverflowing_NoTextComponent_ThrowsArgumentException()
        {
            var canvas = CreateCanvas();
            var gameObject = new GameObject("PlainObject", typeof(RectTransform));
            gameObject.transform.SetParent(canvas.transform, worldPositionStays: false);

            Assert.That(() =>
            {
                Assert.That(gameObject.GetComponent<RectTransform>(), Is.TextOverflowing);
            }, Throws.TypeOf<ArgumentException>()
                .With.Property("ParamName").EqualTo("actual")
                .And.Message.Contains("has no Text or TMP_Text component"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotTextOverflowing_NoTextComponent_ThrowsArgumentException()
        {
            var canvas = CreateCanvas();
            var gameObject = new GameObject("PlainObject", typeof(RectTransform));
            gameObject.transform.SetParent(canvas.transform, worldPositionStays: false);

            Assert.That(() =>
            {
                Assert.That(gameObject.GetComponent<RectTransform>(), Is.Not.TextOverflowing());
            }, Throws.TypeOf<ArgumentException>()
                .With.Property("ParamName").EqualTo("actual")
                .And.Message.Contains("has no Text or TMP_Text component"));
        }

        [Test]
        public void IsTextOverflowing_Null_ThrowsArgumentNullException()
        {
            Assert.That(() =>
            {
                Assert.That(null, Is.TextOverflowing);
            }, Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("actual"));
        }

        [Test]
        public void IsNotTextOverflowing_Null_ThrowsArgumentNullException()
        {
            Assert.That(() =>
            {
                Assert.That(null, Is.Not.TextOverflowing());
            }, Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("actual"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsTextOverflowing_DestroyedGameObject_ThrowsArgumentException()
        {
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label", "Hi", new Vector2(300f, 100f));
            var rectTransform = uguiText.GetComponent<RectTransform>();
            GameObject.DestroyImmediate(uguiText.gameObject);

            Assert.That(() =>
            {
                Assert.That(rectTransform, Is.TextOverflowing);
            }, Throws.TypeOf<ArgumentException>()
                .With.Property("ParamName").EqualTo("actual")
                .And.Message.Contains("destroyed UnityEngine.Object"));
        }

        [Test]
        [Category("Acceptance")]
        public void IsTextOverflowing_UnsupportedActualType_ThrowsArgumentException()
        {
            Assert.That(() =>
            {
                // Not a swapped actual/expected: this constant IS the actual value under test, deliberately an
                // unsupported type, to exercise the "not a RectTransform, GameObject, or Component" failure path.
                Assert.That("not a RectTransform", Is.TextOverflowing);
            }, Throws.TypeOf<ArgumentException>()
                .With.Property("ParamName").EqualTo("actual")
                .And.Message.Contains("is not a RectTransform, GameObject, or Component"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsTextOverflowing_GameObjectWithoutRectTransform_ThrowsArgumentException()
        {
            var gameObject = new GameObject("PlainObject");

            Assert.That(() =>
            {
                Assert.That(gameObject, Is.TextOverflowing);
            }, Throws.TypeOf<ArgumentException>()
                .With.Property("ParamName").EqualTo("actual")
                .And.Message.Contains("has no RectTransform component"));
        }
    }
}
