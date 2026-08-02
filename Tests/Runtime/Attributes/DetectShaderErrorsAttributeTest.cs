// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;
using TestHelper.RuntimeInternals.ShaderErrorDetection;
using UnityEngine;
using UnityEngine.TestTools;

namespace TestHelper.Attributes
{
    [TestFixture]
    public class DetectShaderErrorsAttributeTest
    {
        private const string FallbackWarningMessage =
            "Shader 'MyShader' is not supported on this GPU (fallback to 'Diffuse')";

        private static readonly Regex ShaderNameInExceptionLog = new Regex(".*MyShader.*");

        private static void RunToCompletion(IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
            {
            }
        }

        [Test]
        [DetectShaderErrors]
        [Category("Acceptance")]
        public void Attach_ShaderFallbackWarningLogged_LogsInvalidShaderException()
        {
            Debug.LogWarning(FallbackWarningMessage);

            LogAssert.Expect(LogType.Exception, ShaderNameInExceptionLog);
        }

        [Test]
        [DetectShaderErrors]
        public async Task AttachToAsyncTest_ShaderFallbackWarningLogged_LogsInvalidShaderException()
        {
            Debug.LogWarning(FallbackWarningMessage);
            await Task.Yield();

            LogAssert.Expect(LogType.Exception, ShaderNameInExceptionLog);
        }

        [UnityTest]
        [DetectShaderErrors]
        public IEnumerator AttachToUnityTest_ShaderFallbackWarningLogged_LogsInvalidShaderException()
        {
            Debug.LogWarning(FallbackWarningMessage);
            yield return null;

            LogAssert.Expect(LogType.Exception, ShaderNameInExceptionLog);
        }

        [Test]
        [DetectShaderErrors]
        public void Attach_UnrelatedWarningLogged_DoesNotLogInvalidShaderException()
        {
            Debug.LogWarning("An unrelated warning message");

            LogAssert.Expect(LogType.Warning, "An unrelated warning message"); // consume our own deliberate warning
            LogAssert.NoUnexpectedReceived(); // and confirm nothing else (in particular, no exception log) followed
        }

        [Test, Order(0)]
        [DetectShaderErrors]
        public void AfterRunningTest_DetectShaderErrorsCompletesNormally()
        {
            // Intentionally does nothing shader-related; only exercises a normal Attach/Detach cycle
            // so the following [Order(1)] test can verify the monitor was torn down afterward.
        }

        [Test, Order(1)]
        [Category("Acceptance")]
        public void AfterRunningTest_ShaderFallbackWarningLogged_DoesNotLogInvalidShaderException()
        {
            Debug.LogWarning(FallbackWarningMessage);

            LogAssert.Expect(LogType.Warning, FallbackWarningMessage); // consume our own deliberate warning
            LogAssert.NoUnexpectedReceived(); // and confirm nothing else (in particular, no exception log) followed
        }

        [Test]
        [CreateScene]
        [Category("Acceptance")]
        public void AfterTest_ErrorShaderMaterialPresent_ThrowsInvalidShaderException()
        {
            // Drives the attribute's lifecycle directly (rather than attaching it to this test method),
            // because the exception this scenario produces propagates synchronously out of AfterTest and
            // would otherwise just fail this very test instead of being something we can assert on.
            var attribute = new DetectShaderErrorsAttribute();
            RunToCompletion(attribute.BeforeTest(null));

            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "CubeWithErrorShaderMaterialForAfterTestFinalScan";
            go.GetComponent<MeshRenderer>().sharedMaterial =
                new Material(Shader.Find("Hidden/InternalErrorShader")) { name = "BrokenMaterial" };

            var exception = Assert.Throws<InvalidShaderException>(() => RunToCompletion(attribute.AfterTest(null)));

            Assert.That(exception.Message, Does.Contain(go.name));
            Assert.That(exception.Message, Does.Contain("BrokenMaterial"));
        }

        [UnityTest]
        [CreateScene]
        [DetectShaderErrors]
        [Category("Integration")]
        [Category("Acceptance")]
        public IEnumerator SceneContainsRenderedRendererWithErrorShaderMaterial_LogsInvalidShaderException()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "CubeWithErrorShaderMaterialInScene";
            go.GetComponent<MeshRenderer>().sharedMaterial =
                new Material(Shader.Find("Hidden/InternalErrorShader")) { name = "BrokenMaterial" };

            yield return null; // let the periodic hierarchy scan tick catch this before the test ends
            yield return null;

            LogAssert.Expect(LogType.Exception, new Regex(".*BrokenMaterial.*"));
        }

        [UnityTest]
        [CreateScene]
        [DetectShaderErrors]
        [Category("Integration")]
        [Category("Acceptance")]
        public IEnumerator UnsupportedShaderMaterialIsRendered_LogsInvalidShaderException()
        {
            var shader = Resources.Load<Shader>("UnsupportedShader");
            Assume.That(shader, Is.Not.Null);
            Assume.That(shader.isSupported, Is.False,
                "Fixture shader is expected to be unsupported on this platform; " +
                "skip when it unexpectedly compiles and is supported here.");

            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.GetComponent<MeshRenderer>().sharedMaterial = new Material(shader) { name = "UnsupportedMaterial" };

            yield return null;
            yield return null;

            LogAssert.Expect(LogType.Exception, new Regex(".*" + shader.name.Replace("/", @"\/") + ".*"));
        }

        [UnityTest]
        [CreateScene]
        [DetectShaderErrors]
        [Category("Integration")]
        public IEnumerator SceneContainsNoShaderProblem_DoesNotLogInvalidShaderException()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Sprites/Default"));

            yield return null;
            yield return null;

            LogAssert.NoUnexpectedReceived();
        }
    }
}
