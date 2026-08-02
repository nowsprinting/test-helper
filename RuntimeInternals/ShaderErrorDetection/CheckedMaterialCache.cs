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
        // GetInstanceID() is obsolete from Unity 6.4 (6000.4) onward in favor of EntityId; this is the
        // only place object identity is needed, so the version branch is kept local to this class.
#if UNITY_6000_4_OR_NEWER
        private readonly HashSet<(EntityId MaterialId, EntityId ShaderId)> _checkedKeys =
            new HashSet<(EntityId, EntityId)>();
#else
        private readonly HashSet<(int MaterialId, int ShaderId)> _checkedKeys =
            new HashSet<(int, int)>();
#endif

        /// <summary>
        /// Marks the material (paired with its current shader) as checked.
        /// Returns false if the pair was already marked.
        /// </summary>
        internal bool TryMarkChecked(Material material)
        {
            // A null shader is itself the error state MaterialValidator flags, so it must not throw
            // here; it maps to the "never a real object" sentinel ID instead. The Unity fake-null
            // operator also sends a destroyed shader to the sentinel, consistent with
            // MaterialValidator.IsErrorShader treating it as an error via the same operator.
            var shader = material.shader;
#if UNITY_6000_4_OR_NEWER
            var key = (material.GetEntityId(), shader != null ? shader.GetEntityId() : default(EntityId));
#else
            var key = (material.GetInstanceID(), shader != null ? shader.GetInstanceID() : 0);
#endif
            return _checkedKeys.Add(key);
        }

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
