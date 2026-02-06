// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using UnityEngine;

namespace TestHelper.Attributes
{
    [TestFixture]
    public class LoadAssetAttributeTest
    {
        [LoadAsset("Packages/com.nowsprinting.test-helper/Tests/Prefabs/Cube.prefab")]
        private GameObject _prefab;

        [LoadAsset("../../Prefabs/Sphere.prefab")]
        private GameObject _relative;

        [LoadAsset("../../Prefabs/Capsule.prefab")]
        private static GameObject s_static;

        [field: LoadAsset("../../Prefabs/Cylinder.prefab")]
        [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
        private GameObject Prefab { get; }

        [LoadAsset("../../Images/LoadAssetAttributeTest.png")]
        private Texture2D _texture2d;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            LoadAssetAttribute.LoadAssets(this);
        }

        [Test]
        public void LoadAssets_Field_SetField()
        {
            Assume.That(_prefab, Is.Not.Null);
            Assert.That(_prefab.name, Is.EqualTo("Cube"));
        }

        [Test]
        public void LoadAssets_FieldWithRelativePath_SetField()
        {
            Assume.That(_relative, Is.Not.Null);
            Assert.That(_relative.name, Is.EqualTo("Sphere"));
        }

        [Test]
        public void LoadAssets_StaticField_SetField()
        {
            Assume.That(s_static, Is.Not.Null);
            Assert.That(s_static.name, Is.EqualTo("Capsule"));
        }

        [Test]
        public void LoadAssets_Property_SetField()
        {
            Assume.That(Prefab, Is.Not.Null);
            Assert.That(Prefab.name, Is.EqualTo("Cylinder"));
        }

        [Test]
        public void LoadAssets_ImageToTexture2D_SetField()
        {
            Assume.That(_texture2d, Is.Not.Null);
            Assert.That(_texture2d.format, Is.EqualTo(TextureFormat.RGBA32));
            Assert.That(_texture2d.width, Is.EqualTo(640));
            Assert.That(_texture2d.height, Is.EqualTo(480));
        }
    }
}
