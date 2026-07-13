// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework;

namespace TestHelper.RuntimeInternals
{
    [TestFixture]
    [Category("Internal")]
    public class AssetRootMappingTest
    {
        [Test]
        public void Resolve_PathUnderPhysicalRoot_ReturnsAssetRootWithRemainder()
        {
            var sut = new AssetRootMapping();
            sut.entries.Add(new AssetRootMapping.Entry
            {
                physicalRoot = "/dev/packages/com.example.fake", assetRoot = "Packages/com.example.fakepackage"
            });

            var actual = sut.Resolve("/dev/packages/com.example.fake/Tests/Foo.txt");

            Assert.That(actual, Is.EqualTo("Packages/com.example.fakepackage/Tests/Foo.txt"));
        }

        [Test]
        public void Resolve_PathNotUnderAnyPhysicalRoot_ReturnsNull()
        {
            var sut = new AssetRootMapping();
            sut.entries.Add(new AssetRootMapping.Entry
            {
                physicalRoot = "/dev/packages/com.example.fake", assetRoot = "Packages/com.example.fakepackage"
            });

            var actual = sut.Resolve("/other/place/Foo.txt");

            Assert.That(actual, Is.Null);
        }

        [Test]
        public void Resolve_PathUnderNestedPhysicalRoots_ReturnsLongestRootMapping()
        {
            var sut = new AssetRootMapping();
            sut.entries.Add(new AssetRootMapping.Entry
            {
                physicalRoot = "/dev/project", assetRoot = "Assets"
            });
            sut.entries.Add(new AssetRootMapping.Entry
            {
                physicalRoot = "/dev/project/packages/com.example.fake",
                assetRoot = "Packages/com.example.fakepackage"
            });

            var actual = sut.Resolve("/dev/project/packages/com.example.fake/Tests/Foo.txt");

            Assert.That(actual, Is.EqualTo("Packages/com.example.fakepackage/Tests/Foo.txt"));
        }

        [Test]
        public void Resolve_PathDifferingOnlyByCase_ReturnsAssetRootWithRemainder()
        {
            var sut = new AssetRootMapping();
            sut.entries.Add(new AssetRootMapping.Entry
            {
                physicalRoot = "C:/Dev/Packages/com.example.fake", assetRoot = "Packages/com.example.fakepackage"
            });

            var actual = sut.Resolve("c:/dev/packages/com.example.fake/Tests/Foo.txt");

            Assert.That(actual, Is.EqualTo("Packages/com.example.fakepackage/Tests/Foo.txt"));
        }

        [Test]
        public void Resolve_PathWithBackslashSeparators_ReturnsAssetRootWithRemainder()
        {
            var sut = new AssetRootMapping();
            sut.entries.Add(new AssetRootMapping.Entry
            {
                physicalRoot = "C:/dev/packages/com.example.fake", assetRoot = "Packages/com.example.fakepackage"
            });

            var actual = sut.Resolve("C:\\dev\\packages\\com.example.fake\\Tests\\Foo.txt");

            Assert.That(actual, Is.EqualTo("Packages/com.example.fakepackage/Tests/Foo.txt"));
        }

        [TestCase("/dev/packages/com.example.fake")] // path equals physicalRoot
        [TestCase("/dev/packages/com.example.fakeextra/Foo.txt")] // continues physicalRoot without a separator
        public void Resolve_PathNotAtSegmentBoundary_ReturnsNull(string absolutePath)
        {
            var sut = new AssetRootMapping();
            sut.entries.Add(new AssetRootMapping.Entry
            {
                physicalRoot = "/dev/packages/com.example.fake", assetRoot = "Packages/com.example.fakepackage"
            });

            var actual = sut.Resolve(absolutePath);

            Assert.That(actual, Is.Null);
        }

        [Test]
        public void Resolve_EmptyEntries_ReturnsNull()
        {
            var sut = new AssetRootMapping();

            var actual = sut.Resolve("/dev/packages/com.example.fake/Tests/Foo.txt");

            Assert.That(actual, Is.Null);
        }

        [Test]
        [Category("Acceptance")]
        public void Resolve_NullPath_ReturnsNull()
        {
            var sut = new AssetRootMapping();
            sut.entries.Add(new AssetRootMapping.Entry
            {
                physicalRoot = "/dev/packages/com.example.fake", assetRoot = "Packages/com.example.fakepackage"
            });

            var actual = sut.Resolve(null);

            Assert.That(actual, Is.Null);
        }

        [Test]
        public void Resolve_NullEntries_ReturnsNull()
        {
            var sut = new AssetRootMapping();
            sut.entries = null;

            var actual = sut.Resolve("/dev/packages/com.example.fake/Tests/Foo.txt");

            Assert.That(actual, Is.Null);
        }

        [TestCase("/a/b/")]
        [TestCase("\\a\\b")]
        [TestCase("/a/b")]
        public void AddEntry_UnnormalizedPhysicalRoot_StoresNormalizedRoot(string physicalRoot)
        {
            var sut = new AssetRootMapping();

            sut.AddEntry(physicalRoot, "Packages/com.example.fakepackage");

            Assert.That(sut.entries, Has.Some.Matches<AssetRootMapping.Entry>(x => x.physicalRoot == "/a/b"));
        }

        [Test]
        public void AddEntry_DuplicatePhysicalRoot_KeepsSingleEntry()
        {
            var sut = new AssetRootMapping();

            sut.AddEntry("/dev/packages/com.example.fake", "Packages/com.example.fakepackage");
            sut.AddEntry("/dev/packages/com.example.fake", "Packages/com.example.fakepackage");

            Assert.That(sut.entries, Has.Count.EqualTo(1));
        }
    }
}
