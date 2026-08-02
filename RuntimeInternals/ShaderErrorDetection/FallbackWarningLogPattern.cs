// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Text.RegularExpressions;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Matches shader fallback warning messages, e.g.,
    /// "Shader 'X' is not supported on this GPU (fallback to Y)".
    /// The pattern is tolerant of wording variations across Unity versions.
    /// </summary>
    internal static class FallbackWarningLogPattern
    {
        // Only the leading "Shader '<name>'" and the presence of "fallback" later in the message are
        // fixed; everything in between is left free-form because Unity's exact wording has changed
        // across versions (e.g., "(fallback to 'X')" vs "(using 'X' as a fallback)").
        private static readonly Regex Pattern =
            new Regex(@"^Shader '(?<name>[^']+)'.*fallback", RegexOptions.IgnoreCase);

        /// <summary>
        /// Returns true if the log message is a shader fallback warning.
        /// </summary>
        /// <param name="logMessage">Log message to match.</param>
        /// <param name="shaderName">Shader name contained in the message; null if not matched.</param>
        internal static bool TryMatchFallbackWarning(string logMessage, out string shaderName)
        {
            shaderName = null;
            if (string.IsNullOrEmpty(logMessage))
            {
                return false;
            }

            var match = Pattern.Match(logMessage);
            if (!match.Success)
            {
                return false;
            }

            shaderName = match.Groups["name"].Value;
            return true;
        }
    }
}
