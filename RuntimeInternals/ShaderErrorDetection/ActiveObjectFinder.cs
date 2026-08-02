// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEngine;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Finds all active scene objects of a given type, using the non-obsolete Object-find API
    /// available on the running Unity version.
    /// </summary>
    internal static class ActiveObjectFinder
    {
        /// <summary>
        /// Returns all active-in-hierarchy objects of type <typeparamref name="T"/> in the loaded scenes.
        /// </summary>
        internal static T[] FindActive<T>() where T : Object
        {
#if UNITY_6000_4_OR_NEWER
            // The (FindObjectsInactive, FindObjectsSortMode) and (FindObjectsSortMode) overloads are
            // obsolete on 6000.4+ because FindObjectsSortMode relied on InstanceID-order sorting, which
            // can no longer be guaranteed after the InstanceID -> EntityId migration.
            return Object.FindObjectsByType<T>();
#elif UNITY_2022_3_OR_NEWER
            return Object.FindObjectsByType<T>(FindObjectsSortMode.None);
#else
            return Object.FindObjectsOfType<T>();
#endif
        }
    }
}
