// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;
using UnityEngine;

namespace TestHelper.Attributes
{
    [TestFixture]
    public class IgnoreWindowModeAttributeTest
    {
        [Test]
        [IgnoreWindowMode("Test for skip run on window-mode")]
        public void AttachToMethod_SkipOnWindowMode()
        {
            Assert.That(Application.isBatchMode, Is.True);
        }
    }
}
