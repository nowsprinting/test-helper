// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace TestHelper.RuntimeInternals
{
    [TestFixture]
    [Category("Internal")]
    public class ResourcesAssetRootMappingLoaderTest
    {
        [Test]
        [Category("Acceptance")]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor)]
        public void Load_MappingResourceUnavailable_ReturnsNullWithoutThrowing()
        {
            // The premise of this test is that the mapping resource has not been imported. On the player,
            // TemporaryCopyAssetsForPlayer always writes it at build time, so the premise can never hold
            // there (and the player test runner reports the inconclusive result as a failure); restrict
            // this test to the editor. The Assume guards against a leftover fixture imported by the manual
            // positive-path tests below.
            Assume.That(Resources.Load<TextAsset>(AssetRootMapping.ResourcePath), Is.Null);

            var sut = new ResourcesAssetRootMappingLoader();

            var actual = sut.Load();

            Assert.That(actual, Is.Null);
        }

#if UNITY_EDITOR
        // Note: The positive-path tests below require an imported Resources TextAsset fixture that pollutes
        //  Assets/com.nowsprinting.test-helper until the test run is finished (it is deleted by
        //  TestRunnerCallbacks.RunFinished). So they have no Test attribute and are run manually, same as
        //  TemporaryCopyAssetsForPlayerTest.CopyAssetFiles_CreateResourcesAndCopySpecifiedAsset.
        public void Load_MappingResourceAvailable_ReturnsParsedMapping()
        {
            CreateMappingResourceFixture();
            var sut = new ResourcesAssetRootMappingLoader();

            var actual = sut.Load();

            Assert.That(actual.entries, Has.Some.Matches<AssetRootMapping.Entry>(x =>
                x.physicalRoot == "/dev/packages/com.example.fake" &&
                x.assetRoot == "Packages/com.example.fakepackage"));
        }

        public void Load_CalledTwice_ReturnsSameInstance()
        {
            CreateMappingResourceFixture();
            var sut = new ResourcesAssetRootMappingLoader();

            var first = sut.Load();
            var second = sut.Load();

            Assert.That(second, Is.SameAs(first));
        }

        private static void CreateMappingResourceFixture()
        {
            const string FixtureJson = "{\"entries\":[{\"physicalRoot\":\"/dev/packages/com.example.fake\"," +
                                       "\"assetRoot\":\"Packages/com.example.fakepackage\"}]}";
            // Same location as TemporaryCopyAssetsForPlayer.WriteAssetRootMappingFile writes to
            // (TemporaryCopyAssetsForPlayer is in the editor-only assembly, so the root is spelled out here).
            var path = "Assets/com.nowsprinting.test-helper/Resources/" + AssetRootMapping.ResourcePath + ".json";
            // ReSharper disable once AssignNullToNotNullAttribute
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, FixtureJson);
            AssetDatabase.ImportAsset(path);
        }
#endif
    }
}
