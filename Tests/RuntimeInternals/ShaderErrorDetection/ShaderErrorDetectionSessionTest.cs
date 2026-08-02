// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;
using TestHelper.RuntimeInternals.ShaderErrorDetection.TestDoubles;
using UnityEngine;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    [TestFixture]
    public class ShaderErrorDetectionSessionTest
    {
        private const string FallbackWarningMessage =
            "Shader 'MyShader' is not supported on this GPU (fallback to 'Diffuse')";

        private static ShaderErrorDetectionSession CreateSession(
            IShaderErrorReporter reporter, FallbackWarningLogMonitor monitor, params IMaterialScanner[] scanners)
        {
            return new ShaderErrorDetectionSession(0, reporter, monitor, scanners);
        }

        [Test]
        public void Start_Called_IsRunning()
        {
            var reporter = new SpyShaderErrorReporter();
            var monitor = new FallbackWarningLogMonitor(new FakeLogMessageSource(), reporter);
            var session = CreateSession(reporter, monitor);

            session.Start();

            Assert.That(session.IsRunning, Is.True);
        }

        [Test]
        [Category("Acceptance")]
        public void Start_Called_DoesNotScan()
        {
            var reporter = new SpyShaderErrorReporter();
            var monitor = new FallbackWarningLogMonitor(new FakeLogMessageSource(), reporter);
            var spyScanner = new SpyMaterialScanner();
            var session = CreateSession(reporter, monitor, spyScanner);

            session.Start();

            Assert.That(spyScanner.ScanCallCount, Is.EqualTo(0));
        }

        [Test]
        public void Start_ShaderFallbackWarningLogged_ThrowsInvalidShaderException()
        {
            var reporter = new ThrowingShaderErrorReporter();
            var logSource = new FakeLogMessageSource();
            var monitor = new FallbackWarningLogMonitor(logSource, reporter);
            var session = CreateSession(reporter, monitor);
            session.Start();

            var exception = Assert.Throws<InvalidShaderException>(() =>
                logSource.Raise(FallbackWarningMessage, string.Empty, LogType.Warning));

            Assert.That(exception.Message, Does.Contain("MyShader"));
        }

        [Test]
        public void Stop_AfterStart_IsNotRunning()
        {
            var reporter = new SpyShaderErrorReporter();
            var monitor = new FallbackWarningLogMonitor(new FakeLogMessageSource(), reporter);
            var session = CreateSession(reporter, monitor, new SpyMaterialScanner());
            session.Start();

            session.Stop();

            Assert.That(session.IsRunning, Is.False);
        }

        [Test]
        [Category("Acceptance")]
        public void Stop_AfterStart_ThrowsInvalidShaderExceptionOnFinalScan()
        {
            var reporter = new ThrowingShaderErrorReporter();
            var monitor = new FallbackWarningLogMonitor(new FakeLogMessageSource(), reporter);
            var spyScanner = new SpyMaterialScanner { NextFindings = { "GameObject 'Cube' : Material 'Broken' has an error shader" } };
            var session = CreateSession(reporter, monitor, spyScanner);
            session.Start();

            var exception = Assert.Throws<InvalidShaderException>(() => session.Stop());

            Assert.That(exception.Message, Does.Contain("Cube"));
        }

        [Test]
        [Category("Acceptance")]
        public void Stop_FinalScanThrows_IsNotRunning()
        {
            var reporter = new ThrowingShaderErrorReporter();
            var monitor = new FallbackWarningLogMonitor(new FakeLogMessageSource(), reporter);
            var spyScanner = new SpyMaterialScanner { NextFindings = { "GameObject 'Cube' : Material 'Broken' has an error shader" } };
            var session = CreateSession(reporter, monitor, spyScanner);
            session.Start();

            Assert.Throws<InvalidShaderException>(() => session.Stop());

            Assert.That(session.IsRunning, Is.False);
        }

        [Test]
        public void Stop_AfterStart_ShaderFallbackWarningIsNotReported()
        {
            var reporter = new SpyShaderErrorReporter();
            var logSource = new FakeLogMessageSource();
            var monitor = new FallbackWarningLogMonitor(logSource, reporter);
            var session = CreateSession(reporter, monitor, new SpyMaterialScanner());
            session.Start();
            session.Stop();

            logSource.Raise(FallbackWarningMessage, string.Empty, LogType.Warning);

            Assert.That(reporter.ReportedMessages, Is.Empty);
        }

        [Test]
        [Category("Acceptance")]
        public void Stop_CalledTwice_ScansOnce()
        {
            var reporter = new SpyShaderErrorReporter();
            var monitor = new FallbackWarningLogMonitor(new FakeLogMessageSource(), reporter);
            var spyScanner = new SpyMaterialScanner();
            var session = CreateSession(reporter, monitor, spyScanner);
            session.Start();

            session.Stop();
            session.Stop();

            Assert.That(spyScanner.ScanCallCount, Is.EqualTo(1));
        }

        [Test]
        public void Stop_CalledTwice_IsNotRunning()
        {
            var reporter = new SpyShaderErrorReporter();
            var monitor = new FallbackWarningLogMonitor(new FakeLogMessageSource(), reporter);
            var session = CreateSession(reporter, monitor, new SpyMaterialScanner());
            session.Start();

            session.Stop();
            session.Stop();

            Assert.That(session.IsRunning, Is.False);
        }

        [Test]
        [Category("Acceptance")]
        public void Stop_CalledWithoutStart_DoesNotScan()
        {
            var reporter = new SpyShaderErrorReporter();
            var monitor = new FallbackWarningLogMonitor(new FakeLogMessageSource(), reporter);
            var spyScanner = new SpyMaterialScanner();
            var session = CreateSession(reporter, monitor, spyScanner);

            session.Stop();

            Assert.That(spyScanner.ScanCallCount, Is.EqualTo(0));
        }

        [Test]
        public void ScanOnce_ScannersReturnMessage_ThrowsInvalidShaderException()
        {
            var reporter = new ThrowingShaderErrorReporter();
            var monitor = new FallbackWarningLogMonitor(new FakeLogMessageSource(), reporter);
            var spyScanner = new SpyMaterialScanner { NextFindings = { "GameObject 'Cube' : Material 'Broken' has an error shader" } };
            var session = CreateSession(reporter, monitor, spyScanner);

            var exception = Assert.Throws<InvalidShaderException>(() => session.ScanOnce());

            Assert.That(exception.Message, Is.EqualTo("GameObject 'Cube' : Material 'Broken' has an error shader"));
        }

        [Test]
        public void ScanOnce_ScannersReturnNoMessages_DoesNotReport()
        {
            var reporter = new SpyShaderErrorReporter();
            var monitor = new FallbackWarningLogMonitor(new FakeLogMessageSource(), reporter);
            var session = CreateSession(reporter, monitor, new SpyMaterialScanner());

            session.ScanOnce();

            Assert.That(reporter.ReportedMessages, Is.Empty);
        }

        [Test]
        [Category("Acceptance")]
        public void ScanOnce_CalledAgainAfterThrowing_ThrowsForRemainingMessage()
        {
            var reporter = new ThrowingShaderErrorReporter();
            var monitor = new FallbackWarningLogMonitor(new FakeLogMessageSource(), reporter);
            var spyScanner = new SpyMaterialScanner { NextFindings = { "Message 1", "Message 2" } };
            var session = CreateSession(reporter, monitor, spyScanner);
            Assert.Throws<InvalidShaderException>(() => session.ScanOnce());

            // Simulate that "Message 1"'s underlying issue was already checked (as a real cache-backed
            // scanner would do), leaving only the remaining finding for the next scan.
            spyScanner.NextFindings.Remove("Message 1");

            var exception = Assert.Throws<InvalidShaderException>(() => session.ScanOnce());

            Assert.That(exception.Message, Is.EqualTo("Message 2"));
        }
    }
}
