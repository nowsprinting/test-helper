// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    [TestFixture]
    public class FallbackWarningLogPatternTest
    {
        [TestCase("Shader 'MyShader' is not supported on this GPU (fallback to 'Diffuse')", "MyShader",
            TestName = "{m}(WordingA)")]
        [TestCase("Shader 'Custom/Foo' was not loaded (using 'Legacy Shaders/Diffuse' as a fallback)", "Custom/Foo",
            TestName = "{m}(WordingB)")]
        public void TryMatchFallbackWarning_ShaderFallbackWarning_ReturnsTrueWithShaderName(string message,
            string expectedShaderName)
        {
            var result = FallbackWarningLogPattern.TryMatchFallbackWarning(message, out var shaderName);

            Assert.That(result, Is.True);
            Assert.That(shaderName, Is.EqualTo(expectedShaderName));
        }

        [TestCase("", TestName = "{m}(Empty)")]
        [TestCase("An ordinary log message", TestName = "{m}(OrdinaryText)")]
        [TestCase("Shader 'MyShader' compiled successfully", TestName = "{m}(UnrelatedShaderMessage)")]
        public void TryMatchFallbackWarning_UnrelatedMessage_ReturnsFalse(string message)
        {
            var result = FallbackWarningLogPattern.TryMatchFallbackWarning(message, out _);

            Assert.That(result, Is.False);
        }
    }
}
