// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace TestHelper.Comparers
{
    /// <summary>
    /// Compare GameObjects by name.
    /// </summary>
    public class GameObjectNameComparer : IComparer<GameObject>
    {
        /// <inheritdoc/>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public int Compare(GameObject x, GameObject y)
        {
            return string.Compare(x.name, y.name, StringComparison.Ordinal);
        }
    }
}
