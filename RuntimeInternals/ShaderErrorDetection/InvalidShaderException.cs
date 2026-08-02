// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Thrown when a shader error is detected during a test (e.g., fallback warning, error shader, or missing material).
    /// </summary>
    internal class InvalidShaderException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance with a message that identifies the offending object
        /// (GameObject path, material, and/or shader name).
        /// </summary>
        /// <param name="message">Human-readable description of the detected shader error.</param>
        internal InvalidShaderException(string message)
        {
        }
    }
}
