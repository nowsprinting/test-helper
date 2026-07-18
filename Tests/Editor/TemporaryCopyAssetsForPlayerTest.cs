// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System.IO;
using NUnit.Framework;
using TestHelper.RuntimeInternals;
using UnityEngine;

namespace TestHelper.Editor
{
    [TestFixture]
    public class TemporaryCopyAssetsForPlayerTest
    {
        public void CopyAssetFiles_CreateResourcesAndCopySpecifiedAsset()
        {
            const string BasePath =
                "Assets/com.nowsprinting.test-helper/Resources/Packages/com.nowsprinting.test-helper/Tests/Prefabs";

            Assume.That(Path.Combine(BasePath, "Cube.prefab"), Does.Not.Exist);
            Assume.That(Path.Combine(BasePath, "Sphere.prefab"), Does.Not.Exist);
            Assume.That(Path.Combine(BasePath, "Capsule.prefab"), Does.Not.Exist);
            Assume.That(Path.Combine(BasePath, "Cylinder.prefab"), Does.Not.Exist);

            TemporaryCopyAssetsForPlayer.CopyAssetFiles();
            // Note: Once run, it will not revert until the test is finished.

            Assert.That(Path.Combine(BasePath, "Cube.prefab"), Does.Exist);
            Assert.That(Path.Combine(BasePath, "Sphere.prefab"), Does.Exist);
            Assert.That(Path.Combine(BasePath, "Capsule.prefab"), Does.Exist);
            Assert.That(Path.Combine(BasePath, "Cylinder.prefab"), Does.Exist);
        }

        public void WriteAssetRootMappingFile_Invoked_WritesMappingJsonContainingAssetsEntry()
        {
            var mappingFilePath = Path.Combine(TemporaryCopyAssetsForPlayer.ResourcesRoot, "Resources",
                AssetRootMapping.ResourcePath + ".json");
            Assume.That(mappingFilePath, Does.Not.Exist);

            TemporaryCopyAssetsForPlayer.WriteAssetRootMappingFile();
            // Note: Once run, it will not revert until the test is finished.

            Assert.That(mappingFilePath, Does.Exist, "mapping file written");
            var mapping = JsonUtility.FromJson<AssetRootMapping>(File.ReadAllText(mappingFilePath));
            var expectedAssetsRoot = Path.GetFullPath(Application.dataPath).Replace('\\', '/');
            Assert.That(mapping.entries, Has.Some.Matches<AssetRootMapping.Entry>(x =>
                x.physicalRoot == expectedAssetsRoot && x.assetRoot == "Assets"), "contains Assets entry");
        }
    }
}
