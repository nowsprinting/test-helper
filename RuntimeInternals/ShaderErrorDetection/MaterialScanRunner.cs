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
        /// <summary>
        /// Creates a hidden, <c>DontDestroyOnLoad</c> <see cref="MaterialScanRunner"/> and starts its scan loop.
        /// </summary>
        /// <param name="onScanTick">Callback invoked every <paramref name="intervalFrames"/> frames.
        /// Exceptions are not caught; they propagate out of the coroutine.</param>
        /// <param name="intervalFrames">Frames between ticks. 0 means every frame.</param>
        internal static MaterialScanRunner Create(Action onScanTick, int intervalFrames)
        {
            return null;
        }

        /// <summary>
        /// Stops the scan loop and destroys this instance.
        /// </summary>
        internal void StopAndDestroy()
        {
        }

        private IEnumerator ScanLoop()
        {
            yield break;
        }
    }
}
