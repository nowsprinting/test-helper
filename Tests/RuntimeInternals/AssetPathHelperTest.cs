// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace TestHelper.RuntimeInternals
{
    [TestFixture]
    [Category("Internal")]
    [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor)]
    // Note: Path resolution using the filesystem (package.json lookup) works only in the editor.
    public class AssetPathHelperTest
    {
        private string _baseDirectory;
        private string _fakePackageRoot;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _baseDirectory = Path.Combine(Application.temporaryCachePath, nameof(AssetPathHelperTest));
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
            // The premise of these tests is that the fake package path can not be resolved by the fallback
            // substring search for "Assets"/"Packages".
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
        public void GetAssetPath_CallerUnderAssets_ReturnsAssetsPath()
        {
            var actual = AssetPathHelper.GetAssetPath("./Foo.txt", "Assets/Tests/Runtime/Caller.cs");

            Assert.That(actual, Is.EqualTo("Assets/Tests/Runtime/Foo.txt"));
        }

        [Test]
        public void GetAssetPath_CallerInPackageOutsideProject_ReturnsPackagesPath()
        {
            var callerFilePath = Path.Combine(_fakePackageRoot, "Tests", "Runtime", "Caller.cs");

            var actual = AssetPathHelper.GetAssetPath("./Foo.txt", callerFilePath);

            Assert.That(actual, Is.EqualTo("Packages/com.example.fakepackage/Tests/Runtime/Foo.txt"));
        }

        [Test]
        public void GetAssetPath_UpstreamPathFromPackageOutsideProject_ReturnsPackagesPath()
        {
            var callerFilePath = Path.Combine(_fakePackageRoot, "Tests", "Runtime", "Caller.cs");

            var actual = AssetPathHelper.GetAssetPath("../../DummyDirectory/../Foo/Bar.txt", callerFilePath);

            Assert.That(actual, Is.EqualTo("Packages/com.example.fakepackage/Foo/Bar.txt"));
        }

        [Test]
        public void GetAssetPath_GlobInDirectorySegmentFromPackageOutsideProject_KeepsWildcards()
        {
            var callerFilePath = Path.Combine(_fakePackageRoot, "Tests", "Runtime", "Caller.cs");

            var actual = AssetPathHelper.GetAssetPath("../*/Scene.unity", callerFilePath);

            Assert.That(actual, Is.EqualTo("Packages/com.example.fakepackage/Tests/*/Scene.unity"));
        }

        [Test]
        public void GetAssetPath_TargetEscapesPackage_OutputLogErrorAndReturnsNull()
        {
            var callerFilePath = Path.Combine(_fakePackageRoot, "Tests", "Runtime", "Caller.cs");

            var actual = AssetPathHelper.GetAssetPath("../../../../Foo.txt", callerFilePath);

            Assert.That(actual, Is.Null);
            LogAssert.Expect(LogType.Error, new Regex("Can not resolve absolute path"));
        }

        [Test]
        public void GetAssetPath_NamelessPackageJsonBetweenCallerAndPackageRoot_ReturnsOuterPackagePath()
        {
            var namelessPackageRoot = CreateFakePackage("com.example.nameless", "com.example.namelesspackage");
            File.WriteAllText(Path.Combine(namelessPackageRoot, "Tests", "package.json"), "{\"foo\":\"bar\"}");
            var callerFilePath = Path.Combine(namelessPackageRoot, "Tests", "Runtime", "Caller.cs");

            var actual = AssetPathHelper.GetAssetPath("./Foo.txt", callerFilePath);

            Assert.That(actual, Is.EqualTo("Packages/com.example.namelesspackage/Tests/Runtime/Foo.txt"));
        }

        [Test]
        public void GetAssetPath_RealCallerFilePath_ReturnsPackagesPath()
        {
            var actual = AssetPathHelper.GetAssetPath("./Foo.txt", GetCallerFilePath());

            Assert.That(actual, Is.EqualTo("Packages/com.nowsprinting.test-helper/Tests/RuntimeInternals/Foo.txt"));
        }
    }
}
