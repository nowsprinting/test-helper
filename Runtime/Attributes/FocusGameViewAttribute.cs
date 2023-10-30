// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using TestHelper.RuntimeInternals;

namespace TestHelper.Attributes
{
    /// <summary>
    /// Focus <c>GameView</c> or <c>SimulatorWindow</c> before run test.
    /// Example usage: Tests that use <c>InputEventTrace</c> of the Input System package (com.unity.inputsystem).
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public class FocusGameViewAttribute : NUnitAttribute, IApplyToContext
    {
        /// <inheritdoc />
        public void ApplyToContext(ITestExecutionContext context)
        {
            GameViewControlHelper.Focus();
        }
    }
}
