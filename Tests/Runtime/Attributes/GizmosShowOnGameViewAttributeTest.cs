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

        [UnitySetUp]
        public IEnumerator UnitySetUp()
        {
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.AddComponent<GizmoDemo>();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator UnityTearDown()
        {
            yield return ScreenshotHelper.TakeScreenshot(); // Take screenshot before revert Gizmos.
        }

        [Test]
        [CreateScene(camera: true, light: true)]
        [GizmosShowOnGameView]
        public void Attach_True_ShowGizmos()
        {
            // See the screenshot yourself! Be a witness!!
        }

        [Test]
        [CreateScene(camera: true, light: true)]
        [GizmosShowOnGameView(false)]
        public void Attach_False_HideGizmos()
        {
            // See the screenshot yourself! Be a witness!!
        }

        [Test]
        [CreateScene(camera: true, light: true)]
        [GizmosShowOnGameView]
        public async Task AttachToAsyncTest_ShowGizmos()
        {
            await Task.Yield();
            // See the screenshot yourself! Be a witness!!
        }

        [UnityTest]
        [CreateScene(camera: true, light: true)]
        [GizmosShowOnGameView]
        public IEnumerator AttachToUnityTest_ShowGizmos()
        {
            yield return null;
            // See the screenshot yourself! Be a witness!!
        }
    }
}
