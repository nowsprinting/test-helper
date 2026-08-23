// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;

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
            public string assetRoot;    // "Assets" or "Packages/<name>", no trailing slash
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

        /// <summary>
        /// Resolve a caller directory and relative path when the compiler-baked <c>[CallerFilePath]</c> itself
        /// is relative (not rooted). This happens for packages that live inside the dev machine's project
        /// directory tree (e.g., embedded packages, or a `file:` local package under the project root) rather
        /// than referenced from outside it; Unity then bakes a path relative to the project root instead of an
        /// absolute one.
        /// </summary>
        /// <remarks>
        /// <see cref="Path.GetFullPath(string)"/> only ignores the caller process's current working directory
        /// when its input is already rooted; on the player, the working directory bears no relation to the dev
        /// machine, so a relative caller can not be combined directly (unlike <see cref="Resolve"/>, which is
        /// used for the already-rooted case). This method re-roots the combination on the dev-machine project
        /// root, derived from the always-present <c>Assets</c> entry (whose <c>physicalRoot</c> is
        /// `&lt;projectRoot&gt;/Assets`), before delegating to <see cref="Resolve"/>.
        /// </remarks>
        /// <param name="callerDirectory">Directory portion of a relative (not rooted) caller file path</param>
        /// <param name="relativePath">Relative path from the caller's file location</param>
        /// <returns>Unity asset path starting with `Assets/` or `Packages/`. Returns null if it can not be resolved.</returns>
        internal string ResolveRelativeCallerPath(string callerDirectory, string relativePath)
        {
            var projectRoot = GetProjectRoot();
            if (projectRoot == null)
            {
                return null;
            }

            var absolutePath = Path.GetFullPath(Path.Combine(projectRoot, callerDirectory ?? string.Empty,
                relativePath ?? string.Empty));
            return Resolve(absolutePath);
        }

        /// <summary>
        /// Derive the dev-machine project root (parent of the Assets folder) from the mapping's Assets entry.
        /// Pure string manipulation only (no filesystem access), so it is safe to call with the process's
        /// current working directory being unrelated to the dev machine (i.e., on the player).
        /// </summary>
        private string GetProjectRoot()
        {
            // Entries may come from a hand-edited or corrupted JSON; Find returning null is handled below
            // instead of throwing.
            var assetsEntry = entries?.Find(entry =>
                entry != null && entry.assetRoot == "Assets" && !string.IsNullOrEmpty(entry.physicalRoot));
            if (assetsEntry == null)
            {
                return null;
            }

            const string assetsSuffix = "/Assets";
            var assetsRoot = NormalizeRoot(assetsEntry.physicalRoot);
            return assetsRoot.EndsWith(assetsSuffix, StringComparison.Ordinal)
                ? assetsRoot.Substring(0, assetsRoot.Length - assetsSuffix.Length)
                : null;
        }
    }
}
