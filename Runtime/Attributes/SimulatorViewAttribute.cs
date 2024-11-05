// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using TestHelper.RuntimeInternals;
using UnityEngine;

namespace TestHelper.Attributes
{
    /// <summary>
    /// Set <c>SimulatorView</c> orientation before SetUp test.
    /// This attribute works only with Unity 2022.2 or newer. (not support device simulator package)
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public class SimulatorViewAttribute : NUnitAttribute, IApplyToContext
    {
        private readonly ScreenOrientation _orientation;

        /// <summary>
        /// Set <c>SimulatorView</c> orientation before SetUp test.
        /// This attribute works only with Unity 2022.2 or newer. (not support device simulator package)
        /// </summary>
        /// <param name="orientation">Set screen orientation</param>
        public SimulatorViewAttribute(ScreenOrientation orientation = ScreenOrientation.Portrait)
        {
            _orientation = orientation;
        }

        public void ApplyToContext(ITestExecutionContext context)
        {
            SimulatorViewControlHelper.SetScreenOrientation(_orientation);
        }
    }
}
