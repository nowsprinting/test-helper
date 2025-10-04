// Copyright (c) 2023-2025 Koji Hasegawa.
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
        /// Valid format, e.g., "2023.2.16f1", "2023.2", and "2023".
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
            var v1 = ParseUnityVersion(unityVersion);
            var v2 = ParseUnityVersion(newerThanEqual);
            for (var i = 0; i < v2.Length; i++) // Compare only up to the length of specified version
            {
                if (v1[i] > v2[i]) return true;
                if (v1[i] < v2[i]) return false;
            }

            return true;
        }

        private static bool IsOlderThan(string unityVersion, string olderThan)
        {
            var v1 = ParseUnityVersion(unityVersion);
            var v2 = ParseUnityVersion(olderThan);
            for (var i = 0; i < v2.Length; i++) // Compare only up to the length of specified version
            {
                if (v1[i] < v2[i]) return true;
                if (v1[i] > v2[i]) return false;
            }

            return false;
        }

        private static int[] ParseUnityVersion(string version)
        {
            var parts = version.Split(new[] { '.', 'f', 'b', 'p' }, StringSplitOptions.RemoveEmptyEntries);
            var result = new int[parts.Length];
            for (var i = 0; i < parts.Length; i++)
            {
                if (!int.TryParse(parts[i], out result[i]))
                    result[i] = 0;
            }

            return result;
        }
    }
}
