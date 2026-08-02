// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEngine;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Forwards <c>Application.logMessageReceived</c> to <see cref="MessageReceived"/>.
    /// </summary>
    internal sealed class ApplicationLogMessageSource : ILogMessageSource
    {
        /// <inheritdoc/>
        public event Application.LogCallback MessageReceived;
    }
}
