// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;
using UnityEngine;

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
                    Is.EqualTo("  Expected: destroyed GameObject\n  But was:  <Foo (UnityEngine.GameObject)>\n"));
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
                Assert.That(e.Message, Is.EqualTo("  Expected: destroyed GameObject\n  But was:  null\n"));
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
                Assert.That(e.Message, Is.EqualTo("  Expected: destroyed GameObject\n  But was:  <string.Empty>\n"));
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
                Assert.That(e.Message, Is.EqualTo("  Expected: not destroyed GameObject\n  But was:  <null>\n"));
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
