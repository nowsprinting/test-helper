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
    public class SkyboxComponentMaterialScannerTest
    {
        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void EnabledSkyboxComponentWithErrorShaderMaterial_ReturnsFinding()
        {
            var skybox = new GameObject("SkyboxComponentWithErrorShaderMaterial").AddComponent<Skybox>();
            skybox.material = new Material(Shader.Find("Hidden/InternalErrorShader"))
                { name = "BrokenSkyboxComponentMaterial" };

            var findings = new SkyboxComponentMaterialScanner(new CheckedMaterialCache()).Scan().ToArray();

            Assert.That(findings, Has.Length.EqualTo(1));
            Assert.That(findings[0], Does.Contain(skybox.gameObject.name));
            Assert.That(findings[0], Does.Contain("BrokenSkyboxComponentMaterial"));
        }

        [Test]
        [CreateScene]
        [LinuxHeadlessGpuUnsupported]
        public void EnabledSkyboxComponentWithSupportedShaderMaterial_ReturnsNoFinding()
        {
            var skybox = new GameObject("SkyboxComponentWithSupportedShaderMaterial").AddComponent<Skybox>();
            skybox.material = new Material(Shader.Find("Sprites/Default"));

            var findings = new SkyboxComponentMaterialScanner(new CheckedMaterialCache()).Scan().ToArray();

            Assert.That(findings, Is.Empty);
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void EnabledSkyboxComponentWithoutMaterial_ReturnsNoFinding()
        {
            new GameObject("SkyboxComponentWithoutMaterial").AddComponent<Skybox>();

            var findings = new SkyboxComponentMaterialScanner(new CheckedMaterialCache()).Scan().ToArray();

            Assert.That(findings, Is.Empty);
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void DisabledSkyboxComponentWithErrorShaderMaterial_ReturnsNoFinding()
        {
            var skybox = new GameObject("DisabledSkyboxComponentWithErrorShaderMaterial").AddComponent<Skybox>();
            skybox.material = new Material(Shader.Find("Hidden/InternalErrorShader"));
            skybox.enabled = false;

            var findings = new SkyboxComponentMaterialScanner(new CheckedMaterialCache()).Scan().ToArray();

            Assert.That(findings, Is.Empty);
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void SkyboxComponentWithErrorShaderMaterialOnInactiveGameObject_ReturnsNoFinding()
        {
            var skybox = new GameObject("InactiveSkyboxComponentWithErrorShaderMaterial").AddComponent<Skybox>();
            skybox.material = new Material(Shader.Find("Hidden/InternalErrorShader"));
            skybox.gameObject.SetActive(false);

            var findings = new SkyboxComponentMaterialScanner(new CheckedMaterialCache()).Scan().ToArray();

            Assert.That(findings, Is.Empty);
        }
    }
}
