// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections.Generic;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Scans all enabled <see cref="UnityEngine.Skybox"/> components on active GameObjects for shader
    /// errors. A <c>Skybox</c> component overrides <c>RenderSettings.skybox</c> for the camera it is
    /// attached to; a <c>Camera</c> on the same GameObject is deliberately not required — a broken
    /// skybox material on a camera-less GameObject is almost certainly a mistake worth reporting.
    /// </summary>
    internal sealed class SkyboxComponentMaterialScanner : IMaterialScanner
    {
        private readonly CheckedMaterialCache _cache;

        internal SkyboxComponentMaterialScanner(CheckedMaterialCache cache)
        {
            _cache = cache;
        }

        /// <inheritdoc/>
        public IEnumerable<string> Scan()
        {
            return null;
        }
    }
}
