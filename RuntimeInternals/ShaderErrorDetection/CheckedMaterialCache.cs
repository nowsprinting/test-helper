// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Tracks (material, shader) pairs already validated within a single detection session, so each
    /// pair is validated at most once regardless of which scanner encounters it first. Keying on the
    /// pair (not the material alone) means a material whose shader is swapped mid-session is
    /// re-validated; swapping back to an already-checked shader is not re-reported.
    /// </summary>
    internal sealed class CheckedMaterialCache
    {
#if UNITY_6000_4_OR_NEWER
        private readonly HashSet<(EntityId, EntityId)> _checkedKeys = new HashSet<(EntityId, EntityId)>();
#else
        private readonly HashSet<(int, int)> _checkedKeys = new HashSet<(int, int)>();
#endif

        /// <summary>
        /// Marks the material (paired with its current shader) as checked.
        /// Returns false if the pair was already marked.
        /// </summary>
        internal bool TryMarkChecked(Material material)
        {
            // A null shader is itself the error state MaterialValidator flags, so the key computation
            // must not throw on it; GetId maps null (and Unity fake-null) to a sentinel ID instead.
            return _checkedKeys.Add((GetId(material), GetId(material.shader)));
        }

        // GetInstanceID() is obsolete from Unity 6.4 (6000.4) onward in favor of EntityId; this is the
        // only place object identity is needed, so the version branch is kept local to this class.
        // The sentinel for null (0 / default(EntityId)) is never assigned to a real Unity object.
#if UNITY_6000_4_OR_NEWER
        private static EntityId GetId(Object obj)
        {
            return obj != null ? obj.GetEntityId() : default(EntityId);
        }
#else
        private static int GetId(Object obj)
        {
            return obj != null ? obj.GetInstanceID() : 0;
        }
#endif

        /// <summary>
        /// Marks the material as checked and validates it, unless it was already marked.
        /// </summary>
        internal bool TryMarkCheckedError(Material material, out string reason)
        {
            if (!TryMarkChecked(material))
            {
                reason = null;
                return false;
            }

            return MaterialValidator.TryGetError(material, out reason);
        }

        /// <summary>
        /// Clears all marked materials.
        /// </summary>
        internal void Clear()
        {
            _checkedKeys.Clear();
        }
    }
}
