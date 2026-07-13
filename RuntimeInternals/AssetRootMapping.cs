// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;

namespace TestHelper.RuntimeInternals
{
    /// <summary>
    /// Mapping from physical package/Assets root paths on the development machine to Unity asset root paths
    /// (`Assets` or `Packages/&lt;name&gt;`).
    /// Written at player-build time by <c>TestHelper.Editor.TemporaryCopyAssetsForPlayer</c>; read on the player
    /// by <see cref="AssetPathHelper"/> to resolve relative paths in packages that are not embedded packages
    /// (their source files do not exist on the device).
    /// </summary>
    [Serializable]
    internal class AssetRootMapping
    {
        /// <summary>
        /// Resource path (without extension) used with <c>Resources.Load</c>.
        /// </summary>
        internal const string ResourcePath = "com.nowsprinting.test-helper/AssetRootMapping";

        public List<Entry> entries = new List<Entry>(); // Note: JsonUtility requires public fields

        [Serializable]
        internal class Entry
        {
            public string physicalRoot; // Real path on the development machine, '/'-separators, no trailing slash
            public string assetRoot; // "Assets" or "Packages/<name>", no trailing slash
        }

        /// <summary>
        /// Add an entry; normalizes path separators to '/', trims the trailing slash, and dedupes by
        /// <c>physicalRoot</c> (Ordinal).
        /// </summary>
        internal void AddEntry(string physicalRoot, string assetRoot)
        {
            if (string.IsNullOrEmpty(physicalRoot))
            {
                return;
            }

            var normalizedRoot = NormalizeRoot(physicalRoot);
            foreach (var entry in entries)
            {
                if (entry.physicalRoot.Equals(normalizedRoot, StringComparison.Ordinal))
                {
                    return;
                }
            }

            entries.Add(new Entry { physicalRoot = normalizedRoot, assetRoot = assetRoot });
        }

        /// <summary>
        /// Pure prefix-match resolution: normalizes <paramref name="absolutePath"/> separators to '/', finds the
        /// entry with the longest <c>physicalRoot</c> that is a segment-boundary prefix of the path, and returns
        /// <c>assetRoot</c> + remainder. Returns null on no match. Never throws.
        /// </summary>
        /// <param name="absolutePath">Absolute path on the development machine</param>
        /// <returns>Unity asset path starting with `Assets/` or `Packages/`. Returns null if it can not be resolved.</returns>
        internal string Resolve(string absolutePath)
        {
            if (absolutePath == null || entries == null)
            {
                return null;
                // Note: Do not use Exception (and Assert). Because freezes async tests on UTF v1.3.4, See UUM-25085.
            }

            var normalizedPath = AssetPathHelper.ConvertToUnixPathSeparator(absolutePath);
            Entry longestMatch = null;
            var longestRootLength = -1;

            foreach (var entry in entries)
            {
                if (entry == null || string.IsNullOrEmpty(entry.physicalRoot) || entry.assetRoot == null)
                {
                    continue; // Entries may come from a hand-edited or corrupted JSON; skip instead of throwing.
                }

                // Re-normalize on match, so that entries deserialized from JSON not produced by AddEntry work too.
                var root = NormalizeRoot(entry.physicalRoot);

                // Case-insensitive, same as the Application.dataPath prefix check in AssetPathHelper; tolerates
                // Windows drive-letter casing variance between compiler-baked paths and Path.GetFullPath results.
                if (root.Length > longestRootLength &&
                    normalizedPath.StartsWith(root + "/", StringComparison.OrdinalIgnoreCase))
                {
                    longestMatch = entry;
                    longestRootLength = root.Length;
                }
            }

            if (longestMatch == null)
            {
                return null;
            }

            return longestMatch.assetRoot + normalizedPath.Substring(longestRootLength);
        }

        private static string NormalizeRoot(string path)
        {
            return AssetPathHelper.ConvertToUnixPathSeparator(path).TrimEnd('/');
        }
    }
}
