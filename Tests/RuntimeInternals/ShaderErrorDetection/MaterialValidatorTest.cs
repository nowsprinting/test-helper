// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    [TestFixture]
    public class MaterialValidatorTest
    {
        private const string InternalErrorShaderName = "Hidden/InternalErrorShader";
        private const string UnsupportedShaderResourcePath = "UnsupportedShader";

        [Test]
        public void IsErrorShader_NullShader_ReturnsTrue()
        {
            Assert.That(MaterialValidator.IsErrorShader(null), Is.True);
        }

        [Test]
        public void IsErrorShader_InternalErrorShader_ReturnsTrue()
        {
            var shader = Shader.Find(InternalErrorShaderName);
            Assume.That(shader, Is.Not.Null);

            Assert.That(MaterialValidator.IsErrorShader(shader), Is.True);
        }

        [Test]
        public void IsErrorShader_UnsupportedShader_ReturnsTrue()
        {
            var shader = Resources.Load<Shader>(UnsupportedShaderResourcePath);
            Assume.That(shader, Is.Not.Null);
            Assume.That(shader.isSupported, Is.False,
                "Fixture shader is expected to be unsupported on this platform; " +
                "skip when it unexpectedly compiles and is supported here.");

            Assert.That(MaterialValidator.IsErrorShader(shader), Is.True);
        }

        [Test]
        public void IsErrorShader_SupportedShader_ReturnsFalse()
        {
            var shader = Shader.Find("Sprites/Default");
            Assume.That(shader, Is.Not.Null);
            Assume.That(shader.isSupported, Is.True);

            Assert.That(MaterialValidator.IsErrorShader(shader), Is.False);
        }

        [Test]
        public void TryGetError_MaterialWithErrorShader_ReturnsTrueWithReason()
        {
            var shader = Shader.Find(InternalErrorShaderName);
            Assume.That(shader, Is.Not.Null);
            var material = new Material(shader) { name = "MaterialWithErrorShader" };

            var result = MaterialValidator.TryGetError(material, out var reason);

            Assert.That(result, Is.True);
            Assert.That(reason, Does.Contain(material.name));
            Assert.That(reason, Does.Contain(shader.name));
        }

        [Test]
        [LinuxHeadlessGpuUnsupported]
        public void TryGetError_MaterialWithSupportedShader_ReturnsFalse()
        {
            var shader = Shader.Find("Sprites/Default");
            Assume.That(shader, Is.Not.Null);
            var material = new Material(shader);

            var result = MaterialValidator.TryGetError(material, out _);

            Assert.That(result, Is.False);
        }

        [Test]
        [CreateScene]
        public void IsIgnorableNullSlot_TrailSlotOfParticleSystemRendererWithTrailsDisabled_ReturnsTrue()
        {
            var particleSystem =
                new GameObject(
                        nameof(IsIgnorableNullSlot_TrailSlotOfParticleSystemRendererWithTrailsDisabled_ReturnsTrue))
                    .AddComponent<ParticleSystem>();
            var trails = particleSystem.trails;
            trails.enabled = false;
            var renderer = particleSystem.GetComponent<ParticleSystemRenderer>();

            Assert.That(MaterialValidator.IsIgnorableNullSlot(renderer, 1), Is.True);
        }

        [Test]
        [CreateScene]
        public void IsIgnorableNullSlot_TrailSlotOfParticleSystemRendererWithTrailsEnabled_ReturnsFalse()
        {
            var particleSystem =
                new GameObject(
                        nameof(IsIgnorableNullSlot_TrailSlotOfParticleSystemRendererWithTrailsEnabled_ReturnsFalse))
                    .AddComponent<ParticleSystem>();
            var trails = particleSystem.trails;
            trails.enabled = true;
            var renderer = particleSystem.GetComponent<ParticleSystemRenderer>();

            Assert.That(MaterialValidator.IsIgnorableNullSlot(renderer, 1), Is.False);
        }

        [Test]
        [CreateScene]
        public void IsIgnorableNullSlot_NonTrailSlotOfParticleSystemRenderer_ReturnsFalse()
        {
            var particleSystem =
                new GameObject(nameof(IsIgnorableNullSlot_NonTrailSlotOfParticleSystemRenderer_ReturnsFalse))
                    .AddComponent<ParticleSystem>();
            var renderer = particleSystem.GetComponent<ParticleSystemRenderer>();

            Assert.That(MaterialValidator.IsIgnorableNullSlot(renderer, 0), Is.False);
        }

        [Test]
        [CreateScene]
        public void IsIgnorableNullSlot_SlotOfMeshRenderer_ReturnsFalse()
        {
            var renderer = GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<MeshRenderer>();

            Assert.That(MaterialValidator.IsIgnorableNullSlot(renderer, 0), Is.False);
        }
    }
}
