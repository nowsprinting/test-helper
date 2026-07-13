// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using UnityEngine;

namespace TestHelper.RuntimeInternals
{
    /// <summary>
    /// Loads <see cref="AssetRootMapping"/> from the Resources at <see cref="AssetRootMapping.ResourcePath"/>.
    /// Loads lazily and only once; the result (including the negative result: not found or malformed) is cached
    /// in instance fields, so re-assigning a new instance to <see cref="AssetPathHelper.MappingLoader"/>
    /// effectively resets the cache.
    /// </summary>
    internal class ResourcesAssetRootMappingLoader : IAssetRootMappingLoader
    {
        private bool _loaded;
        private AssetRootMapping _mapping;

        /// <inheritdoc/>
        public AssetRootMapping Load()
        {
            if (_loaded)
            {
                return _mapping;
            }

            _loaded = true;
            try
            {
                var textAsset = Resources.Load<TextAsset>(AssetRootMapping.ResourcePath);
                if (textAsset != null)
                {
                    _mapping = JsonUtility.FromJson<AssetRootMapping>(textAsset.text);
                }
            }
            catch (Exception)
            {
                _mapping = null;
                // Note: Do not use Exception (and Assert). Because freezes async tests on UTF v1.3.4, See UUM-25085.
            }

            return _mapping;
        }
    }
}
