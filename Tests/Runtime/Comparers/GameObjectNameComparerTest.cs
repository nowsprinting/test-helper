// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
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
            var actual = new GameObject("test object");

            Assert.That(actual, Is.EqualTo(new GameObject("test object")).Using(new GameObjectNameComparer()));
        }

        [Test]
        public void UsingWithEqualTo_NotEqualName_Failure()
        {
            var actual = new GameObject("actual object");

            Assert.That(() =>
            {
                Assert.That(actual, Is.EqualTo(new GameObject("expected object")).Using(new GameObjectNameComparer()));
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: <expected object (UnityEngine.GameObject)>{Environment.NewLine}  But was:  <actual object (UnityEngine.GameObject)>{Environment.NewLine}"));
        }

        [Test]
        public void UsingWithCollection_CompareGameObjectsByName()
        {
            var actual = new[] { new GameObject("test1"), new GameObject("test2"), new GameObject("test3"), };

            Assert.That(actual, Does.Contain(new GameObject("test3")).Using(new GameObjectNameComparer()));
        }

        [Test]
        public void UsingWithCollection_NotContain_Failure()
        {
            var actual = new[] { new GameObject("test1"), new GameObject("test2"), new GameObject("test3"), };

            Assert.That(() =>
            {
                Assert.That(actual, Does.Contain(new GameObject("test4")).Using(new GameObjectNameComparer()));
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: collection containing <test4 (UnityEngine.GameObject)>{Environment.NewLine}  But was:  < <test1 (UnityEngine.GameObject)>, <test2 (UnityEngine.GameObject)>, <test3 (UnityEngine.GameObject)> >{Environment.NewLine}"));
        }
    }
}
