// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;
using TestHelper.RuntimeInternals.ShaderErrorDetection.TestDoubles;
using UnityEngine;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    [TestFixture]
    public class FallbackWarningLogMonitorTest
    {
        private const string FallbackWarningMessage =
            "Shader 'MyShader' is not supported on this GPU (fallback to 'Diffuse')";

        [Test]
        [Category("Acceptance")]
        public void Start_ShaderFallbackWarningLogged_ThrowsInvalidShaderException()
        {
            var logSource = new FakeLogMessageSource();
            var monitor = new FallbackWarningLogMonitor(logSource, new ThrowingShaderErrorReporter());
            monitor.Start();

            var exception = Assert.Throws<InvalidShaderException>(() =>
                logSource.Raise(FallbackWarningMessage, string.Empty, LogType.Warning));

            Assert.That(exception.Message, Does.Contain("MyShader"));
        }

        [Test]
        public void Start_UnrelatedWarningLogged_DoesNotReport()
        {
            var logSource = new FakeLogMessageSource();
            var spyReporter = new SpyShaderErrorReporter();
            var monitor = new FallbackWarningLogMonitor(logSource, spyReporter);
            monitor.Start();

            logSource.Raise("An unrelated warning message", string.Empty, LogType.Warning);

            Assert.That(spyReporter.ReportedMessages, Is.Empty);
        }

        [TestCase(LogType.Error, TestName = "{m}(Error)")]
        [TestCase(LogType.Log, TestName = "{m}(Log)")]
        [TestCase(LogType.Assert, TestName = "{m}(Assert)")]
        [TestCase(LogType.Exception, TestName = "{m}(Exception)")]
        [Category("Acceptance")]
        public void Start_ShaderFallbackTextLoggedWithNonWarningLogType_DoesNotReport(LogType logType)
        {
            var logSource = new FakeLogMessageSource();
            var spyReporter = new SpyShaderErrorReporter();
            var monitor = new FallbackWarningLogMonitor(logSource, spyReporter);
            monitor.Start();

            logSource.Raise(FallbackWarningMessage, string.Empty, logType);

            Assert.That(spyReporter.ReportedMessages, Is.Empty);
        }

        [Test]
        public void Stop_AfterStart_ShaderFallbackWarningIsNotReported()
        {
            var logSource = new FakeLogMessageSource();
            var spyReporter = new SpyShaderErrorReporter();
            var monitor = new FallbackWarningLogMonitor(logSource, spyReporter);
            monitor.Start();
            monitor.Stop();

            logSource.Raise(FallbackWarningMessage, string.Empty, LogType.Warning);

            Assert.That(spyReporter.ReportedMessages, Is.Empty);
        }
    }
}
