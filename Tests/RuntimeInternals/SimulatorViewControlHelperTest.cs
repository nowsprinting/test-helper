// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine;
using UnityEngine.TestTools;

namespace TestHelper.RuntimeInternals
{
    [TestFixture]
    [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor)]
    [UnityVersion("2022.2")]
    public class SimulatorViewControlHelperTest
    {
        [TestCase(ScreenOrientation.Portrait)]
        [TestCase(ScreenOrientation.PortraitUpsideDown)]
        [TestCase(ScreenOrientation.LandscapeLeft)]
        [TestCase(ScreenOrientation.LandscapeRight)]
        public async Task SetScreenOrientation_RotateScreen(ScreenOrientation orientation)
        {
            SimulatorViewControlHelper.SetScreenOrientation(orientation);
            await UniTask.NextFrame();

            Assert.That(Screen.orientation, Is.EqualTo(orientation));
        }
    }
}
