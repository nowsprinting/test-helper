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
        // Not an async Task test: this test verifies the attribute on a coroutine-style UnityTest method;
        // the async Task variant is covered by the AttachToAsyncTest_ test.
#pragma warning disable UTF4006
        public IEnumerator AttachToUnityTest_SkipOnWindowMode()
#pragma warning restore UTF4006
        {
            yield return null;
            Assert.That(Application.isBatchMode, Is.True);
        }
    }
}
