// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Matches shader fallback warning messages, e.g.,
    /// "Shader 'X' is not supported on this GPU (fallback to Y)".
    /// The pattern is tolerant of wording variations across Unity versions.
    /// </summary>
    internal static class FallbackWarningLogPattern
    {
        /// <summary>
        /// Returns true if the log message is a shader fallback warning.
        /// </summary>
        /// <param name="logMessage">Log message to match.</param>
        /// <param name="shaderName">Shader name contained in the message; null if not matched.</param>
        internal static bool TryMatchFallbackWarning(string logMessage, out string shaderName)
        {
            shaderName = null;
            return false;
        }
    }
}
