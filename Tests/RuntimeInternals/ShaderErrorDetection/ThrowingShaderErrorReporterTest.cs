// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    [TestFixture]
    public class ThrowingShaderErrorReporterTest
    {
        [Test]
        public void Report_Called_ThrowsInvalidShaderException()
        {
            var reporter = new ThrowingShaderErrorReporter();

            var exception = Assert.Throws<InvalidShaderException>(() => reporter.Report("Some shader error message"));

            Assert.That(exception.Message, Is.EqualTo("Some shader error message"));
        }

        [Test]
        [Category("Acceptance")]
        public void Report_CalledTwiceWithSameMessage_ThrowsOnBothCalls()
        {
            var reporter = new ThrowingShaderErrorReporter();

            Assert.Throws<InvalidShaderException>(() => reporter.Report("Repeated message"));
            Assert.Throws<InvalidShaderException>(() => reporter.Report("Repeated message"));
        }
    }
}
