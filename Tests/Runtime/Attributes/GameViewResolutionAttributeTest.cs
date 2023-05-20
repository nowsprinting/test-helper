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
        private const uint Width = 1920;
        private const uint Height = 1080;

        [Test]
        [GameViewResolution(Width, Height, "Full HD")]
        public async Task Attach_SetScreenSize()
        {
            await Task.Yield(); // Wait to apply change GameView resolution

            Assert.That(Screen.width, Is.EqualTo(Width));
            Assert.That(Screen.height, Is.EqualTo(Height));
        }
    }
}
