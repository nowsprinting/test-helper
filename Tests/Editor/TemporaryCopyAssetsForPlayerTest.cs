// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;

namespace TestHelper.Editor
{
    [TestFixture]
    public class TemporaryCopyAssetsForPlayerTest
    {
        [Test]
        public void CopyAssetFiles_CreateResourcesAndCopySpecifiedAsset()
        {
            const string CopiedAssetPath =
                "Assets/com.nowsprinting.test-helper/Resources/Packages/com.nowsprinting.test-helper/Tests/Prefabs/Cube.prefab";
            Assume.That(CopiedAssetPath, Does.Not.Exist);

            TemporaryCopyAssetsForPlayer.CopyAssetFiles();

            Assert.That(CopiedAssetPath, Does.Exist);
        }
    }
}
