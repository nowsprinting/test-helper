// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

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
        [Test]
        [GameViewResolution(1920, 1080, "Full HD")]
        public async Task Attach_SetScreenSizeToFullHD()
        {
            await Task.Yield(); // Wait to apply change GameView resolution

            Assert.That(Screen.width, Is.EqualTo(1920));
            Assert.That(Screen.height, Is.EqualTo(1080));
        }

        [Test]
        [GameViewResolution(640, 480, "VGA")]
        public async Task Attach_SetScreenSizeToVGA()
        {
            await Task.Yield(); // Wait to apply change GameView resolution

            Assert.That(Screen.width, Is.EqualTo(640));
            Assert.That(Screen.height, Is.EqualTo(480));
        }
    }
}
