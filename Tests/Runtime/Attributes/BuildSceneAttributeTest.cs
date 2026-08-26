// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.Threading.Tasks;
// ReSharper disable once RedundantUsingDirective -- required to enable async Task test support on Unity 2022 or older
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using TestHelper.RuntimeInternals;
using UnityEngine;

namespace TestHelper.Attributes
{
    /// <summary>
    /// Tests running on the player, where the scene must be added to "Scenes in Build" first.
    /// </summary>
    /// <seealso cref="LoadSceneAttributeTest"/>
    [TestFixture]
    [BuildScene(TestScene)]
    public class BuildSceneAttributeTest
    {
        private const string TestScene = "../../Scenes/NotInScenesInBuildForBuild.unity";
        private const string ObjectName = "CubeInNotInScenesInBuild";

        [Test]
        public async Task Attach_SceneIntoBuild()
        {
            var cube = GameObject.Find(ObjectName);
            Assume.That(cube, Is.Null, "Not loaded ");

            await SceneManagerHelper.LoadSceneAsync(TestScene); // Can also be loaded by running the player
            cube = GameObject.Find(ObjectName);
            Assume.That(cube, Is.Not.Null);
        }

        private const string InferredObjectName = "CubeInInferredBuildSceneAttribute";
        private readonly string InferredScene = $"./{nameof(BuildSceneAttributeTest)}.unity";

        [BuildScene]
        [Test]
        public async Task Attach_InferredSceneIntoBuild()
        {
            var cube = GameObject.Find(InferredObjectName);
            Assume.That(cube, Is.Null, "Not loaded ");

            await SceneManagerHelper.LoadSceneAsync(InferredScene); // Can also be loaded by running the player
            cube = GameObject.Find(InferredObjectName);
            Assume.That(cube, Is.Not.Null);
        }
    }
}
