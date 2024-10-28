// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.TestTools;

namespace TestHelper.Attributes
{
    /// <summary>
    /// Change the <c>Time.timeScale</c> during this test running.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TimeScaleAttribute : NUnitAttribute, IOuterUnityTestAction
    {
        private readonly float _beforeTimeScale;
        private readonly float _timeScale;

        /// <summary>
        /// Change the <c>Time.timeScale</c> during this test running.
        /// </summary>
        /// <param name="timeScale">The scale at which time passes.</param>
        public TimeScaleAttribute(float timeScale)
        {
            _beforeTimeScale = Time.timeScale;
            _timeScale = timeScale;
        }

        /// <inheritdoc />
        public IEnumerator BeforeTest(ITest test)
        {
            Time.timeScale = _timeScale;
            yield return null;
        }

        /// <inheritdoc />
        public IEnumerator AfterTest(ITest test)
        {
            Time.timeScale = _beforeTimeScale;
            yield return null;
        }
    }
}
