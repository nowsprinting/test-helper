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
    public class RendererMaterialScannerTest
    {
        private static Material SupportedMaterial => new Material(Shader.Find("Sprites/Default"));

        private static Material ErrorShaderMaterial => new Material(Shader.Find("Hidden/InternalErrorShader"))
            { name = "BrokenMaterial" };

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void RenderedRendererWithMissingShaderReferenceMaterial_ReturnsFinding()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "CubeWithMissingShaderMaterial";
            go.GetComponent<MeshRenderer>().sharedMaterial = ErrorShaderMaterial;

            var findings = new RendererMaterialScanner(new CheckedMaterialCache()).Scan().ToArray();

            Assert.That(findings, Has.Length.EqualTo(1));
            Assert.That(findings[0], Does.Contain(go.name));
            Assert.That(findings[0], Does.Contain("BrokenMaterial"));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void RenderedRendererWithUnsupportedShaderMaterial_ReturnsFinding()
        {
            var shader = Resources.Load<Shader>("UnsupportedShader");
            Assume.That(shader, Is.Not.Null);
            Assume.That(shader.isSupported, Is.False,
                "Fixture shader is expected to be unsupported on this platform; " +
                "skip when it unexpectedly compiles and is supported here.");

            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "CubeWithUnsupportedShaderMaterial";
            go.GetComponent<MeshRenderer>().sharedMaterial = new Material(shader) { name = "UnsupportedMaterial" };

            var findings = new RendererMaterialScanner(new CheckedMaterialCache()).Scan().ToArray();

            Assert.That(findings, Has.Length.EqualTo(1));
            Assert.That(findings[0], Does.Contain(go.name));
            Assert.That(findings[0], Does.Contain("UnsupportedMaterial"));
        }

        [Test]
        [CreateScene]
        public void RenderedRendererWithNullMaterialSlot_ReturnsFinding()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "CubeWithNullMaterialSlot";
            go.GetComponent<MeshRenderer>().sharedMaterials = new Material[] { null };

            var findings = new RendererMaterialScanner(new CheckedMaterialCache()).Scan().ToArray();

            Assert.That(findings, Has.Length.EqualTo(1));
            Assert.That(findings[0], Does.Contain(go.name));
            Assert.That(findings[0], Does.Contain("0")); // slot index
        }

        [Test]
        [CreateScene]
        [LinuxHeadlessGpuUnsupported]
        public void RenderedRendererWithSupportedShaderMaterial_ReturnsNoFinding()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.GetComponent<MeshRenderer>().sharedMaterial = SupportedMaterial;

            var findings = new RendererMaterialScanner(new CheckedMaterialCache()).Scan().ToArray();

            Assert.That(findings, Is.Empty);
        }

        [Test]
        [CreateScene]
        public void InactiveRendererWithErrorShaderMaterial_ReturnsNoFinding()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.GetComponent<MeshRenderer>().sharedMaterial = ErrorShaderMaterial;
            go.SetActive(false);

            var findings = new RendererMaterialScanner(new CheckedMaterialCache()).Scan().ToArray();

            Assert.That(findings, Is.Empty);
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        [LinuxHeadlessGpuUnsupported]
        public void RenderedParticleSystemRendererWithTrailsDisabled_ReturnsNoFinding()
        {
            var go = new GameObject("ParticleWithDisabledTrailsAndNullTrailSlot");
            var particleSystem = go.AddComponent<ParticleSystem>();
            var trails = particleSystem.trails;
            trails.enabled = false;
            var renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterials = new[] { SupportedMaterial, null };

            var findings = new RendererMaterialScanner(new CheckedMaterialCache()).Scan().ToArray();

            Assert.That(findings, Is.Empty);
        }

        [Test]
        [CreateScene]
        // On LinuxPlayer, the fixture's "supported" material also gets falsely flagged (see
        // LinuxHeadlessGpuUnsupportedAttribute), so the finding count no longer matches the
        // expected single trail-slot finding.
        [LinuxHeadlessGpuUnsupported]
        public void RenderedParticleSystemRendererWithTrailsEnabledAndNullTrailSlot_ReturnsFinding()
        {
            var go = new GameObject("ParticleWithEnabledTrailsAndNullTrailSlot");
            var particleSystem = go.AddComponent<ParticleSystem>();
            var trails = particleSystem.trails;
            trails.enabled = true;
            var renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterials = new[] { SupportedMaterial, null };

            var findings = new RendererMaterialScanner(new CheckedMaterialCache()).Scan().ToArray();

            Assert.That(findings, Has.Length.EqualTo(1));
            Assert.That(findings[0], Does.Contain(go.name));
            Assert.That(findings[0], Does.Contain("1")); // slot index
        }

        [Test]
        [CreateScene]
        public void SameErrorMaterialOnMultipleRenderedRenderers_ReturnsFindingOnce()
        {
            var sharedErrorMaterial = ErrorShaderMaterial;
            GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<MeshRenderer>().sharedMaterial =
                sharedErrorMaterial;
            GameObject.CreatePrimitive(PrimitiveType.Sphere).GetComponent<MeshRenderer>().sharedMaterial =
                sharedErrorMaterial;

            var findings = new RendererMaterialScanner(new CheckedMaterialCache()).Scan().ToArray();

            Assert.That(findings, Has.Length.EqualTo(1));
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void ScannedTwiceWithSameErrorMaterial_ReturnsNoFindingOnSecondScan()
        {
            GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<MeshRenderer>().sharedMaterial =
                ErrorShaderMaterial;
            var scanner = new RendererMaterialScanner(new CheckedMaterialCache());
            scanner.Scan().ToArray(); // first scan; consumes the fixture's findings into the shared cache

            var secondScanFindings = scanner.Scan().ToArray();

            Assert.That(secondScanFindings, Is.Empty);
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void ScannedTwiceWithShaderSwappedToErrorShader_ReturnsFindingOnSecondScan()
        {
            var material = new Material(Shader.Find("Sprites/Default")) { name = "SwappedToErrorShaderMaterial" };
            GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<MeshRenderer>().sharedMaterial = material;
            var scanner = new RendererMaterialScanner(new CheckedMaterialCache());
            Assume.That(scanner.Scan(), Is.Empty); // precondition: the still-healthy material passes the first scan

            material.shader = Shader.Find("Hidden/InternalErrorShader");
            var secondScanFindings = scanner.Scan().ToArray();

            Assert.That(secondScanFindings, Has.Length.EqualTo(1));
            Assert.That(secondScanFindings[0], Does.Contain("SwappedToErrorShaderMaterial"));
        }
    }
}
