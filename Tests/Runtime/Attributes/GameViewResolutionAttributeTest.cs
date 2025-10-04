// Copyright (c) 2023-2025 Koji Hasegawa.
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
        public async Task AttachToAsyncTest_SetScreenSizeToFullHD()
        {
            await Task.Yield(); // Wait to apply change GameView resolution

            Assert.That(Screen.width, Is.EqualTo(1920));
            Assert.That(Screen.height, Is.EqualTo(1080));
        }

        [UnityTest, Order(1)]
        [GameViewResolution(GameViewResolution.VGA)]
        public IEnumerator AttachToUnityTest_SetScreenSizeToVGA()
        {
            yield return null; // Wait to apply change GameView resolution

            Assert.That(Screen.width, Is.EqualTo(640));
            Assert.That(Screen.height, Is.EqualTo(480));
        }

        [Test]
        public void Constructor_WithoutName_Defined_ReturnsDefinedNameAndSize()
        {
            var actual = new GameViewResolutionAttribute(400, 240);
            Assert.That(actual._name, Is.EqualTo("WQVGA (400x240)"));
        }

        [Test]
        public void Constructor_WithoutName_NotDefined_ReturnsSizeOnly()
        {
            var actual = new GameViewResolutionAttribute(23, 57);
            Assert.That(actual._name, Is.EqualTo("23x57"));
        }
    }
}
