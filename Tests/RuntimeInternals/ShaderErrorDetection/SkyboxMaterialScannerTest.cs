// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Linq;
using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    [TestFixture]
    [Category("Integration")]
    public class SkyboxMaterialScannerTest
    {
        private Material _originalSkybox;

        [SetUp]
        public void SetUp()
        {
            _originalSkybox = RenderSettings.skybox;
        }

        [TearDown]
        public void TearDown()
        {
            RenderSettings.skybox = _originalSkybox;
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void RenderedSkyboxWithErrorShaderMaterial_ReturnsFinding()
        {
            RenderSettings.skybox = new Material(Shader.Find("Hidden/InternalErrorShader")) { name = "BrokenSkyboxMaterial" };

            var findings = new SkyboxMaterialScanner(new CheckedMaterialCache()).Scan().ToArray();

            Assert.That(findings, Has.Length.EqualTo(1));
            Assert.That(findings[0], Does.Contain("BrokenSkyboxMaterial"));
        }

        [Test]
        [CreateScene]
        public void RenderedSkyboxWithSupportedShaderMaterial_ReturnsNoFinding()
        {
            var shader = Shader.Find("Skybox/Procedural") ?? Shader.Find("Sprites/Default");
            Assume.That(shader, Is.Not.Null);
            RenderSettings.skybox = new Material(shader);

            var findings = new SkyboxMaterialScanner(new CheckedMaterialCache()).Scan().ToArray();

            Assert.That(findings, Is.Empty);
        }

        [Test]
        [CreateScene]
        public void SkyboxIsNotSet_ReturnsNoFinding()
        {
            RenderSettings.skybox = null;

            var findings = new SkyboxMaterialScanner(new CheckedMaterialCache()).Scan().ToArray();

            Assert.That(findings, Is.Empty);
        }
    }
}
