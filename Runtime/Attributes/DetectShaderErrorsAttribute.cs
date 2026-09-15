// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using TestHelper.RuntimeInternals.ShaderErrorDetection;
using UnityEngine.TestTools;

namespace TestHelper.Attributes
{
    /// <summary>
    /// Detect shader errors (fallback warnings, error shaders, missing materials) while this test is running.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly)]
    public sealed class DetectShaderErrorsAttribute : NUnitAttribute, IOuterUnityTestAction
    {
        private readonly int _scanIntervalFrames;

        private ShaderErrorDetectionSession _session;

        /// <summary>
        /// Detect shader errors while this test is running.
        /// </summary>
        /// <param name="scanIntervalFrames">Material scan interval in frames. 0 or less means every frame (default).</param>
        public DetectShaderErrorsAttribute(int scanIntervalFrames = 0)
        {
            _scanIntervalFrames = scanIntervalFrames;
        }

        /// <inheritdoc/>
        public IEnumerator BeforeTest(ITest test)
        {
            _session = ShaderErrorDetectionSession.CreateDefault(_scanIntervalFrames);
            _session.Start();
            yield break;
        }

        /// <inheritdoc/>
        public IEnumerator AfterTest(ITest test)
        {
            _session.Stop();
            yield break;
        }
    }
}
