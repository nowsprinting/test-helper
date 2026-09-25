// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Receives detected shader error messages.
    /// </summary>
    internal interface IShaderErrorReporter
    {
        /// <summary>
        /// Reports a detected shader error.
        /// </summary>
        /// <param name="message">Human-readable description of the detected shader error.</param>
        void Report(string message);
    }
}
