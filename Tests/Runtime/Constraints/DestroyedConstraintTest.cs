// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Windows;
using Object = UnityEngine.Object;

namespace TestHelper.Constraints
{
    public class DestroyedConstraintTest
    {
        private static GameObject CreateDestroyedObject()
        {
            var gameObject = new GameObject();
            Object.DestroyImmediate(gameObject);
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

            try
            {
                Assert.That(actual, Is.Destroyed);
                Assert.Fail();
            }
            catch (AssertionException e)
            {
                Assert.That(e.Message,
                    Is.EqualTo(
                        $"  Expected: destroyed GameObject{Environment.NewLine}  But was:  <Foo (UnityEngine.GameObject)>{Environment.NewLine}"));
            }
        }

        [Test]
        public void IsDestroyed_Null_Failure()
        {
            GameObject actual = null;

            try
            {
                Assert.That(actual, Is.Destroyed);
                Assert.Fail();
            }
            catch (AssertionException e)
            {
                Assert.That(e.Message,
                    Is.EqualTo(
                        $"  Expected: destroyed GameObject{Environment.NewLine}  But was:  null{Environment.NewLine}"));
            }
        }

        [Test]
        public void IsDestroyed_NotGameObject_Failure()
        {
            var actual = string.Empty;

            try
            {
                Assert.That(actual, Is.Destroyed);
                Assert.Fail();
            }
            catch (AssertionException e)
            {
                Assert.That(e.Message,
                    Is.EqualTo(
                        $"  Expected: destroyed GameObject{Environment.NewLine}  But was:  <string.Empty>{Environment.NewLine}"));
            }
        }

        [Test]
        public void IsNotDestroyed_DestroyedGameObject_Failure()
        {
            var actual = CreateDestroyedObject();

            try
            {
                Assert.That(actual, Is.Not.Destroyed()); // Note: Use it in method style when with operators
                Assert.Fail();
            }
            catch (AssertionException e)
            {
                Assert.That(e.Message,
                    Is.EqualTo(
                        $"  Expected: not destroyed GameObject{Environment.NewLine}  But was:  <null>{Environment.NewLine}"));
            }
        }

        [Test]
        public void IsNotDestroyed_NotDestroyedGameObject_Success()
        {
            var actual = new GameObject("Foo");

            Assert.That(actual, Is.Not.Destroyed()); // Note: Use it in method style when with operators
        }
    }
}
