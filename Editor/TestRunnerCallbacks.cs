// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

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

        /// <summary>
        /// Not implemented.
        /// </summary>
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
