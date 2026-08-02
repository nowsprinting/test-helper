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
        private const string InternalErrorShaderName = "Hidden/InternalErrorShader";

        // A ParticleSystemRenderer's trail material occupies sharedMaterials[1]; that slot is
        // legitimately null when the Trails module is disabled, so it must not be reported.
        private const int ParticleSystemTrailMaterialSlotIndex = 1;

        /// <summary>
        /// Returns true if the shader is null, named "Hidden/InternalErrorShader", or unsupported.
        /// </summary>
        internal static bool IsErrorShader(Shader shader)
        {
            if (shader == null)
            {
                return true;
            }

            if (shader.name == InternalErrorShaderName)
            {
                return true;
            }

            return !shader.isSupported;
        }

        /// <summary>
        /// Returns true (with a human-readable reason) when the non-null material's shader is an error shader.
        /// </summary>
        internal static bool TryGetError(Material material, out string reason)
        {
            if (!IsErrorShader(material.shader))
            {
                reason = null;
                return false;
            }

            var shaderName = material.shader != null ? material.shader.name : "(missing)";
            reason = $"Material '{material.name}' has error shader '{shaderName}'";
            return true;
        }

        /// <summary>
        /// Returns true when a null material slot is expected and should not be reported as an error
        /// (e.g., a <see cref="ParticleSystemRenderer"/>'s trail material slot when the Trails module is disabled).
        /// </summary>
        internal static bool IsIgnorableNullSlot(Renderer renderer, int slotIndex)
        {
            if (slotIndex != ParticleSystemTrailMaterialSlotIndex)
            {
                return false;
            }

            if (!(renderer is ParticleSystemRenderer particleSystemRenderer))
            {
                return false;
            }

            var particleSystem = particleSystemRenderer.GetComponent<ParticleSystem>();
            if (particleSystem == null)
            {
                return false;
            }

            return !particleSystem.trails.enabled;
        }
    }
}
