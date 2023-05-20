// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using UnityEngine;

namespace TestHelper.Attributes
{
    /// <summary>
    /// Skip this test run on batchmode (headless mode)
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public class IgnoreBatchModeAttribute : NUnitAttribute, IApplyToTest
    {
        private readonly string _reason;

        /// <summary>
        /// Skip this test run on batchmode (headless mode)
        /// </summary>
        /// <param name="reason">Reason for skip this test</param>
        public IgnoreBatchModeAttribute(string reason) => this._reason = reason;

        void IApplyToTest.ApplyToTest(Test test)
        {
            if (test.RunState == RunState.NotRunnable)
            {
                return;
            }

            if (!Application.isBatchMode)
            {
                return;
            }

            test.RunState = RunState.Ignored;
            test.Properties.Set("_SKIPREASON", (object)this._reason);
        }
    }
}
