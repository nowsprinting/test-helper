// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Log-monitoring detection method: watches log messages and reports shader fallback warnings.
    /// Only <c>LogType.Warning</c> messages are processed, so exceptions thrown by the reporter
    /// (logged as <c>LogType.Exception</c>) can never re-enter this monitor.
    /// </summary>
    internal sealed class FallbackWarningLogMonitor
    {
        internal FallbackWarningLogMonitor(ILogMessageSource logMessageSource, IShaderErrorReporter reporter)
        {
        }

        /// <summary>
        /// Starts monitoring log messages.
        /// </summary>
        internal void Start()
        {
        }

        /// <summary>
        /// Stops monitoring log messages.
        /// </summary>
        internal void Stop()
        {
        }
    }
}
