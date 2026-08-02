// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Reporter that always throws <see cref="InvalidShaderException"/> for every reported message.
    /// </summary>
    internal sealed class ThrowingShaderErrorReporter : IShaderErrorReporter
    {
        /// <inheritdoc/>
        public void Report(string message)
        {
        }
    }
}
