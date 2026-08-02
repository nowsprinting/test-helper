// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Scans all active <see cref="UnityEngine.Renderer"/>s in the hierarchy for shader errors.
    /// </summary>
    internal sealed class RendererMaterialScanner : IMaterialScanner
    {
        private readonly CheckedMaterialCache _cache;

        internal RendererMaterialScanner(CheckedMaterialCache cache)
        {
            _cache = cache;
        }

        /// <inheritdoc/>
        public IEnumerable<string> Scan()
        {
            var findings = new List<string>();
            foreach (var renderer in ActiveObjectFinder.FindActive<Renderer>())
            {
                var materials = renderer.sharedMaterials;
                for (var slotIndex = 0; slotIndex < materials.Length; slotIndex++)
                {
                    var material = materials[slotIndex];
                    if (material == null)
                    {
                        if (!MaterialValidator.IsIgnorableNullSlot(renderer, slotIndex))
                        {
                            findings.Add(
                                $"GameObject '{GameObjectPathFormatter.GetPath(renderer.gameObject)}' : material slot {slotIndex} is null");
                        }

                        continue;
                    }

                    if (!_cache.TryMarkChecked(material))
                    {
                        continue;
                    }

                    if (MaterialValidator.TryGetError(material, out var reason))
                    {
                        findings.Add($"GameObject '{GameObjectPathFormatter.GetPath(renderer.gameObject)}' : {reason}");
                    }
                }
            }

            return findings;
        }
    }
}
