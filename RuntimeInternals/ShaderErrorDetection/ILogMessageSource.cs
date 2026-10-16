// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEngine;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Source of log messages, abstracting <c>Application.logMessageReceived</c> for testability.
    /// </summary>
    internal interface ILogMessageSource
    {
        /// <summary>
        /// Raised for each log message with (condition, stackTrace, type).
        /// </summary>
        event Application.LogCallback MessageReceived;
    }
}
