// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEngine;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection.TestDoubles
{
    /// <summary>
    /// Fake <see cref="ILogMessageSource"/> that raises <see cref="MessageReceived"/> synchronously
    /// on demand, instead of forwarding <c>Application.logMessageReceived</c>. This lets tests observe
    /// exceptions thrown by a subscribed handler directly (no Unity log-dispatch boundary involved).
    /// </summary>
    internal sealed class FakeLogMessageSource : ILogMessageSource
    {
        /// <inheritdoc/>
        public event Application.LogCallback MessageReceived;

        /// <summary>
        /// Raises <see cref="MessageReceived"/> with the given log message.
        /// </summary>
        internal void Raise(string condition, string stackTrace, LogType type)
        {
            MessageReceived?.Invoke(condition, stackTrace, type);
        }
    }
}
