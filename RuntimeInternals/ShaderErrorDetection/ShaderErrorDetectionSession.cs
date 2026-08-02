// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Orchestrates one shader-error detection session: owns the log monitor, the periodic
    /// hierarchy scan driver, the scanners, and the reporter. One instance corresponds to one
    /// test's detection lifetime (created fresh in <c>BeforeTest</c>).
    /// </summary>
    internal sealed class ShaderErrorDetectionSession
    {
        private readonly int _scanIntervalFrames;
        private readonly IShaderErrorReporter _reporter;
        private readonly FallbackWarningLogMonitor _monitor;
        private readonly IReadOnlyList<IMaterialScanner> _scanners;

        private MaterialScanRunner _runner;

        internal ShaderErrorDetectionSession(
            int scanIntervalFrames,
            IShaderErrorReporter reporter,
            FallbackWarningLogMonitor monitor,
            IReadOnlyList<IMaterialScanner> scanners)
        {
            // Negative values are treated as "every frame" here, in one place, so callers
            // (the attribute, MaterialScanRunner) never need their own clamping logic.
            _scanIntervalFrames = Mathf.Max(0, scanIntervalFrames);
            _reporter = reporter;
            _monitor = monitor;
            _scanners = scanners;
        }

        /// <summary>
        /// Creates a session wired with production defaults: <see cref="ThrowingShaderErrorReporter"/>,
        /// <see cref="ApplicationLogMessageSource"/>, and Renderer/Graphic/Skybox scanners sharing
        /// one <see cref="CheckedMaterialCache"/>.
        /// </summary>
        /// <param name="scanIntervalFrames">Frames between hierarchy scan ticks. Values &lt;= 0 mean every frame.</param>
        internal static ShaderErrorDetectionSession CreateDefault(int scanIntervalFrames)
        {
            var reporter = new ThrowingShaderErrorReporter();
            var monitor = new FallbackWarningLogMonitor(new ApplicationLogMessageSource(), reporter);
            var cache = new CheckedMaterialCache();
            var scanners = new List<IMaterialScanner>
            {
                new RendererMaterialScanner(cache),
                new GraphicMaterialScanner(cache),
                new SkyboxMaterialScanner(cache),
            };
            return new ShaderErrorDetectionSession(scanIntervalFrames, reporter, monitor, scanners);
        }

        internal bool IsRunning { get; private set; }

        /// <summary>
        /// Starts the session: starts the log monitor, and (only when <c>Application.isPlaying</c>)
        /// spawns the periodic scan driver. Idempotent: calling <see cref="Start"/> again while
        /// already running has no effect. Does not perform an immediate scan.
        /// </summary>
        internal void Start()
        {
            if (IsRunning)
            {
                return;
            }

            IsRunning = true;
            _monitor.Start();

            // Coroutines do not tick in Edit Mode test runs; the log monitor is the only detection
            // method available there.
            if (Application.isPlaying)
            {
                _runner = MaterialScanRunner.Create(ScanOnce, _scanIntervalFrames);
            }
        }

        /// <summary>
        /// Stops the session: tears down the log monitor and scan driver first, then performs one
        /// final <see cref="ScanOnce"/>. Idempotent: only the first call after a <see cref="Start"/>
        /// performs teardown and the final scan; a call without a preceding <see cref="Start"/> does nothing.
        /// </summary>
        internal void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            // Tear down before the final scan (which may throw) so a thrown exception never leaves
            // the monitor subscribed or the runner alive to cascade into later tests.
            IsRunning = false;
            _monitor.Stop();
            if (_runner != null)
            {
                _runner.StopAndDestroy();
                _runner = null;
            }

            ScanOnce();
        }

        /// <summary>
        /// Runs all scanners once and reports every returned message. Callable regardless of session state.
        /// </summary>
        internal void ScanOnce()
        {
            foreach (var scanner in _scanners)
            {
                foreach (var message in scanner.Scan())
                {
                    _reporter.Report(message);
                }
            }
        }
    }
}
