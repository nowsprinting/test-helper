// Copyright (c) 2023-2025 Koji Hasegawa.
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

        [LoadAsset("../../Prefabs/Cube.prefab")]
        private GameObject _relative;

        [LoadAsset("../../Prefabs/Cube.prefab")]
        private static GameObject s_static;

        [field: LoadAsset("../../Prefabs/Cube.prefab")]
        [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
        private GameObject Prefab { get; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            LoadAssetAttribute.LoadAssets(this);
        }

        [Test]
        public void LoadAssets_Field_SetAssetField()
        {
            Assume.That(_prefab, Is.Not.Null);
            Assert.That(_prefab.name, Is.EqualTo("Cube"));
        }

        [Test]
        public void LoadAssets_FieldWithRelativePath_SetAssetField()
        {
            Assume.That(_relative, Is.Not.Null);
            Assert.That(_relative.name, Is.EqualTo("Cube"));
        }

        [Test]
        public void LoadAssets_StaticField_SetAssetField()
        {
            Assume.That(s_static, Is.Not.Null);
            Assert.That(s_static.name, Is.EqualTo("Cube"));
        }

        [Test]
        public void LoadAssets_Property_SetAssetField()
        {
            Assume.That(Prefab, Is.Not.Null);
            Assert.That(Prefab.name, Is.EqualTo("Cube"));
        }

        #region internal methods tests

        private static IEnumerable<TestCaseData> GetAbsolutePathTestCases()
        {
            yield return new TestCaseData("./Foo.txt",
                    "Assets/Tests/Runtime/Caller.cs",
                    "Assets/Tests/Runtime/Foo.txt")
                .SetName(nameof(GetAbsolutePath) + "(current path)");
            yield return new TestCaseData("../../DummyDirectory/../Foo/Bar.txt",
                    "Packages/com.nowsprinting.test-helper/Tests/Runtime/Attributes/Caller.cs",
                    "Packages/com.nowsprinting.test-helper/Tests/Foo/Bar.txt")
                .SetName(nameof(GetAbsolutePath) + "(upstream path)");
        }

        [TestCaseSource(nameof(GetAbsolutePathTestCases))]
        public void GetAbsolutePath(string relativePath, string callerFilePath, string expected)
        {
            var actual = LoadAssetAttribute.GetAbsolutePath(relativePath, callerFilePath);
            Assert.That(actual, Is.EqualTo(expected));
        }

        #endregion
    }
}
