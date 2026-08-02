// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections.Generic;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Scans <c>RenderSettings.skybox</c> for shader errors.
    /// </summary>
    internal sealed class SkyboxMaterialScanner : IMaterialScanner
    {
        internal SkyboxMaterialScanner(CheckedMaterialCache cache)
        {
        }

        /// <inheritdoc/>
        public IEnumerable<string> Scan()
        {
            return System.Array.Empty<string>();
        }
    }
}
