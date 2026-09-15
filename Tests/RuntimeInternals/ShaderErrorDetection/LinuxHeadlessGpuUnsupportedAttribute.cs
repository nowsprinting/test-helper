// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEngine;
using UnityEngine.TestTools;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Excludes a shader error detection test from running on <c>RuntimePlatform.LinuxPlayer</c>.
    /// CI runs that player headless (<c>GraphicsDeviceType.Null</c>, no real GPU), where
    /// <c>Shader.isSupported</c> is false for every shader, so a material that is actually
    /// supported gets falsely reported as an error shader.
    /// </summary>
    internal sealed class LinuxHeadlessGpuUnsupportedAttribute : UnityPlatformAttribute
    {
        public LinuxHeadlessGpuUnsupportedAttribute()
        {
            exclude = new[] { RuntimePlatform.LinuxPlayer };
        }
    }
}
