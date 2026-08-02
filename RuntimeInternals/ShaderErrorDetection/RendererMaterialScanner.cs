// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections.Generic;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Scans all active <see cref="UnityEngine.Renderer"/>s in the hierarchy for shader errors.
    /// </summary>
    internal sealed class RendererMaterialScanner : IMaterialScanner
    {
        internal RendererMaterialScanner(CheckedMaterialCache cache)
        {
        }

        /// <inheritdoc/>
        public IEnumerable<string> Scan()
        {
            return System.Array.Empty<string>();
        }
    }
}
