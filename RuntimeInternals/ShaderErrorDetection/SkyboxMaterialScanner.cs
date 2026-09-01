// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Scans <c>RenderSettings.skybox</c> for shader errors.
    /// </summary>
    internal sealed class SkyboxMaterialScanner : IMaterialScanner
    {
        private readonly CheckedMaterialCache _cache;

        internal SkyboxMaterialScanner(CheckedMaterialCache cache)
        {
            _cache = cache;
        }

        /// <inheritdoc/>
        public IEnumerable<string> Scan()
        {
            var material = RenderSettings.skybox;
            if (material != null && _cache.TryMarkCheckedError(material, out var reason))
            {
                return new[] { $"Skybox : {reason}" };
            }

            return Array.Empty<string>();
        }
    }
}
