// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using UnityEngine;

namespace TestHelper.Attributes
{
    /// <summary>
    /// Skip this test run if Unity version is older and/or newer than specified.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public class UnityVersionAttribute : NUnitAttribute, IApplyToTest
    {
        private readonly string _newerThanOrEqual;
        private readonly string _olderThan;

        /// <summary>
        /// Skip this test run if Unity version is older and/or newer than specified.
        /// </summary>
        /// <param name="newerThanOrEqual">This test will run if the Unity editor version is newer than or equal the specified version.</param>
        /// <param name="olderThan">This test will run if the Unity editor version is older than the specified version.</param>
        public UnityVersionAttribute(string newerThanOrEqual = null, string olderThan = null)
        {
            _newerThanOrEqual = newerThanOrEqual;
            _olderThan = olderThan;
        }

        public void ApplyToTest(Test test)
        {
            if (IsSkip(Application.unityVersion))
            {
                test.RunState = RunState.Ignored;
                test.Properties.Set("_SKIPREASON", "Unity editor version is out of range.");
            }
        }

        internal bool IsSkip(string unityVersion)
        {
            if (!string.IsNullOrEmpty(_newerThanOrEqual) && !IsNewerThanEqual(unityVersion, _newerThanOrEqual))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(_olderThan) && !IsOlderThan(unityVersion, _olderThan))
            {
                return true;
            }

            return false;
        }

        private static bool IsNewerThanEqual(string unityVersion, string newerThanEqual)
        {
            return string.Compare(unityVersion, newerThanEqual, StringComparison.Ordinal) >= 0;
        }

        private static bool IsOlderThan(string unityVersion, string olderThan)
        {
            var digits = olderThan.Length; // Evaluate only up to olderThan`s digits
            return string.Compare(unityVersion.Substring(0, digits), olderThan, StringComparison.Ordinal) <= 0;
        }
    }
}
