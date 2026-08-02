// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections;
using UnityEngine;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Hidden, scene-persistent driver that invokes a scan callback at a fixed frame interval.
    /// Uses <c>yield return null</c> (frame-based) rather than <c>WaitForEndOfFrame</c>, because
    /// <c>WaitForEndOfFrame</c> never fires when the Editor runs in batch mode.
    /// </summary>
    internal sealed class MaterialScanRunner : MonoBehaviour
    {
        private Action _onScanTick;
        private int _framesToWait;

        /// <summary>
        /// Creates a hidden, <c>DontDestroyOnLoad</c> <see cref="MaterialScanRunner"/> and starts its scan loop.
        /// </summary>
        /// <param name="onScanTick">Callback invoked every <paramref name="intervalFrames"/> frames.
        /// Exceptions are not caught; they propagate out of the coroutine.</param>
        /// <param name="intervalFrames">Frames between ticks. 0 means every frame.</param>
        internal static MaterialScanRunner Create(Action onScanTick, int intervalFrames)
        {
            var go = new GameObject(nameof(MaterialScanRunner)) { hideFlags = HideFlags.HideAndDontSave };
            DontDestroyOnLoad(go);

            var runner = go.AddComponent<MaterialScanRunner>();
            runner._onScanTick = onScanTick;
            // 0 still requires waiting at least one frame boundary before the first tick,
            // so "every frame" is the smallest possible wait (1), not an immediate tick.
            runner._framesToWait = Mathf.Max(1, intervalFrames);
            runner.StartCoroutine(runner.ScanLoop());
            return runner;
        }

        /// <summary>
        /// Stops the scan loop and destroys this instance.
        /// </summary>
        internal void StopAndDestroy()
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }

        private IEnumerator ScanLoop()
        {
            while (true)
            {
                for (var i = 0; i < _framesToWait; i++)
                {
                    yield return null;
                }

                _onScanTick?.Invoke();
            }
        }
    }
}
