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
        /// Initializes a new instance without a message. Only present for the standard exception
        /// constructor set; ThrowingShaderErrorReporter always uses the message constructor below.
        /// </summary>
        internal InvalidShaderException()
        {
        }

        /// <summary>
        /// Initializes a new instance with a message that identifies the offending object
        /// (GameObject path, material, and/or shader name).
        /// </summary>
        /// <param name="message">Human-readable description of the detected shader error.</param>
        internal InvalidShaderException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance with a message and an inner exception. Only present for the
        /// standard exception constructor set; nothing in this feature throws with an inner exception.
        /// </summary>
        internal InvalidShaderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
