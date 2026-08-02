// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine;

namespace TestHelper.Constraints
{
    [SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
    public class DestroyedConstraintTest
    {
        public enum ObjectKind
        {
            GameObject,
            Component,
            ScriptableObject,
        }

        private class DemoComponent : MonoBehaviour
        {
        }

        // Cleans up the ScriptableObject created by IsNotDestroyed_AliveObject_Success: unlike a GameObject,
        // it is not covered by [CreateScene] and would otherwise leak into subsequent tests.
        private ScriptableObject _createdScriptableObject;

        [TearDown]
        public void TearDown()
        {
            if (_createdScriptableObject != null)
            {
                UnityEngine.Object.DestroyImmediate(_createdScriptableObject);
                _createdScriptableObject = null;
            }
        }

        private static UnityEngine.Object CreateObject(ObjectKind kind)
        {
            switch (kind)
            {
                case ObjectKind.GameObject:
                    return new GameObject("Foo");
                case ObjectKind.Component:
                    return new GameObject("Foo").AddComponent<DemoComponent>();
                case ObjectKind.ScriptableObject:
                    return ScriptableObject.CreateInstance<ScriptableObject>();
                default:
                    return new GameObject("Foo");
            }
        }

        private static UnityEngine.Object CreateDestroyedObject(ObjectKind kind)
        {
            var actual = CreateObject(kind);
            UnityEngine.Object.DestroyImmediate(actual);
            return actual;
        }

        [Test]
        [CreateScene]
        public void IsDestroyed_DestroyedObject_Success([Values] ObjectKind kind)
        {
            var actual = CreateDestroyedObject(kind);

            Assert.That(actual, Is.Destroyed);
        }

        [Test]
        [CreateScene]
        public void IsDestroyed_AliveGameObject_Failure()
        {
            var actual = new GameObject("Foo");

            Assert.That(() =>
            {
                Assert.That(actual, Is.Destroyed);
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: destroyed UnityEngine.Object{Environment.NewLine}  But was:  <Foo (UnityEngine.GameObject)>{Environment.NewLine}"));
        }

        [Test]
        public void IsDestroyed_Null_ThrowsArgumentNullException()
        {
            Assert.That(() =>
            {
                Assert.That(null, Is.Destroyed);
            }, Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("actual"));
        }

        [Test]
        public void IsDestroyed_UnsupportedActualType_ThrowsArgumentException()
        {
            Assert.That(() =>
            {
                // Not a swapped actual/expected: this constant IS the actual value under test, deliberately an
                // unsupported type, to exercise the "not a UnityEngine.Object" failure path.
                Assert.That("not a UnityEngine.Object", Is.Destroyed);
            }, Throws.TypeOf<ArgumentException>()
                .With.Property("ParamName").EqualTo("actual")
                .And.Message.Contains("is not a UnityEngine.Object"));
        }

        [Test]
        [CreateScene]
        public void IsNotDestroyed_AliveObject_Success([Values] ObjectKind kind)
        {
            var actual = CreateObject(kind);
            _createdScriptableObject = actual as ScriptableObject;

            Assert.That(actual, Is.Not.Destroyed);
        }

        [Test]
        [CreateScene]
        public void IsNotDestroyed_DestroyedGameObject_Failure()
        {
            var actual = CreateDestroyedObject(ObjectKind.GameObject);

            Assert.That(() =>
            {
                Assert.That(actual, Is.Not.Destroyed);
            }, Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                $"  Expected: not destroyed UnityEngine.Object{Environment.NewLine}  But was:  <null>{Environment.NewLine}"));
        }

        [Test]
        public void IsNotDestroyed_Null_ThrowsArgumentNullException()
        {
            Assert.That(() =>
            {
                Assert.That(null, Is.Not.Destroyed);
            }, Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("actual"));
        }

        [Test]
        public void IsNotDestroyed_UnsupportedActualType_ThrowsArgumentException()
        {
            Assert.That(() =>
            {
                // Not a swapped actual/expected: this constant IS the actual value under test, deliberately an
                // unsupported type, to exercise the "not a UnityEngine.Object" failure path.
                Assert.That("not a UnityEngine.Object", Is.Not.Destroyed);
            }, Throws.TypeOf<ArgumentException>()
                .With.Property("ParamName").EqualTo("actual")
                .And.Message.Contains("is not a UnityEngine.Object"));
        }
    }
}
