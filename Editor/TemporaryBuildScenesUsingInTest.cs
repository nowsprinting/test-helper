// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using TestHelper.Attributes;
using TestHelper.Editor;
using UnityEditor;
using UnityEditor.TestTools;

[assembly: TestPlayerBuildModifier(typeof(TemporaryBuildScenesUsingInTest))]

namespace TestHelper.Editor
{
    /// <summary>
    /// Temporarily build scenes specified by <c>LoadSceneAttribute</c> when running play mode tests on standalone player.
    /// </summary>
    public class TemporaryBuildScenesUsingInTest : ITestPlayerBuildModifier
    {
        private static IEnumerable<LoadSceneAttribute> FindLoadSceneAttributesOnAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var attribute in assemblies
                         .Select(assembly => assembly.GetCustomAttributes(typeof(LoadSceneAttribute), false))
                         .SelectMany(attributes => attributes))
            {
                yield return attribute as LoadSceneAttribute;
            }
        }

        private static IEnumerable<LoadSceneAttribute> FindLoadSceneAttributesOnTypes()
        {
            var symbols = TypeCache.GetTypesWithAttribute<LoadSceneAttribute>();
            foreach (var attribute in symbols
                         .Select(symbol => symbol.GetCustomAttributes(typeof(LoadSceneAttribute), false))
                         .SelectMany(attributes => attributes))
            {
                yield return attribute as LoadSceneAttribute;
            }
        }

        private static IEnumerable<LoadSceneAttribute> FindLoadSceneAttributesOnMethods()
        {
            var symbols = TypeCache.GetMethodsWithAttribute<LoadSceneAttribute>();
            foreach (var attribute in symbols
                         .Select(symbol => symbol.GetCustomAttributes(typeof(LoadSceneAttribute), false))
                         .SelectMany(attributes => attributes))
            {
                yield return attribute as LoadSceneAttribute;
            }
        }

        internal static IEnumerable<string> GetScenesUsingInTest()
        {
            var attributes = FindLoadSceneAttributesOnAssemblies()
                .Concat(FindLoadSceneAttributesOnTypes())
                .Concat(FindLoadSceneAttributesOnMethods());
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
        /// Add temporary scenes to build when running play mode tests on standalone player.
        /// </summary>
        /// <remarks>
        /// Required Unity Test Framework package v1.1.13 or higher is to use this script.
        /// For details, see the <see href="https://forum.unity.com/threads/testplayerbuildmodifier-not-working.844447/">report in forum</see>.
        /// </remarks>
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
