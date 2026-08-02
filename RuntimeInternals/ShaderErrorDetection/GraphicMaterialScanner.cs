// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections.Generic;
using UnityEngine.UI;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Scans all active uGUI <see cref="UnityEngine.UI.Graphic"/> components in the hierarchy for shader errors.
    /// </summary>
    internal sealed class GraphicMaterialScanner : IMaterialScanner
    {
        private readonly CheckedMaterialCache _cache;

        internal GraphicMaterialScanner(CheckedMaterialCache cache)
        {
            _cache = cache;
        }

        /// <inheritdoc/>
        public IEnumerable<string> Scan()
        {
            var findings = new List<string>();
            foreach (var graphic in ActiveObjectFinder.FindActive<Graphic>())
            {
                var material = graphic.materialForRendering;
                if (material == null)
                {
                    continue;
                }

                if (!_cache.TryMarkChecked(material))
                {
                    continue;
                }

                if (MaterialValidator.TryGetError(material, out var reason))
                {
                    findings.Add($"GameObject '{GameObjectPathFormatter.GetPath(graphic.gameObject)}' : {reason}");
                }
            }

            return findings;
        }
    }
}
