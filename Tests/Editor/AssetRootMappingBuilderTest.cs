// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using TestHelper.RuntimeInternals;
using UnityEngine;

namespace TestHelper.Editor
{
    [TestFixture]
    [Category("Internal")]
    public class AssetRootMappingBuilderTest
    {
        private string _baseDirectory;
        private string _fakePackageRoot;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _baseDirectory = Path.Combine(Application.temporaryCachePath, nameof(AssetRootMappingBuilderTest));
            _fakePackageRoot = CreateFakePackage("com.example.fake", "com.example.fakepackage");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Directory.Delete(_baseDirectory, true);
        }

        [SetUp]
        public void SetUp()
        {
            // The premise of these tests is that the fake package path is outside the project (it has no
            // "Assets"/"Packages" path segment and is not under Application.dataPath).
            Assume.That(_fakePackageRoot, Does.Not.Contain("Assets"));
            Assume.That(_fakePackageRoot, Does.Not.Contain("Packages"));
        }

        private string CreateFakePackage(string directoryName, string packageName)
        {
            var packageRoot = Path.Combine(_baseDirectory, directoryName);
            Directory.CreateDirectory(Path.Combine(packageRoot, "Tests", "Runtime"));
            File.WriteAllText(Path.Combine(packageRoot, "package.json"), $"{{\"name\":\"{packageName}\"}}");
            return packageRoot;
        }

        private static string GetCallerFilePath([CallerFilePath] string callerFilePath = null)
        {
            return callerFilePath;
        }

        [Test]
        public void CreateFromCallerFilePaths_CallerUnderPackageOutsideProject_AddsPackageEntry()
        {
            var callerFilePath = Path.Combine(_fakePackageRoot, "Tests", "Runtime", "Caller.cs");
            var expectedPhysicalRoot = _fakePackageRoot.Replace('\\', '/');

            var actual = AssetRootMappingBuilder.CreateFromCallerFilePaths(new[] { callerFilePath });

            Assert.That(actual.entries, Has.Some.Matches<AssetRootMapping.Entry>(x =>
                x.physicalRoot == expectedPhysicalRoot && x.assetRoot == "Packages/com.example.fakepackage"));
        }

        [Test]
        public void CreateFromCallerFilePaths_CallerUnderAssets_OmitsPackageEntry()
        {
            var callerFilePath = Path.Combine(Application.dataPath, "Tests", "Runtime", "Caller.cs");

            var actual = AssetRootMappingBuilder.CreateFromCallerFilePaths(new[] { callerFilePath });

            Assert.That(actual.entries, Has.Count.EqualTo(1), "entry count");
            Assert.That(actual.entries[0].assetRoot, Is.EqualTo("Assets"), "assetRoot");
        }

        [Test]
        public void CreateFromCallerFilePaths_CallerWithoutPackageRoot_OmitsEntry()
        {
            var callerDirectory = Path.Combine(_baseDirectory, "NoPackageJson");
            Directory.CreateDirectory(callerDirectory);
            var callerFilePath = Path.Combine(callerDirectory, "Caller.cs");

            var actual = AssetRootMappingBuilder.CreateFromCallerFilePaths(new[] { callerFilePath });

            Assert.That(actual.entries, Has.Count.EqualTo(1), "entry count");
            Assert.That(actual.entries[0].assetRoot, Is.EqualTo("Assets"), "assetRoot");
        }

        [TestCase(null)]
        [TestCase("")]
        public void CreateFromCallerFilePaths_NullOrEmptyPath_SkipsPath(string callerFilePath)
        {
            var actual = AssetRootMappingBuilder.CreateFromCallerFilePaths(new[] { callerFilePath });

            Assert.That(actual.entries, Has.Count.EqualTo(1), "entry count");
            Assert.That(actual.entries[0].assetRoot, Is.EqualTo("Assets"), "assetRoot");
        }

        [Test]
        [Category("Acceptance")]
        public void CreateFromCallerFilePaths_EmptyInput_ContainsOnlyAssetsEntry()
        {
            var expectedAssetsRoot = Path.GetFullPath(Application.dataPath).Replace('\\', '/');

            var actual = AssetRootMappingBuilder.CreateFromCallerFilePaths(Enumerable.Empty<string>());

            Assert.That(actual.entries, Has.Count.EqualTo(1), "entry count");
            Assert.That(actual.entries[0].physicalRoot, Is.EqualTo(expectedAssetsRoot), "physicalRoot");
            Assert.That(actual.entries[0].assetRoot, Is.EqualTo("Assets"), "assetRoot");
        }

        [Test]
        public void CreateFromCallerFilePaths_MultipleCallersInSamePackage_AddsSinglePackageEntry()
        {
            var callerFilePaths = new[]
            {
                Path.Combine(_fakePackageRoot, "Tests", "Runtime", "Caller.cs"),
                Path.Combine(_fakePackageRoot, "Tests", "Runtime", "AnotherCaller.cs")
            };

            var actual = AssetRootMappingBuilder.CreateFromCallerFilePaths(callerFilePaths);

            Assert.That(actual.entries.Count(x => x.assetRoot == "Packages/com.example.fakepackage"),
                Is.EqualTo(1));
        }

        [Test]
        [Category("Acceptance")]
        public void Build_InRealEnvironment_ContainsThisPackageEntryWithSymlinkPhysicalRoot()
        {
            // This test file is at <packageRoot>/Tests/Editor/, so the package root is two directories above
            // the compiler-baked caller file's directory. Deriving the expected value from the baked path
            // instead of a hardcoded path keeps this test valid in both embedded and non-embedded (symlink)
            // package configurations. Path.GetFullPath is required because embedded packages bake a
            // project-root-relative path, which the production walk normalizes against the editor's current
            // directory (the project root); it does not resolve symlinks, so the non-embedded symlink-based
            // root is preserved as-is.
            var expectedPhysicalRoot =
                Path.GetFullPath(Path.Combine(GetCallerFilePath(), "..", "..", "..")).Replace('\\', '/');

            var actual = AssetRootMappingBuilder.Build();

            Assert.That(actual.entries, Has.Some.Matches<AssetRootMapping.Entry>(x =>
                x.physicalRoot == expectedPhysicalRoot && x.assetRoot == "Packages/com.nowsprinting.test-helper"));
        }
    }
}
