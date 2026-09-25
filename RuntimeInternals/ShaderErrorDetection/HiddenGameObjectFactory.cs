// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEngine;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Creates hidden, scene-persistent <c>GameObject</c>s for driving components that must keep
    /// running across scene loads without being visible in the hierarchy or saved with the scene.
    /// </summary>
    internal static class HiddenGameObjectFactory
    {
        /// <summary>
        /// Creates a <see cref="HideFlags.HideAndDontSave"/>, <c>DontDestroyOnLoad</c> <c>GameObject</c>
        /// named <paramref name="name"/> and returns its newly added <typeparamref name="T"/> component.
        /// </summary>
        internal static T CreateHidden<T>(string name) where T : Component
        {
            var go = new GameObject(name) { hideFlags = HideFlags.HideAndDontSave };
            Object.DontDestroyOnLoad(go);
            return go.AddComponent<T>();
        }
    }
}
