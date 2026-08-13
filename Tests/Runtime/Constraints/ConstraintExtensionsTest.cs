// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace TestHelper.Constraints
{
    // Regression coverage for ConstraintExtensions: adding property style (Is.Not.<X>) must not break the
    // pre-existing method style (Is.Not.<X>()). Compiling and passing is the assertion here; per-constraint
    // failure-message coverage already lives in each constraint's own test class.
    [SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
    public class ConstraintExtensionsTest
    {
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

        [Test]
        [CreateScene]
        public void Destroyed_MethodStyleWithOperator_Available()
        {
            var actual = new GameObject("Foo");

            Assert.That(actual, Is.Not.Destroyed());
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void WithinScreen_MethodStyleWithOperator_Available()
        {
            var canvas = CreateCanvas();
            const float Width = 50f;
            const float Overshoot = 20f;
            var actual = CreateElement(canvas.transform, "Element",
                new Vector2(Screen.width - Width + Overshoot, 10f), new Vector2(Width, 50f));

            Assert.That(actual, Is.Not.WithinScreen());
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void WithinContainer_MethodStyleWithOperator_Available()
        {
            var canvas = CreateCanvas();
            var container = CreateElement(canvas.transform, "Viewport", Vector2.zero, new Vector2(200f, 150f));
            var actual = CreateElement(canvas.transform, "Element", new Vector2(300f, 300f), new Vector2(50f, 50f));

            Assert.That(actual, Is.Not.WithinContainer(container));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void Overlapping_MethodStyleWithOperator_Available()
        {
            var canvas = CreateCanvas();
            var actual = new[]
            {
                CreateElement(canvas.transform, "TestCard (0)", new Vector2(0f, 0f), new Vector2(50f, 50f)),
                CreateElement(canvas.transform, "TestCard (1)", new Vector2(60f, 0f), new Vector2(50f, 50f)),
            };

            Assert.That(actual, Is.Not.Overlapping());
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task TextOverflowing_MethodStyleWithOperator_Available()
        {
            var canvas = CreateCanvas();
            var element = CreateElement(canvas.transform, "Label", Vector2.zero, new Vector2(300f, 100f));
            var text = element.gameObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>(
#if UNITY_2022_2_OR_NEWER
                "LegacyRuntime.ttf"
#else
                "Arial.ttf" // Arial.ttf was replaced by LegacyRuntime.ttf in Unity 2022.2
#endif
            );
            text.text = "Hi";
            text.fontSize = 20;
            Assume.That(text.font, Is.Not.Null);
            Canvas.ForceUpdateCanvases();
            await UniTask.NextFrame();

            Assert.That(element, Is.Not.TextOverflowing());
        }
    }
}
