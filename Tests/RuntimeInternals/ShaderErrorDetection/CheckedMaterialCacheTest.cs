// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;
using UnityEngine;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    [TestFixture]
    public class CheckedMaterialCacheTest
    {
        [Test]
        public void TryMarkChecked_NotYetCheckedMaterial_ReturnsTrue()
        {
            var cache = new CheckedMaterialCache();
            var material = new Material(Shader.Find("Sprites/Default"));

            Assert.That(cache.TryMarkChecked(material), Is.True);
        }

        [Test]
        public void TryMarkChecked_AlreadyCheckedMaterial_ReturnsFalse()
        {
            var cache = new CheckedMaterialCache();
            var material = new Material(Shader.Find("Sprites/Default"));
            cache.TryMarkChecked(material);

            Assert.That(cache.TryMarkChecked(material), Is.False);
        }

        [Test]
        public void TryMarkChecked_AfterClear_ReturnsTrue()
        {
            var cache = new CheckedMaterialCache();
            var material = new Material(Shader.Find("Sprites/Default"));
            cache.TryMarkChecked(material);

            cache.Clear();

            Assert.That(cache.TryMarkChecked(material), Is.True);
        }
    }
}
