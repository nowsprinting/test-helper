// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using TestHelper.RuntimeInternals;
using UnityEngine;

namespace TestHelper.Attributes
{
    /// <seealso cref="LoadSceneAttributeTest"/>
    [TestFixture]
    [BuildScene]
    public class BuildSceneAttributeInferredTest
    {
        private const string ObjectName = "CubeInNotInScenesInBuild";

        [Test]
        public async Task Attach_SceneIntoBuild()
        {
            var cube = GameObject.Find(ObjectName);
            Assume.That(cube, Is.Null, "Not loaded ");

            await SceneManagerHelper.LoadSceneAsync($"./{nameof(BuildSceneAttributeInferredTest)}.unity"); // Can also be loaded by running the player
            cube = GameObject.Find(ObjectName);
            Assume.That(cube, Is.Not.Null);
        }
    }
}
