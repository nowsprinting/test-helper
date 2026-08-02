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
        private bool _isHandling;

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
            if (type != LogType.Warning || _isHandling)
            {
                // The reentrancy guard matters because Debug.LogException's own dispatched log text
                // embeds the original exception's Message verbatim (here, the fallback-warning text
                // that triggered it), so without it a Debug.LogException call made in response to our
                // own reported finding could be mistaken for a new fallback warning and re-reported.
                return;
            }

            if (FallbackWarningLogPattern.TryMatchFallbackWarning(condition, out _))
            {
                _isHandling = true;
                try
                {
                    _reporter.Report(condition);
                }
                finally
                {
                    _isHandling = false;
                }
            }
        }
    }
}
