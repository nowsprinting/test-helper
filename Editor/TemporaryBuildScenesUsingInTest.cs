// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using TestHelper.Attributes;
using TestHelper.Editor;
using UnityEditor;
using UnityEditor.TestTools;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

[assembly: TestPlayerBuildModifier(typeof(TemporaryBuildScenesUsingInTest.RunOnStandalonePlayer))]

namespace TestHelper.Editor
{
    /// <summary>
    /// Temporarily build scenes specified by <c>ScenesUsingInTestAttribute</c> when running play mode tests.
    /// </summary>
    public static class TemporaryBuildScenesUsingInTest
    {
        private static IEnumerable<ScenesUsingInTestAttribute> FindScenesUsingInTestAttributesOnAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var attribute in assemblies
                         .Select(assembly => assembly.GetCustomAttributes(typeof(ScenesUsingInTestAttribute), false))
                         .SelectMany(attributes => attributes))
            {
                yield return attribute as ScenesUsingInTestAttribute;
            }
        }

        private static IEnumerable<ScenesUsingInTestAttribute> FindScenesUsingInTestAttributesOnTypes()
        {
            var symbols = TypeCache.GetTypesWithAttribute<ScenesUsingInTestAttribute>();
            foreach (var attribute in symbols
                         .Select(symbol => symbol.GetCustomAttributes(typeof(ScenesUsingInTestAttribute), false))
                         .SelectMany(attributes => attributes))
            {
                yield return attribute as ScenesUsingInTestAttribute;
            }
        }

        private static IEnumerable<ScenesUsingInTestAttribute> FindScenesUsingInTestAttributesOnMethods()
        {
            var symbols = TypeCache.GetMethodsWithAttribute<ScenesUsingInTestAttribute>();
            foreach (var attribute in symbols
                         .Select(symbol => symbol.GetCustomAttributes(typeof(ScenesUsingInTestAttribute), false))
                         .SelectMany(attributes => attributes))
            {
                yield return attribute as ScenesUsingInTestAttribute;
            }
        }

        internal static IEnumerable<string> GetScenesUsingInTest()
        {
            var attributes = FindScenesUsingInTestAttributesOnAssemblies()
                .Concat(FindScenesUsingInTestAttributesOnTypes())
                .Concat(FindScenesUsingInTestAttributesOnMethods());
            foreach (var attribute in attributes)
            {
                if (attribute.ScenePath.ToLower().EndsWith(".unity"))
                {
                    yield return attribute.ScenePath;
                }
                else
                {
                    foreach (var guid in AssetDatabase.FindAssets("t:SceneAsset", new[] { attribute.ScenePath }))
                    {
                        yield return AssetDatabase.GUIDToAssetPath(guid);
                    }
                }
            }
        }

        /// <summary>
        /// Add temporary scenes to build when running play mode tests in editor.
        /// </summary>
        public class RunInEditor : ICallbacks
        {
            [InitializeOnLoadMethod]
            private static void SetupRunningInEditor()
            {
                var api = ScriptableObject.CreateInstance<TestRunnerApi>();
                api.RegisterCallbacks(new RunInEditor());
            }

            /// <inheritdoc />
            public void RunStarted(ITestAdaptor testsToRun)
            {
                var scenesInBuild = EditorBuildSettings.scenes.ToList();
                foreach (var scenePath in GetScenesUsingInTest())
                {
                    if (scenesInBuild.All(scene => scene.path != scenePath))
                    {
                        scenesInBuild.Add(new EditorBuildSettingsScene(scenePath, true));
                    }
                }

                EditorBuildSettings.scenes = scenesInBuild.ToArray();
            }

            /// <inheritdoc />
            public void RunFinished(ITestResultAdaptor result) { }

            /// <inheritdoc />
            public void TestStarted(ITestAdaptor test) { }

            /// <inheritdoc />
            public void TestFinished(ITestResultAdaptor result) { }
        }

        /// <summary>
        /// Add temporary scenes to build when running play mode tests on standalone player.
        /// </summary>
        /// <remarks>
        /// Required Unity Test Framework package v1.1.13 or higher is to use this script.
        /// For details, see the <see href="https://forum.unity.com/threads/testplayerbuildmodifier-not-working.844447/">report in forum</see>.
        /// </remarks>
        public class RunOnStandalonePlayer : ITestPlayerBuildModifier
        {
            /// <inheritdoc />
            public BuildPlayerOptions ModifyOptions(BuildPlayerOptions playerOptions)
            {
                var scenesInBuild = new List<string>(playerOptions.scenes);
                foreach (var scenePath in GetScenesUsingInTest())
                {
                    if (!scenesInBuild.Contains(scenePath))
                    {
                        scenesInBuild.Add(scenePath);
                    }
                }

                playerOptions.scenes = scenesInBuild.ToArray();
                return playerOptions;
            }
        }
    }
}
