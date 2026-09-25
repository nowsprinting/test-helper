// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine;
using UnityEngine.TestTools;

namespace TestHelper.RuntimeInternals.ShaderErrorDetection
{
    [TestFixture]
    public class MaterialScanRunnerTest
    {
        private const string TestScene = "../../Scenes/ScreenshotTest.unity";

        private MaterialScanRunner _runner;

        [TearDown]
        public void TearDown()
        {
            _runner?.StopAndDestroy();
        }

        [UnityTest]
        [Category("Acceptance")]
        public IEnumerator Create_BeforeFirstIntervalElapsed_DoesNotInvokeScanCallback()
        {
            var tickCount = 0;
            _runner = MaterialScanRunner.Create(() => tickCount++, intervalFrames: 3);

            yield return null; // fewer frames than the interval

            Assert.That(tickCount, Is.EqualTo(0));
        }

        [UnityTest]
        [Category("Acceptance")]
        public IEnumerator Create_IntervalFramesIsEveryFrameValue_InvokesScanCallbackEveryFrame(
            [Values(0, 1)] int intervalFrames)
        {
            var tickCount = 0;
            _runner = MaterialScanRunner.Create(() => tickCount++, intervalFrames);

            const int framesToWait = 10;
            for (var i = 0; i < framesToWait; i++)
            {
                yield return null;
            }

            // Assert over a frame window (not an exact frame) because coroutine tick timing relative to
            // this test's own yields is not guaranteed to the frame; see Known Trade-offs #5 in the plan.
            Assert.That(tickCount, Is.GreaterThanOrEqualTo(framesToWait - 1));
        }

        [UnityTest]
        [Category("Acceptance")]
        public IEnumerator Create_IntervalFramesIsThree_InvokesScanCallbackEveryThreeFrames()
        {
            var tickCount = 0;
            _runner = MaterialScanRunner.Create(() => tickCount++, intervalFrames: 3);

            const int framesToWait = 12; // multiple of the interval, so an expected count is well-defined
            for (var i = 0; i < framesToWait; i++)
            {
                yield return null;
            }

            var expectedTicks = framesToWait / 3;
            Assert.That(tickCount, Is.InRange(expectedTicks - 1, expectedTicks));
        }

        [Test]
        public void Create_Called_ScanRunnerObjectIsHiddenAndNotSaved()
        {
            _runner = MaterialScanRunner.Create(() => { }, intervalFrames: 0);

            Assert.That(_runner.gameObject.hideFlags, Is.EqualTo(HideFlags.HideAndDontSave));
        }

        [UnityTest]
        public IEnumerator StopAndDestroy_Called_DoesNotInvokeScanCallback()
        {
            var tickCount = 0;
            _runner = MaterialScanRunner.Create(() => tickCount++, intervalFrames: 0);
            yield return null; // let at least one tick happen, proving the runner is actually ticking
            var tickCountAtStop = tickCount;

            _runner.StopAndDestroy();
            _runner = null; // already destroyed; avoid double StopAndDestroy in TearDown

            yield return null;
            yield return null;
            yield return null;

            Assert.That(tickCount, Is.EqualTo(tickCountAtStop));
        }

        [UnityTest]
        [Category("Integration")]
        [Category("Acceptance")]
        [LoadScene(TestScene)]
        public IEnumerator SceneIsLoaded_ContinuesInvokingScanCallback()
        {
            var tickCount = 0;
            _runner = MaterialScanRunner.Create(() => tickCount++, intervalFrames: 0);
            yield return null;
            var tickCountBeforeSceneLoad = tickCount;

            yield return SceneManagerHelper.LoadSceneAsync(TestScene);
            yield return null;
            yield return null;

            Assert.That(tickCount, Is.GreaterThan(tickCountBeforeSceneLoad));
        }
    }
}
