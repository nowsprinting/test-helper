// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections.Generic;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Orchestrates one shader-error detection session: owns the log monitor, the periodic
    /// hierarchy scan driver, the scanners, and the reporter. One instance corresponds to one
    /// test's detection lifetime (created fresh in <c>BeforeTest</c>).
    /// </summary>
    internal sealed class ShaderErrorDetectionSession
    {
        internal ShaderErrorDetectionSession(
            int scanIntervalFrames,
            IShaderErrorReporter reporter,
            FallbackWarningLogMonitor monitor,
            IReadOnlyList<IMaterialScanner> scanners)
        {
        }

        /// <summary>
        /// Creates a session wired with production defaults: <see cref="ThrowingShaderErrorReporter"/>,
        /// <see cref="ApplicationLogMessageSource"/>, and Renderer/Graphic/Skybox scanners sharing
        /// one <see cref="CheckedMaterialCache"/>.
        /// </summary>
        /// <param name="scanIntervalFrames">Frames between hierarchy scan ticks. Values &lt;= 0 mean every frame.</param>
        internal static ShaderErrorDetectionSession CreateDefault(int scanIntervalFrames)
        {
            return null;
        }

        internal bool IsRunning => false;

        /// <summary>
        /// Starts the session: starts the log monitor, and (only when <c>Application.isPlaying</c>)
        /// spawns the periodic scan driver. Idempotent: calling <see cref="Start"/> again while
        /// already running has no effect. Does not perform an immediate scan.
        /// </summary>
        internal void Start()
        {
        }

        /// <summary>
        /// Stops the session: tears down the log monitor and scan driver first, then performs one
        /// final <see cref="ScanOnce"/>. Idempotent: only the first call after a <see cref="Start"/>
        /// performs teardown and the final scan; a call without a preceding <see cref="Start"/> does nothing.
        /// </summary>
        internal void Stop()
        {
        }

        /// <summary>
        /// Runs all scanners once and reports every returned message. Callable regardless of session state.
        /// </summary>
        internal void ScanOnce()
        {
        }
    }
}
