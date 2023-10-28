// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;
using UnityEngine;

namespace TestHelper.Comparers
{
    [TestFixture]
    public class GameObjectNameComparerTest
    {
        [Test]
        public void UsingWithEqualTo_CompareGameObjectsByName()
        {
            var actual = new GameObject("test");

            Assert.That(actual, Is.EqualTo(new GameObject("test")).Using(new GameObjectNameComparer()));
            // Message if failure:
            //   Expected: <test1 (UnityEngine.GameObject)>
            //   But was:  <test (UnityEngine.GameObject)>
        }

        [Test]
        public void UsingWithCollection_CompareGameObjectsByName()
        {
            var actual = new []
            {
                new GameObject("test1"), new GameObject("test2"), new GameObject("test3"),
            };

            Assert.That(actual, Does.Contain(new GameObject("test3")).Using(new GameObjectNameComparer()));
            // Message if failure:
            //   Expected: collection containing <test4 (UnityEngine.GameObject)>
            //   But was:  < <test1 (UnityEngine.GameObject)>, <test2 (UnityEngine.GameObject)>, <test3 (UnityEngine.GameObject)> >
        }
    }
}
