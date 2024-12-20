// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using TestHelper.Editor.JUnitXml;
using TestHelper.RuntimeInternals;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace TestHelper.Editor
{
    public class TestRunnerCallbacks : ICallbacks
    {
        [InitializeOnLoadMethod]
        private static void SetupCallbacks()
        {
            var api = ScriptableObject.CreateInstance<TestRunnerApi>();
            api.RegisterCallbacks(new TestRunnerCallbacks());
        }

        public void RunStarted(ITestAdaptor testsToRun)
        {
        }

        public void RunFinished(ITestResultAdaptor result)
        {
            var path = CommandLineArgs.GetJUnitResultsPath();
            if (path != null)
            {
                JUnitXmlWriter.WriteTo(result, path);
            }
        }

        public void TestStarted(ITestAdaptor test)
        {
        }

        public void TestFinished(ITestResultAdaptor result)
        {
        }
    }
}
