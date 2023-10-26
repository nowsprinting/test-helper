// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace TestHelper.Attributes
{
    [TestFixture]
    [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor)]
    public class GameViewResolutionAttributeTest
    {
        [Test, Order(0)]
        [GameViewResolution(1920, 1080, "Full HD")]
        public void Attach_SetScreenSizeToFullHD()
        {
            Assert.That(Screen.width, Is.EqualTo(1920));
            Assert.That(Screen.height, Is.EqualTo(1080));
        }

        [Test, Order(0)]
        [GameViewResolution(GameViewResolution.XGA)]
        public async Task AttachToAsyncTest_SetScreenSizeToFullHD()
        {
            Assert.That(Screen.width, Is.EqualTo(1024));
            Assert.That(Screen.height, Is.EqualTo(768));

            await Task.Yield(); // Not require awaiting before test
        }

        [UnityTest] // Run last
        [GameViewResolution(GameViewResolution.VGA)]
        public IEnumerator AttachToUnityTest_SetScreenSizeToVGA()
        {
            Assert.That(Screen.width, Is.EqualTo(640));
            Assert.That(Screen.height, Is.EqualTo(480));

            yield return null; // Not require awaiting before test
        }
    }
}
