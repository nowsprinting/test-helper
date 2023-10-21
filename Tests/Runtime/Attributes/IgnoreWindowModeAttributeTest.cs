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
    public class IgnoreWindowModeAttributeTest
    {
        [Test]
        [IgnoreWindowMode("Test for skip run on window-mode")]
        public void Attach_SkipOnWindowMode()
        {
            Assert.That(Application.isBatchMode, Is.True);
        }

        [Test]
        [IgnoreWindowMode("Test for skip run on window-mode")]
        public async Task AttachToAsyncTest_SkipOnWindowMode()
        {
            await Task.Yield();
            Assert.That(Application.isBatchMode, Is.True);
        }

        [UnityTest]
        [IgnoreWindowMode("Test for skip run on window-mode")]
        public IEnumerator AttachToUnityTest_SkipOnWindowMode()
        {
            yield return null;
            Assert.That(Application.isBatchMode, Is.True);
        }
    }
}
