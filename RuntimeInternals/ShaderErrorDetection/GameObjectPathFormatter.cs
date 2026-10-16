// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEngine;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    /// <summary>
    /// Formats a GameObject's hierarchy path for use in shader error messages.
    /// </summary>
    internal static class GameObjectPathFormatter
    {
        /// <summary>
        /// Returns the slash-separated hierarchy path from the root to the given GameObject, inclusive.
        /// </summary>
        internal static string GetPath(GameObject go)
        {
            var path = go.name;
            var parent = go.transform.parent;
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return path;
        }
    }
}
