// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections.Generic;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Hierarchy-scanning detection method: scans a category of objects for shader errors.
    /// </summary>
    internal interface IMaterialScanner
    {
        /// <summary>
        /// Scans and returns messages describing detected shader errors
        /// (including GameObject path, material, and/or shader name).
        /// </summary>
        IEnumerable<string> Scan();
    }
}
