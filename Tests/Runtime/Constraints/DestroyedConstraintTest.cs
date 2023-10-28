// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using UnityEngine;

namespace TestHelper.Constraints
{
    [SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
    public class DestroyedConstraintTest
    {
        private static GameObject CreateDestroyedObject()
        {
            var gameObject = new GameObject();
            GameObject.DestroyImmediate(gameObject);
            return gameObject;
        }

        [Test]
        public void IsDestroyed_DestroyedGameObject_Success()
        {
            var actual = CreateDestroyedObject();

            Assert.That(actual, Is.Destroyed);
        }

        [Test]
        public void IsDestroyed_NotDestroyedGameObject_Failure()
        {
            var actual = new GameObject("Foo");

            Assert.That(() =>
            {
                Assert.That(actual, Is.Destroyed);
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: destroyed GameObject{Environment.NewLine}  But was:  <Foo (UnityEngine.GameObject)>{Environment.NewLine}"));
        }

        [Test]
        public void IsDestroyed_Null_Failure()
        {
            GameObject actual = null;

            Assert.That(() =>
            {
                // ReSharper disable once ExpressionIsAlwaysNull
                Assert.That(actual, Is.Destroyed);
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: destroyed GameObject{Environment.NewLine}  But was:  null{Environment.NewLine}"));
        }

        [Test]
        public void IsDestroyed_NotGameObject_Failure()
        {
            var actual = string.Empty;

            Assert.That(() =>
            {
                Assert.That(actual, Is.Destroyed);
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: destroyed GameObject{Environment.NewLine}  But was:  <string.Empty>{Environment.NewLine}"));
        }

        [Test]
        public void IsNotDestroyed_DestroyedGameObject_Failure()
        {
            var actual = CreateDestroyedObject();

            Assert.That(() =>
            {
                Assert.That(actual, Is.Not.Destroyed()); // Note: Use it in method style when with operators
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: not destroyed GameObject{Environment.NewLine}  But was:  <null>{Environment.NewLine}"));
        }

        [Test]
        public void IsNotDestroyed_NotDestroyedGameObject_Success()
        {
            var actual = new GameObject("Foo");

            Assert.That(actual, Is.Not.Destroyed()); // Note: Use it in method style when with operators
        }
    }
}
