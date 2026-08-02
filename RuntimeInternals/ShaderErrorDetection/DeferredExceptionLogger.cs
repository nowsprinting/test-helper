// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Hidden, scene-persistent relay that logs queued exceptions on the next frame, outside the
    /// call stack that raised them. Needed because Unity's log dispatch does not deliver any log
    /// raised while a listener callback is already executing (verified empirically: even a direct
    /// <c>Debug.LogException</c> call made from inside an <c>Application.logMessageReceived</c>
    /// listener never reaches other listeners, nor UTF's own log tracking) — deferring by one frame
    /// moves the <c>Debug.LogException</c> call outside that nested dispatch, where it logs normally.
    /// </summary>
    internal sealed class DeferredExceptionLogger : MonoBehaviour
    {
        private readonly Queue<Exception> _pending = new Queue<Exception>();

        /// <summary>
        /// Creates a hidden, <c>DontDestroyOnLoad</c> <see cref="DeferredExceptionLogger"/> and starts its drain loop.
        /// </summary>
        internal static DeferredExceptionLogger Create()
        {
            var logger = HiddenGameObjectFactory.CreateHidden<DeferredExceptionLogger>(nameof(DeferredExceptionLogger));
            logger.StartCoroutine(logger.DrainLoop());
            return logger;
        }

        /// <summary>
        /// Queues an exception to be logged via <c>Debug.LogException</c> on the next frame.
        /// </summary>
        internal void Enqueue(Exception exception)
        {
            _pending.Enqueue(exception);
        }

        /// <summary>
        /// Stops the drain loop and destroys this instance.
        /// </summary>
        internal void StopAndDestroy()
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }

        // ReSharper disable once IteratorNeverReturns -- runs for this instance's lifetime; stopped
        // externally by StopAndDestroy (StopAllCoroutines), not by the iterator returning.
        private IEnumerator DrainLoop()
        {
            while (true)
            {
                yield return null;
                while (_pending.Count > 0)
                {
                    Debug.LogException(_pending.Dequeue());
                }
            }
        }
    }
}
