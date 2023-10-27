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
    public class TimeScaleAttributeTest
    {
        [Test, Order(0)]
        [TimeScale(2.0f)]
        public void Attach_ApplyTimeScale()
        {
            Assert.That(Time.timeScale, Is.EqualTo(2.0f));
        }

        [Test, Order(0)]
        [TimeScale(3.0f)]
        public async Task AttachToAsyncTest_ApplyTimeScale()
        {
            Assert.That(Time.timeScale, Is.EqualTo(3.0f));
            await Task.Yield();
        }

        [UnityTest, Order(0)]
        [TimeScale(4.0f)]
        public IEnumerator AttachToUnityTest_ApplyTimeScale()
        {
            Assert.That(Time.timeScale, Is.EqualTo(4.0f));
            yield return null;
        }

        [Test, Order(1)]
        public void AfterRunningTest_RevertTimeScale()
        {
            Assert.That(Time.timeScale, Is.EqualTo(1.0f));
        }
    }
}
