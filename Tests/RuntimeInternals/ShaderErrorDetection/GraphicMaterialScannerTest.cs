// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Linq;
using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    [TestFixture]
    [Category("Integration")]
    public class GraphicMaterialScannerTest
    {
        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void RenderedGraphicWithErrorShaderMaterial_ReturnsFinding()
        {
            var canvas = new GameObject("Canvas", typeof(Canvas)).GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var image = new GameObject("ImageWithErrorShaderMaterial", typeof(Image)).GetComponent<Image>();
            image.transform.SetParent(canvas.transform, false);
            image.material = new Material(Shader.Find("Hidden/InternalErrorShader")) { name = "BrokenUiMaterial" };

            var findings = new GraphicMaterialScanner(new CheckedMaterialCache()).Scan().ToArray();

            Assert.That(findings, Has.Length.EqualTo(1));
            Assert.That(findings[0], Does.Contain(image.gameObject.name));
            Assert.That(findings[0], Does.Contain("BrokenUiMaterial"));
        }

        [Test]
        [CreateScene]
        public void RenderedGraphicWithSupportedShaderMaterial_ReturnsNoFinding()
        {
            var canvas = new GameObject("Canvas", typeof(Canvas)).GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var image = new GameObject("ImageWithSupportedShaderMaterial", typeof(Image)).GetComponent<Image>();
            image.transform.SetParent(canvas.transform, false);

            var findings = new GraphicMaterialScanner(new CheckedMaterialCache()).Scan().ToArray();

            Assert.That(findings, Is.Empty);
        }

        [Test]
        [CreateScene]
        public void InactiveGraphicWithErrorShaderMaterial_ReturnsNoFinding()
        {
            var canvas = new GameObject("Canvas", typeof(Canvas)).GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var image = new GameObject("InactiveImageWithErrorShaderMaterial", typeof(Image)).GetComponent<Image>();
            image.transform.SetParent(canvas.transform, false);
            image.material = new Material(Shader.Find("Hidden/InternalErrorShader"));
            image.gameObject.SetActive(false);

            var findings = new GraphicMaterialScanner(new CheckedMaterialCache()).Scan().ToArray();

            Assert.That(findings, Is.Empty);
        }
    }
}
