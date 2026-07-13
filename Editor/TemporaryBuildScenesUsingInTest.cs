// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestHelper.Attributes;
using TestHelper.Editor;
using TestHelper.RuntimeInternals;
using UnityEditor;
using UnityEditor.TestTools;
using UnityEngine;

[assembly: TestPlayerBuildModifier(typeof(TemporaryBuildScenesUsingInTest))]

namespace TestHelper.Editor
{
    /// <summary>
    /// Temporarily build scenes specified by <c>LoadSceneAttribute</c> when running play mode tests on player.
    /// </summary>
    public class TemporaryBuildScenesUsingInTest : ITestPlayerBuildModifier
    {
        internal static IEnumerable<string> GetScenesUsingInTest()
        {
            var attributes = AttributeFinder.FindOnAssemblies<BuildSceneAttribute>()
                .Concat(AttributeFinder.FindOnTypes<BuildSceneAttribute>())
                .Concat(AttributeFinder.FindOnMethods<BuildSceneAttribute>());
            foreach (var attribute in attributes)
            {
                string scenePath;
                try
                {
                    scenePath = SceneManagerHelper.GetExistScenePath(attribute.ScenePath, attribute.CallerFilePath);
                }
                catch (ArgumentException e)
                {
                    Debug.LogWarning(e.Message);
                    continue;
                }
                catch (FileNotFoundException e)
                {
                    Debug.LogWarning(e.Message);
                    continue;
                }

                yield return scenePath;
            }
        }

        /// <summary>
        /// Temporarily build scenes specified by <c>LoadSceneAttribute</c> when running play mode tests on player.
        /// And temporarily copy asset files specified by <c>LoadAssetAttribute</c> to the Resources folder.
        /// </summary>
        /// <remarks>
        /// Required Unity Test Framework package v1.1.13 or higher is to use this script.
        /// For details, see the <see href="https://forum.unity.com/threads/testplayerbuildmodifier-not-working.844447/">report in forum</see>.
        /// </remarks>
        public BuildPlayerOptions ModifyOptions(BuildPlayerOptions playerOptions)
        {
            // Temporarily copy asset files specified by LoadAssetAttribute to the Resources folder
            TemporaryCopyAssetsForPlayer.CopyAssetFiles();

            // Write the asset-root mapping used by AssetPathHelper to resolve relative paths on the player
            TemporaryCopyAssetsForPlayer.WriteAssetRootMappingFile();

            // Temporarily build scenes specified by LoadSceneAttribute
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
