// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using TestHelper.Attributes;
#if ENABLE_TMP || ENABLE_UGUI2
using TMPro;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace TestHelper.Constraints
{
    public class TextOverflowingConstraintTest
    {
        public enum ActualKind
        {
            RectTransform,
            GameObject,
            Component,
        }

#if UNITY_2022_2_OR_NEWER
        private const string BuiltinFontName = "LegacyRuntime.ttf";
#else
        private const string BuiltinFontName = "Arial.ttf";
        // Arial.ttf was replaced by LegacyRuntime.ttf in Unity 2022.2
#endif

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
            uguiText.font = Resources.GetBuiltinResource<Font>(BuiltinFontName);
            uguiText.text = text;
            uguiText.fontSize = 20;
            uguiText.horizontalOverflow = horizontalOverflow;
            uguiText.verticalOverflow = verticalOverflow;
            uguiText.resizeTextForBestFit = resizeTextForBestFit;
            uguiText.resizeTextMinSize = 10;
            uguiText.resizeTextMaxSize = 40;
            return uguiText;
        }

#if ENABLE_TMP || ENABLE_UGUI2
        private static TMP_FontAsset s_fallbackTmpFontAsset;

        private static TMP_FontAsset GetTmpFontAsset()
        {
            if (TMP_Settings.defaultFontAsset != null)
            {
                return TMP_Settings.defaultFontAsset;
            }

            // TMP_Settings.defaultFontAsset requires the "TMP Essential Resources" to be imported, which
            // freshly-generated CI projects never do — every TMP test would go Inconclusive there. Fall
            // back to a runtime-created dynamic font asset instead. Cached in a static because each
            // CreateFontAsset call builds a new dynamic atlas.
            if (s_fallbackTmpFontAsset == null)
            {
                s_fallbackTmpFontAsset =
                    TMP_FontAsset.CreateFontAsset(Resources.GetBuiltinResource<Font>(BuiltinFontName));
            }

            return s_fallbackTmpFontAsset;
        }

        private static TMP_Text CreateTmpText(Transform parent, string name, string text, Vector2 size,
            bool enableWordWrapping = true, bool enableAutoSizing = false,
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
            tmpText.font = GetTmpFontAsset();
            tmpText.text = text;
            tmpText.fontSize = 20;
#if ENABLE_UGUI2
            tmpText.textWrappingMode = enableWordWrapping ? TextWrappingModes.Normal : TextWrappingModes.NoWrap;
#else
            tmpText.enableWordWrapping = enableWordWrapping;
#endif
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
            await UniTask.NextFrame();

            Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
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
            await UniTask.NextFrame();
            var actual = AsActual(uguiText.GetComponent<RectTransform>(), kind);

            Assert.That(actual, Is.Not.TextOverflowing);
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
            await UniTask.NextFrame();

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
            await UniTask.NextFrame();

            Assert.That(() =>
            {
                Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
            }, Throws.TypeOf<AssertionException>()
                .With.Message.Contains("Expected: not text overflowing its RectTransform")
                .And.Message.Contains("\"Label\"")
                .And.Message.Contains("exceeds rect"));
        }

        [Test]
        [CreateScene]
        public async Task IsTextOverflowingAndNull_UguiPreferredSizeExceedsRect_Failure()
        {
            // Left (TextOverflowing) passes and right (Null) fails, so both sides are actually evaluated:
            // wiring And to OrOperator by mistake would make this pass instead.
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label", "Overflowing text content",
                new Vector2(5f, 5f));
            Assume.That(uguiText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await UniTask.NextFrame();

            Assert.That(() => { Assert.That(uguiText.GetComponent<RectTransform>(), Is.TextOverflowing.And.Null); },
                Throws.TypeOf<AssertionException>());
        }

        [Test]
        [CreateScene]
        public async Task IsTextOverflowingWithNull_UguiPreferredSizeExceedsRect_Failure()
        {
            // Left (TextOverflowing) passes and right (Null) fails, so both sides are actually evaluated:
            // wiring With to OrOperator by mistake would make this pass instead.
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label", "Overflowing text content",
                new Vector2(5f, 5f));
            Assume.That(uguiText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await UniTask.NextFrame();

            Assert.That(() => { Assert.That(uguiText.GetComponent<RectTransform>(), Is.TextOverflowing.With.Null); },
                Throws.TypeOf<AssertionException>());
        }

        [Test]
        [CreateScene]
        public async Task IsTextOverflowingOrNotNull_UguiTextFitsRect_Success()
        {
            // Left (TextOverflowing) fails and right (Not.Null) passes, so both sides are actually evaluated:
            // wiring Or to AndOperator by mistake would make this fail instead.
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label", "Hi", new Vector2(300f, 100f));
            Assume.That(uguiText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await UniTask.NextFrame();

            Assert.That(uguiText.GetComponent<RectTransform>(), Is.TextOverflowing.Or.Not.Null);
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
            await UniTask.NextFrame();
            var preferredWidth = uguiText.preferredWidth;
            var rectTransform = uguiText.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(preferredWidth - excess, 200f);
            Canvas.ForceUpdateCanvases();
            await UniTask.NextFrame();

            Assert.That(rectTransform, Is.Not.TextOverflowing);
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
            await UniTask.NextFrame();
            var preferredWidth = uguiText.preferredWidth;
            var rectTransform = uguiText.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(preferredWidth - Excess, 200f);
            Canvas.ForceUpdateCanvases();
            await UniTask.NextFrame();

            Assert.That(rectTransform, Is.Not.TextOverflowing.Within(2f));
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
            await UniTask.NextFrame();

            Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_UguiBestFitEnabledAndTruncateDropsCharacters_Failure()
        {
            var canvas = CreateCanvas();
            var uguiText = CreateUguiText(canvas.transform, "Label", "Very long overflowing label text",
                new Vector2(5f, 5f), HorizontalWrapMode.Wrap, VerticalWrapMode.Truncate,
                resizeTextForBestFit: true);
            Assume.That(uguiText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await UniTask.NextFrame();
            // Best fit cannot shrink below resizeTextMinSize, so a rect smaller than the minimum-size
            // glyphs drops characters under Truncate. Guard the fixture: if a font's metrics keep every
            // character visible, report Inconclusive instead of a false result.
            Assume.That(uguiText.cachedTextGenerator.characterCountVisible,
                Is.LessThan(uguiText.text.Length));

            Assert.That(() =>
            {
                Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
            }, Throws.TypeOf<AssertionException>()
                .With.Message.Contains("\"Label\"")
                .And.Message.Contains("are rendered"));
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
            await UniTask.NextFrame();

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
            await UniTask.NextFrame();

            Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
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
            await UniTask.NextFrame();

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
            await UniTask.NextFrame();

            Assert.That(() =>
            {
                Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
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
            await UniTask.NextFrame();

            Assert.That(() =>
            {
                Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
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
            await UniTask.NextFrame();

            Assert.That(() =>
            {
                Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
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
            await UniTask.NextFrame();

            Assert.That(() =>
            {
                Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
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
            await UniTask.NextFrame();

            Assert.That(() =>
            {
                Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
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
                Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
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
            await UniTask.NextFrame();

            Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
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
            await UniTask.NextFrame();

            Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
        }

#if ENABLE_TMP || ENABLE_UGUI2
        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_TmpTextFitsRect_Success()
        {
            var canvas = CreateCanvas();
            var tmpText = CreateTmpText(canvas.transform, "Label", "Hi", new Vector2(300f, 100f));
            Assume.That(tmpText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await UniTask.NextFrame();

            Assert.That(tmpText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
        }

        [Test]
        [CreateScene]
        public async Task IsNotTextOverflowing_TmpRenderedHeightExceedsRectHeight_Failure()
        {
            var canvas = CreateCanvas();
            var tmpText = CreateTmpText(canvas.transform, "Label",
                "This is a long sentence that will wrap over lines", new Vector2(100f, 10f));
            Assume.That(tmpText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await UniTask.NextFrame();

            Assert.That(() =>
            {
                Assert.That(tmpText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
            }, Throws.TypeOf<AssertionException>()
                .With.Message.Contains("\"Label\"")
                .And.Message.Contains("exceeds rect")
                .And.Message.Not.Contains("are not rendered"));
        }

        [Test]
        [CreateScene]
        public async Task IsNotTextOverflowing_TmpAutoSizingEnabledAndTextExceedsRect_Success()
        {
            var canvas = CreateCanvas();
            var tmpText = CreateTmpText(canvas.transform, "Label",
                "This is a long sentence that will wrap over lines", new Vector2(100f, 10f),
                enableAutoSizing: true);
            Assume.That(tmpText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await UniTask.NextFrame();

            Assert.That(tmpText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_TmpAutoSizingEnabledAndTruncateDropsCharacters_Failure()
        {
            var canvas = CreateCanvas();
            var tmpText = CreateTmpText(canvas.transform, "Label",
                "This text will not fully fit and gets truncated", new Vector2(150f, 20f),
                enableAutoSizing: true, overflowMode: TextOverflowModes.Truncate);
            // Auto sizing cannot shrink below fontSizeMin; pin the bounds so the fixture drops
            // characters regardless of the serialized TMP defaults.
            tmpText.fontSizeMin = 10f;
            tmpText.fontSizeMax = 40f;
            Assume.That(tmpText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await UniTask.NextFrame();
            // Guard the fixture: if a font's metrics let the text fit at the minimum size after all,
            // report Inconclusive instead of a false result.
            Assume.That(tmpText.firstOverflowCharacterIndex, Is.GreaterThanOrEqualTo(0));

            Assert.That(() =>
            {
                Assert.That(tmpText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
            }, Throws.TypeOf<AssertionException>()
                .With.Message.Contains("\"Label\"")
                .And.Message.Contains("are not rendered (overflowMode: Truncate)"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_TmpRenderedWidthExceedsRectWidth_Failure()
        {
            var canvas = CreateCanvas();
            var tmpText = CreateTmpText(canvas.transform, "Label", "A very very very long single line of text",
                new Vector2(5f, 100f), enableWordWrapping: false);
            Assume.That(tmpText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await UniTask.NextFrame();

            Assert.That(() =>
            {
                Assert.That(tmpText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
            }, Throws.TypeOf<AssertionException>()
                .With.Message.Contains("\"Label\"")
                .And.Message.Contains("exceeds rect"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_TmpWrappedTextWithinRect_Success()
        {
            var canvas = CreateCanvas();
            // Every word is narrower than the 150px line box, so wrapping keeps the rendered width
            // within the rect; the rect is tall enough for all wrapped lines.
            var tmpText = CreateTmpText(canvas.transform, "Label",
                "This is a long sentence that will wrap over lines", new Vector2(150f, 300f));
            Assume.That(tmpText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await UniTask.NextFrame();

            Assert.That(tmpText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_TmpRenderedWidthWithinRectButExceedsMargin_Failure()
        {
            var canvas = CreateCanvas();
            var tmpText = CreateTmpText(canvas.transform, "Label", "Measure me", new Vector2(1000f, 100f),
                enableWordWrapping: false);
            Assume.That(tmpText.font, Is.Not.Null);
            // Any rendered text is wider than the 20px left between the margins yet narrower than the
            // 1000px rect, so the rendered width fits the rect but exceeds the rect minus the margins —
            // no font-dependent measure-then-resize choreography needed.
            tmpText.margin = new Vector4(490f, 0f, 490f, 0f); // x=left, z=right
            Canvas.ForceUpdateCanvases();
            await UniTask.NextFrame();

            Assert.That(() =>
            {
                Assert.That(tmpText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
            }, Throws.TypeOf<AssertionException>()
                .With.Message.Contains("\"Label\"")
                .And.Message.Contains("exceeds rect"));
        }

        [TestCase(TextOverflowModes.Truncate)]
        [TestCase(TextOverflowModes.Ellipsis)]
        [CreateScene]
        [Category("Acceptance")]
        public async Task IsNotTextOverflowing_TmpTruncatingOverflowMode_Failure(TextOverflowModes overflowMode)
        {
            var canvas = CreateCanvas();
            var tmpText = CreateTmpText(canvas.transform, "Label",
                "This text will not fully fit and gets truncated", new Vector2(150f, 20f),
                overflowMode: overflowMode);
            Assume.That(tmpText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await UniTask.NextFrame();

            Assert.That(() =>
            {
                Assert.That(tmpText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
            }, Throws.TypeOf<AssertionException>()
                .With.Message.Contains("\"Label\"")
                .And.Message.Contains("characters from index")
                .And.Message.Contains($"are not rendered (overflowMode: {overflowMode})"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotTextOverflowing_TmpEmptyTextNotLaidOut_Success()
        {
            var canvas = CreateCanvas();
            var tmpText = CreateTmpText(canvas.transform, "Label", string.Empty, new Vector2(5f, 5f));
            Assume.That(tmpText.font, Is.Not.Null);
            // Note: intentionally skip Canvas.ForceUpdateCanvases()/frame wait — empty text has nothing to
            // overflow, so it must pass without being reported as "not laid out".

            Assert.That(tmpText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void IsNotTextOverflowing_TmpTextNotLaidOut_Failure()
        {
            var canvas = CreateCanvas();
            var tmpText = CreateTmpText(canvas.transform, "Label", "Some text", new Vector2(200f, 50f));
            Assume.That(tmpText.font, Is.Not.Null);
            // Note: intentionally skip Canvas.ForceUpdateCanvases()/frame wait so the text mesh is never
            // generated: rendered sizes are only valid after a layout pass, so an un-laid-out element must
            // be reported instead of silently passing.

            Assert.That(() =>
            {
                Assert.That(tmpText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
            }, Throws.TypeOf<AssertionException>()
                .With.Message.Contains("\"Label\"")
                .And.Message.Contains("has not been laid out; call Canvas.ForceUpdateCanvases() before asserting"));
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
                new Vector2(5f, 5f), enableWordWrapping: false);
            Assume.That(tmpText.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await UniTask.NextFrame();

            Assert.That(uguiText.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
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
                Assert.That(gameObject.GetComponent<RectTransform>(), Is.Not.TextOverflowing);
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
                Assert.That(null, Is.Not.TextOverflowing);
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
#pragma warning disable NUnit2007
                Assert.That("not a RectTransform", Is.TextOverflowing);
#pragma warning restore NUnit2007
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
