// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections.Generic;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection.TestDoubles
{
    /// <summary>
    /// Spy <see cref="IMaterialScanner"/> that returns a configured, mutable set of finding
    /// messages and records how many times <see cref="Scan"/> was invoked.
    /// </summary>
    internal sealed class SpyMaterialScanner : IMaterialScanner
    {
        internal int ScanCallCount { get; private set; }

        /// <summary>
        /// Messages to return from the next <see cref="Scan"/> call.
        /// </summary>
        internal List<string> NextFindings = new List<string>();

        /// <inheritdoc/>
        public IEnumerable<string> Scan()
        {
            ScanCallCount++;
            return NextFindings.ToArray();
        }
    }
}
