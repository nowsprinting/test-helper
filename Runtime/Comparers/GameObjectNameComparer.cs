// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace TestHelper.Comparers
{
    /// <summary>
    /// Compare two <c>GameObject</c> by name.
    /// </summary>
    /// <remarks>
    /// This class is reference implementation for custom comparer.
    /// </remarks>
    /// <example>
    /// <code>
    /// [TestFixture]
    /// public class MyTestClass
    /// {
    ///     [Test]
    ///     public void MyTestMethod()
    ///     {
    ///         var actual = FindGameObjects();
    ///         Assert.That(actual, Does.Contain(new GameObject("test")).Using(new GameObjectNameComparer()));
    ///     }
    /// }
    /// </code>
    /// </example>
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
