// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;
using UnityEngine;

namespace TestHelper.Attributes
{
    [TestFixture]
    public class IgnoreBatchModeAttributeTest
    {
        [Test]
        [IgnoreBatchMode("Test for skip run on batch-mode")]
        public void AttachToMethod_SkipOnBatchMode()
        {
            Assert.That(Application.isBatchMode, Is.False);
        }
    }
}
