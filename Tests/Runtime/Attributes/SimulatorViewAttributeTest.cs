// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using TestHelper.RuntimeInternals;
using UnityEngine;
using UnityEngine.TestTools;

namespace TestHelper.Attributes
{
    [TestFixture]
    [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor)]
    [UnityVersion("2022.2")]
    public class SimulatorViewAttributeTest
    {
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            SimulatorViewControlHelper.SetScreenOrientation(ScreenOrientation.Portrait);
        }

        [Test]
        [SimulatorView(ScreenOrientation.PortraitUpsideDown)]
        public async Task Attach_ScreenOrientation_PortraitUpsideDown()
        {
            await UniTask.NextFrame(); // Wait to apply change SimulatorView

            Assert.That(Screen.orientation, Is.EqualTo(ScreenOrientation.PortraitUpsideDown));
        }

        [Test]
        [SimulatorView(ScreenOrientation.LandscapeLeft)]
        public async Task Attach_ScreenOrientation_LandscapeLeft()
        {
            await UniTask.NextFrame(); // Wait to apply change SimulatorView

            Assert.That(Screen.orientation, Is.EqualTo(ScreenOrientation.LandscapeLeft));
        }
    }
}
