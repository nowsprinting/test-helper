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
    public class IgnoreBatchModeAttributeTest
    {
        [Test]
        [IgnoreBatchMode("Test for skip run on batch-mode")]
        public void Attach_SkipOnBatchMode()
        {
            Assert.That(Application.isBatchMode, Is.False);
        }

        [Test]
        [IgnoreBatchMode("Test for skip run on batch-mode")]
        public async Task AttachToAsyncTest_SkipOnBatchMode()
        {
            await Task.Yield();
            Assert.That(Application.isBatchMode, Is.False);
        }

        [UnityTest]
        [IgnoreBatchMode("Test for skip run on batch-mode")]
        // Not an async Task test: this test verifies the attribute on a coroutine-style UnityTest method;
        // the async Task variant is covered by the AttachToAsyncTest_ test.
#pragma warning disable UTF4006
        public IEnumerator AttachToUnityTest_SkipOnBatchMode()
#pragma warning restore UTF4006
        {
            yield return null;
            Assert.That(Application.isBatchMode, Is.False);
        }
    }
}
