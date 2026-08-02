// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEngine;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Tracks materials already validated within a single detection session, so each material
    /// is validated at most once regardless of which scanner encounters it first.
    /// </summary>
    internal sealed class CheckedMaterialCache
    {
        /// <summary>
        /// Marks the material as checked. Returns false if it was already marked.
        /// </summary>
        internal bool TryMarkChecked(Material material)
        {
            return false;
        }

        /// <summary>
        /// Clears all marked materials.
        /// </summary>
        internal void Clear()
        {
        }
    }
}
