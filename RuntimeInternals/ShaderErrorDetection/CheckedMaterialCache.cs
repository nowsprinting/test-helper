// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Tracks materials already validated within a single detection session, so each material
    /// is validated at most once regardless of which scanner encounters it first.
    /// </summary>
    internal sealed class CheckedMaterialCache
    {
        // GetInstanceID() is obsolete from Unity 6.4 (6000.4) onward in favor of EntityId; this is the
        // only place object identity is needed, so the version branch is kept local to this class.
#if UNITY_6000_4_OR_NEWER
        private readonly HashSet<EntityId> _checkedIds = new HashSet<EntityId>();
#else
        private readonly HashSet<int> _checkedIds = new HashSet<int>();
#endif

        /// <summary>
        /// Marks the material as checked. Returns false if it was already marked.
        /// </summary>
        internal bool TryMarkChecked(Material material)
        {
#if UNITY_6000_4_OR_NEWER
            var id = material.GetEntityId();
#else
            var id = material.GetInstanceID();
#endif
            return _checkedIds.Add(id);
        }

        /// <summary>
        /// Clears all marked materials.
        /// </summary>
        internal void Clear()
        {
            _checkedIds.Clear();
        }
    }
}
