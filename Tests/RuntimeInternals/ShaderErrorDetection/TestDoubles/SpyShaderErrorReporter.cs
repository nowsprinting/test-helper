// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections.Generic;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection.TestDoubles
{
    /// <summary>
    /// Spy <see cref="IShaderErrorReporter"/> that records every reported message instead of throwing.
    /// </summary>
    internal sealed class SpyShaderErrorReporter : IShaderErrorReporter
    {
        internal readonly List<string> ReportedMessages = new List<string>();

        /// <inheritdoc/>
        public void Report(string message)
        {
            ReportedMessages.Add(message);
        }
    }
}
