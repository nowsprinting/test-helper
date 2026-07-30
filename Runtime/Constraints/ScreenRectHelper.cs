// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEngine;

namespace TestHelper.Constraints
{
    /// <summary>
    /// Computes the axis-aligned screen-space rect of a <see cref="RectTransform"/>, supporting all three
    /// <see cref="Canvas"/> render modes.
    /// </summary>
    /// <remarks>
    /// The rect is the axis-aligned bounding box of the four world corners, so a rotated element is
    /// over-approximated. Scale is handled correctly. Masking/clipping (<c>RectMask2D</c>),
    /// <c>Canvas.enabled</c>, <c>CanvasGroup.alpha</c>, and <c>activeInHierarchy</c> are ignored — this is
    /// geometry only.
    /// </remarks>
    internal static class ScreenRectHelper
    {
        internal static Rect GetScreenRect(RectTransform rectTransform)
        {
            var camera = GetScreenSpaceCamera(rectTransform);

            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            var min = new Vector2(float.MaxValue, float.MaxValue);
            var max = new Vector2(float.MinValue, float.MinValue);
            for (var i = 0; i < corners.Length; i++)
            {
                var screenPoint = RectTransformUtility.WorldToScreenPoint(camera, corners[i]);
                min = Vector2.Min(min, screenPoint);
                max = Vector2.Max(max, screenPoint);
            }

            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        internal static Camera GetScreenSpaceCamera(RectTransform rectTransform)
        {
            var canvas = FindCanvasInParent(rectTransform);
            if (canvas == null)
            {
                return null;
            }

            var rootCanvas = canvas.rootCanvas;

            // A non-null camera under Overlay would yield wrong values; RectTransformUtility.WorldToScreenPoint
            // treats a null camera as "world position == screen position", which is exactly the Overlay
            // convention (and also the convention used when there is no Canvas ancestor at all).
            return rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera;
        }

        private static Canvas FindCanvasInParent(RectTransform rectTransform)
        {
            // A manual walk instead of GetComponentInParent<Canvas>(), because the includeInactive overload
            // does not exist in Unity 2019.4 and the no-arg form is not reliable for inactive ancestors — a
            // common test scenario (a Canvas temporarily disabled to check its visibility state).
            var current = rectTransform.transform;
            while (current != null)
            {
                var canvas = current.GetComponent<Canvas>();
                if (canvas != null)
                {
                    return canvas;
                }

                current = current.parent;
            }

            return null;
        }
    }
}
