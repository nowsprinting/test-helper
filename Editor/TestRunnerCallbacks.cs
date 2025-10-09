// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System.IO;
using TestHelper.Editor.JUnitXml;
using TestHelper.RuntimeInternals;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace TestHelper.Editor
{
    /// <summary>
    /// Output the test results in JUnit XML format in <c>RunFinished</c>.
    /// </summary>
    public class TestRunnerCallbacks : ICallbacks
    {
        [InitializeOnLoadMethod]
        private static void SetupCallbacks()
        {
            var api = ScriptableObject.CreateInstance<TestRunnerApi>();
            api.RegisterCallbacks(new TestRunnerCallbacks());
        }

        /// <inheritdoc />
        public void RunStarted(ITestAdaptor testsToRun)
        {
            GameViewResolutionSwitcher.ParseArgumentsAndSwitchIfNeeded();
        }

        /// <inheritdoc />
        public void RunFinished(ITestResultAdaptor result)
        {
            var path = CommandLineArgs.GetJUnitResultsPath();
            if (path != null)
            {
                JUnitXmlWriter.WriteTo(result, path);
            }

            // Delete temporary copied asset files for running play mode tests on player.
            if (Directory.Exists(TemporaryCopyAssetsForPlayer.ResourcesRoot))
            {
                AssetDatabase.DeleteAsset(TemporaryCopyAssetsForPlayer.ResourcesRoot);
                // Note: delete with .meta file.
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void TestStarted(ITestAdaptor test)
        {
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void TestFinished(ITestResultAdaptor result)
        {
        }
    }
}
