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

        [Test]
        [Category("Acceptance")]
        public void TryMarkChecked_ShaderSwappedOnCheckedMaterial_ReturnsTrue()
        {
            var cache = new CheckedMaterialCache();
            var material = new Material(Shader.Find("Sprites/Default"));
            cache.TryMarkChecked(material);

            material.shader = Shader.Find("Hidden/InternalErrorShader");

            Assert.That(cache.TryMarkChecked(material), Is.True);
        }

        [Test]
        [Category("Acceptance")]
        public void TryMarkChecked_ShaderSwappedBackToCheckedShader_ReturnsFalse()
        {
            var cache = new CheckedMaterialCache();
            var checkedShader = Shader.Find("Sprites/Default");
            var material = new Material(checkedShader);
            cache.TryMarkChecked(material);
            material.shader = Shader.Find("Hidden/InternalErrorShader");
            Assume.That(cache.TryMarkChecked(material), Is.True); // precondition: the swapped shader was re-registered

            material.shader = checkedShader;

            Assert.That(cache.TryMarkChecked(material), Is.False);
        }

        [Test]
        public void TryMarkChecked_UncheckedMaterialWithCheckedShader_ReturnsTrue()
        {
            var cache = new CheckedMaterialCache();
            var shader = Shader.Find("Sprites/Default");
            cache.TryMarkChecked(new Material(shader));

            Assert.That(cache.TryMarkChecked(new Material(shader)), Is.True);
        }

        [Test]
        [Category("Acceptance")]
        public void TryMarkChecked_NullShaderMaterial_ReturnsTrue()
        {
            var cache = new CheckedMaterialCache();
            var material = new Material(Shader.Find("Sprites/Default")) { shader = null };

            Assert.That(cache.TryMarkChecked(material), Is.True);
        }

        [Test]
        [Category("Acceptance")]
        public void TryMarkChecked_AlreadyCheckedNullShaderMaterial_ReturnsFalse()
        {
            var cache = new CheckedMaterialCache();
            var material = new Material(Shader.Find("Sprites/Default")) { shader = null };
            cache.TryMarkChecked(material);

            Assert.That(cache.TryMarkChecked(material), Is.False);
        }
    }
}
