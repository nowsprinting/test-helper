// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

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
            List<string> findings = null;
            foreach (var skybox in ActiveObjectFinder.FindActive<Skybox>())
            {
                // FindActive excludes inactive GameObjects but not disabled Behaviours.
                if (!skybox.enabled)
                {
                    continue;
                }

                var material = skybox.material;
                if (material == null)
                {
                    continue;
                }

                if (_cache.TryMarkCheckedError(material, out var reason))
                {
                    (findings ?? (findings = new List<string>())).Add(
                        $"GameObject '{GameObjectPathFormatter.GetPath(skybox.gameObject)}' : {reason}");
                }
            }

            return findings ?? (IEnumerable<string>)Array.Empty<string>();
        }
    }
}
