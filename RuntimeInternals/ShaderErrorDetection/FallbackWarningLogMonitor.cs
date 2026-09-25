// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEngine;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Log-monitoring detection method: watches log messages and reports shader fallback warnings.
    /// </summary>
    internal sealed class FallbackWarningLogMonitor
    {
        private readonly ILogMessageSource _logMessageSource;
        private readonly IShaderErrorReporter _reporter;

        private bool _isRunning;

        internal FallbackWarningLogMonitor(ILogMessageSource logMessageSource, IShaderErrorReporter reporter)
        {
            _logMessageSource = logMessageSource;
            _reporter = reporter;
        }

        /// <summary>
        /// Starts monitoring log messages.
        /// </summary>
        internal void Start()
        {
            if (_isRunning)
            {
                return;
            }

            _isRunning = true;
            _logMessageSource.MessageReceived += HandleLogMessage;
        }

        /// <summary>
        /// Stops monitoring log messages.
        /// </summary>
        internal void Stop()
        {
            if (!_isRunning)
            {
                return;
            }

            _isRunning = false;
            _logMessageSource.MessageReceived -= HandleLogMessage;
        }

        private void HandleLogMessage(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Warning)
            {
                return;
            }

            if (FallbackWarningLogPattern.TryMatchFallbackWarning(condition, out _))
            {
                _reporter.Report(condition);
            }
        }
    }
}
