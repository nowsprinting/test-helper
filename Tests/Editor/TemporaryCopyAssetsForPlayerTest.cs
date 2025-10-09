// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System.IO;
using NUnit.Framework;

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
    }
}
