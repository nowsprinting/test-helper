// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using TestHelper.RuntimeInternals;
using UnityEngine;
using UnityEngine.TestTools;

namespace TestHelper.Attributes
{
    [TestFixture]
    [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor)]
    public class GizmosShowOnGameViewAttributeTest
    {
        private class GizmoDemo : MonoBehaviour
        {
            private void OnDrawGizmos()
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position, 0.2f);
            }
        }

        [SetUp]
        public async Task SetUp()
        {
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.AddComponent<GizmoDemo>();
            await Task.Yield();
        }

        [UnityTearDown]
        public IEnumerator UnityTearDown()
        {
            yield return ScreenshotHelper.TakeScreenshot(); // Take screenshot before revert Gizmos.
        }

        [Test]
        [CreateScene(camera: true, light: true)]
        [GizmosShowOnGameView]
        [Description("See the screenshot yourself! Be a witness!!")]
        public void Attach_True_ShowGizmos()
        {
        }

        [Test]
        [CreateScene(camera: true, light: true)]
        [GizmosShowOnGameView(false)]
        [Description("See the screenshot yourself! Be a witness!!")]
        public void Attach_False_HideGizmos()
        {
        }

        [Test]
        [CreateScene(camera: true, light: true)]
        [GizmosShowOnGameView]
        [Description("See the screenshot yourself! Be a witness!!")]
        public async Task AttachToAsyncTest_ShowGizmos()
        {
            await Task.Yield();
        }

        [IgnoreBatchMode(
            "The following error occurred since UTF v1.4.5: UnityTest yielded WaitForEndOfFrame, which is not evoked in batchmode.")]
        [UnityTest]
        [CreateScene(camera: true, light: true)]
        [GizmosShowOnGameView]
        [Description("See the screenshot yourself! Be a witness!!")]
        // Not an async Task test: this test verifies the attribute on a coroutine-style UnityTest method;
        // the async Task variant is covered by the AttachToAsyncTest_ test.
#pragma warning disable UTF4006
        public IEnumerator AttachToUnityTest_ShowGizmos()
#pragma warning restore UTF4006
        {
            yield return null;
        }
    }
}
