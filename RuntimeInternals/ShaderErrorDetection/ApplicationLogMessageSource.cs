// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using UnityEngine;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Forwards <c>Application.logMessageReceived</c> to <see cref="MessageReceived"/>.
    /// </summary>
    internal sealed class ApplicationLogMessageSource : ILogMessageSource
    {
        private Application.LogCallback _messageReceived;
        private DeferredExceptionLogger _deferredLogger;

        /// <inheritdoc/>
        public event Application.LogCallback MessageReceived
        {
            add
            {
                // Subscribe to Application.logMessageReceived only while at least one listener is registered,
                // so this source has no effect when nothing is observing it.
                if (_messageReceived == null)
                {
                    Application.logMessageReceived += OnLogMessageReceived;
                }

                _messageReceived += value;
            }
            remove
            {
                _messageReceived -= value;
                if (_messageReceived == null)
                {
                    Application.logMessageReceived -= OnLogMessageReceived;
                    if (_deferredLogger != null)
                    {
                        _deferredLogger.StopAndDestroy();
                        _deferredLogger = null;
                    }
                }
            }
        }

        private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            try
            {
                _messageReceived?.Invoke(condition, stackTrace, type);
            }
            catch (Exception e)
            {
                // Unity's log dispatch delivers nothing raised from within an already-executing
                // listener callback (verified empirically), so Debug.LogException here would be
                // silently dropped; defer it to the next frame via DeferredExceptionLogger, outside
                // this nested dispatch, where it logs normally. In Edit Mode no frame ever advances,
                // so there is no "next frame" to defer to; log immediately there as a best effort.
                if (Application.isPlaying)
                {
                    if (_deferredLogger == null)
                    {
                        _deferredLogger = DeferredExceptionLogger.Create();
                    }

                    _deferredLogger.Enqueue(e);
                }
                else
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}
