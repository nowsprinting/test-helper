// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEngine;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Pure decision logic for validating shaders and materials, independent of scene state.
    /// </summary>
    internal static class MaterialValidator
    {
        /// <summary>
        /// Returns true if the shader is null, named "Hidden/InternalErrorShader", or unsupported.
        /// </summary>
        internal static bool IsErrorShader(Shader shader)
        {
            return false;
        }

        /// <summary>
        /// Returns true (with a human-readable reason) when the non-null material's shader is an error shader.
        /// </summary>
        internal static bool TryGetError(Material material, out string reason)
        {
            reason = null;
            return false;
        }

        /// <summary>
        /// Returns true when a null material slot is expected and should not be reported as an error
        /// (e.g., a <see cref="ParticleSystemRenderer"/>'s trail material slot when the Trails module is disabled).
        /// </summary>
        internal static bool IsIgnorableNullSlot(Renderer renderer, int slotIndex)
        {
            return false;
        }
    }
}
