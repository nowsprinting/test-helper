// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine;

namespace TestHelper.Constraints
{
    public class ScreenRectHelperTest
    {
        private static RectTransform CreateElement(Transform parent, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            var gameObject = new GameObject("Element", typeof(RectTransform));
            var rectTransform = (RectTransform)gameObject.transform;
            rectTransform.SetParent(parent, worldPositionStays: false);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.pivot = Vector2.zero;
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = sizeDelta;
            return rectTransform;
        }

        private static Canvas CreateOverlayCanvas()
        {
            var gameObject = new GameObject("Canvas", typeof(Canvas));
            var canvas = gameObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            return canvas;
        }

        // Real Camera projection (used by Screen Space - Camera / World Space) round-trips world<->screen
        // through a projection matrix, which introduces sub-pixel floating-point noise (~1e-5) even for
        // "round number" inputs — an inherent property of the math, not a defect. Screen Space - Overlay
        // and the no-Canvas-ancestor path bypass the camera entirely and stay exact (see the other tests
        // in this fixture using Is.EqualTo without tolerance).
        private static void AssertApproximatelyEqual(Rect actual, Rect expected)
        {
            const float Tolerance = 0.001f;
            Assert.That(actual.x, Is.EqualTo(expected.x).Within(Tolerance), "x");
            Assert.That(actual.y, Is.EqualTo(expected.y).Within(Tolerance), "y");
            Assert.That(actual.width, Is.EqualTo(expected.width).Within(Tolerance), "width");
            Assert.That(actual.height, Is.EqualTo(expected.height).Within(Tolerance), "height");
        }

        private static Camera CreatePerspectiveCamera()
        {
            var gameObject = new GameObject("Camera", typeof(Camera));
            return gameObject.GetComponent<Camera>();
        }

        private static Camera CreateOrthographicCamera()
        {
            var camera = CreatePerspectiveCamera();
            camera.orthographic = true;
            camera.orthographicSize = Screen.height / 2f;
            camera.transform.position = new Vector3(0f, 0f, -10f);
            return camera;
        }

        private static Canvas CreateScreenSpaceCameraCanvas(Camera camera)
        {
            var gameObject = new GameObject("Canvas", typeof(Canvas));
            var canvas = gameObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 10f;
            return canvas;
        }

