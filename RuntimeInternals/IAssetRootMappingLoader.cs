// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

namespace TestHelper.RuntimeInternals
{
    /// <summary>
    /// Loader of <see cref="AssetRootMapping"/>.
    /// Injection seam for <see cref="AssetPathHelper"/>; tests can replace it with a stub returning fixed entries.
    /// </summary>
    internal interface IAssetRootMappingLoader
    {
        /// <summary>
        /// Returns the mapping, or null if unavailable. Must not throw.
        /// </summary>
        AssetRootMapping Load();
    }
}