        private static Canvas CreateWorldSpaceCanvas(Camera camera)
        {
            var gameObject = new GameObject("Canvas", typeof(Canvas));
            var canvas = gameObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = camera;

            var rectTransform = (RectTransform)canvas.transform;
            rectTransform.pivot = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
            rectTransform.position = new Vector3(-Screen.width / 2f, -Screen.height / 2f, 0f);
            return canvas;
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task GetScreenRect_ScreenSpaceOverlayCanvas_ReturnsScreenSpaceRect()
        {
            var canvas = CreateOverlayCanvas();
            var element = CreateElement(canvas.transform, new Vector2(10f, 20f), new Vector2(100f, 50f));
            await UniTask.NextFrame();

            var actual = ScreenRectHelper.GetScreenRect(element);

            Assert.That(actual, Is.EqualTo(new Rect(10f, 20f, 100f, 50f)));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task GetScreenRect_ScreenSpaceCameraCanvas_ReturnsScreenSpaceRect()
        {
            var camera = CreatePerspectiveCamera();
            var canvas = CreateScreenSpaceCameraCanvas(camera);
            var element = CreateElement(canvas.transform, new Vector2(10f, 20f), new Vector2(100f, 50f));
            await UniTask.NextFrame();

            var actual = ScreenRectHelper.GetScreenRect(element);

            AssertApproximatelyEqual(actual, new Rect(10f, 20f, 100f, 50f));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task GetScreenRect_WorldSpaceCanvas_ReturnsScreenSpaceRect()
        {
            var camera = CreateOrthographicCamera();
            var canvas = CreateWorldSpaceCanvas(camera);
            var element = CreateElement(canvas.transform, new Vector2(10f, 20f), new Vector2(100f, 50f));
            await UniTask.NextFrame();

            var actual = ScreenRectHelper.GetScreenRect(element);

            AssertApproximatelyEqual(actual, new Rect(10f, 20f, 100f, 50f));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void GetScreenRect_NoCanvasAncestor_ReturnsRectFromWorldCoordinates()
        {
            var element = CreateElement(null, new Vector2(10f, 20f), new Vector2(100f, 50f));

            var actual = ScreenRectHelper.GetScreenRect(element);

            Assert.That(actual, Is.EqualTo(new Rect(10f, 20f, 100f, 50f)));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void GetScreenRect_InactiveCanvasAncestor_ReturnsScreenSpaceRect()
        {
            var canvas = CreateOverlayCanvas();
            var element = CreateElement(canvas.transform, new Vector2(10f, 20f), new Vector2(100f, 50f));
            canvas.gameObject.SetActive(false);

            var actual = ScreenRectHelper.GetScreenRect(element);

            Assert.That(actual, Is.EqualTo(new Rect(10f, 20f, 100f, 50f)));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public async Task GetScreenRect_NestedCanvasUnderScreenSpaceCameraRootCanvas_ReturnsScreenSpaceRect()
        {
            var camera = CreatePerspectiveCamera();
            var rootCanvas = CreateScreenSpaceCameraCanvas(camera);
            var nestedCanvasGameObject = new GameObject("NestedCanvas", typeof(Canvas));
            nestedCanvasGameObject.transform.SetParent(rootCanvas.transform, worldPositionStays: false);
            // A freshly added RectTransform defaults to a point anchor with a fixed 100x100 size, not a
            // stretch to fill its parent; stretch it explicitly so this nested canvas is a transparent
            // passthrough over its parent's full rect — otherwise it introduces its own (irrelevant to
            // this test) offset unrelated to the screen-space-camera conversion under test.
            var nestedRectTransform = (RectTransform)nestedCanvasGameObject.transform;
            nestedRectTransform.anchorMin = Vector2.zero;
            nestedRectTransform.anchorMax = Vector2.one;
            nestedRectTransform.offsetMin = Vector2.zero;
            nestedRectTransform.offsetMax = Vector2.zero;
            var element = CreateElement(nestedCanvasGameObject.transform, new Vector2(10f, 20f),
                new Vector2(100f, 50f));
            await UniTask.NextFrame();

            var actual = ScreenRectHelper.GetScreenRect(element);

            AssertApproximatelyEqual(actual, new Rect(10f, 20f, 100f, 50f));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void GetScreenRect_ScaledElement_ReturnsScaledRect()
        {
            var canvas = CreateOverlayCanvas();
            var element = CreateElement(canvas.transform, new Vector2(10f, 20f), new Vector2(50f, 50f));
            element.localScale = new Vector3(2f, 2f, 1f);

            var actual = ScreenRectHelper.GetScreenRect(element);

            Assert.That(actual, Is.EqualTo(new Rect(10f, 20f, 100f, 100f)));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void GetScreenRect_RotatedElement_ReturnsAxisAlignedBoundingBox()
        {
            var canvas = CreateOverlayCanvas();
            var gameObject = new GameObject("Element", typeof(RectTransform));
            var element = (RectTransform)gameObject.transform;
            element.SetParent(canvas.transform, worldPositionStays: false);
            element.anchorMin = new Vector2(0.5f, 0.5f);
            element.anchorMax = new Vector2(0.5f, 0.5f);
            element.pivot = new Vector2(0.5f, 0.5f);
            element.anchoredPosition = Vector2.zero;
            element.sizeDelta = new Vector2(100f, 100f);
            element.localRotation = Quaternion.Euler(0f, 0f, 45f);
            var expectedBoundingBoxSide = 100f * Mathf.Sqrt(2f); // 45-degree rotated square's AABB side

            var actual = ScreenRectHelper.GetScreenRect(element);

            Assert.That(actual.width, Is.EqualTo(expectedBoundingBoxSide).Within(0.01f), "width");
            Assert.That(actual.height, Is.EqualTo(expectedBoundingBoxSide).Within(0.01f), "height");
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void GetScreenSpaceCamera_ScreenSpaceOverlayCanvas_ReturnsNull()
        {
            var canvas = CreateOverlayCanvas();
            var element = CreateElement(canvas.transform, Vector2.zero, Vector2.zero);

            var actual = ScreenRectHelper.GetScreenSpaceCamera(element);

            Assert.That(actual, Is.Null);
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void GetScreenSpaceCamera_NoCanvasAncestor_ReturnsNull()
        {
            var element = CreateElement(null, Vector2.zero, Vector2.zero);

            var actual = ScreenRectHelper.GetScreenSpaceCamera(element);

            Assert.That(actual, Is.Null);
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void GetScreenSpaceCamera_ScreenSpaceCameraCanvas_ReturnsCanvasWorldCamera()
        {
            var camera = CreatePerspectiveCamera();
            var canvas = CreateScreenSpaceCameraCanvas(camera);
            var element = CreateElement(canvas.transform, Vector2.zero, Vector2.zero);

            var actual = ScreenRectHelper.GetScreenSpaceCamera(element);

            Assert.That(actual, Is.SameAs(camera));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void GetScreenSpaceCamera_WorldSpaceCanvas_ReturnsCanvasWorldCamera()
        {
            var camera = CreateOrthographicCamera();
            var canvas = CreateWorldSpaceCanvas(camera);
            var element = CreateElement(canvas.transform, Vector2.zero, Vector2.zero);

            var actual = ScreenRectHelper.GetScreenSpaceCamera(element);

            Assert.That(actual, Is.SameAs(camera));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void GetScreenSpaceCamera_NestedCanvas_ReturnsRootCanvasWorldCamera()
        {
            var camera = CreatePerspectiveCamera();
            var rootCanvas = CreateScreenSpaceCameraCanvas(camera);
            var nestedCanvasGameObject = new GameObject("NestedCanvas", typeof(Canvas));
            nestedCanvasGameObject.transform.SetParent(rootCanvas.transform, worldPositionStays: false);
            var element = CreateElement(nestedCanvasGameObject.transform, Vector2.zero, Vector2.zero);

            var actual = ScreenRectHelper.GetScreenSpaceCamera(element);

            Assert.That(actual, Is.SameAs(camera));
        }
    }
}
