// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System.Globalization;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace TestHelper.RuntimeInternals
{
    [TestFixture]
    public class TemporaryFileHelperTest
    {
        private static string SubdirectoryFromNamespace =>
            nameof(TestHelper) + Path.DirectorySeparatorChar +
            nameof(RuntimeInternals) + Path.DirectorySeparatorChar +
            nameof(TemporaryFileHelperTest);

        [Test]
        public void CreatePath_WithoutBaseDirectory_UseTemporaryCachePath()
        {
            var actual = TemporaryFileHelper.CreatePath();
            var expected = Path.Combine(
                Application.temporaryCachePath,
                nameof(CreatePath_WithoutBaseDirectory_UseTemporaryCachePath));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CreatePath_WithBaseDirectory_UseSpecifyDirectory()
        {
            var baseDirectory = Path.Combine(Application.temporaryCachePath, nameof(TemporaryFileHelperTest));

            var actual = TemporaryFileHelper.CreatePath(baseDirectory: baseDirectory);
            var expected = Path.Combine(
                baseDirectory,
                nameof(CreatePath_WithBaseDirectory_UseSpecifyDirectory));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CreatePath_WithExtension_UseExtension()
        {
            var actual = TemporaryFileHelper.CreatePath(extension: "txt");
            Assert.That(actual, Does.EndWith(".txt"));
        }

        [Test]
        public void CreatePath_WithExtensionAndDot_UseExtension()
        {
            var actual = TemporaryFileHelper.CreatePath(extension: ".png");
            Assert.That(actual, Does.EndWith(".png"));
        }

        [Test]
        public void CreatePath_WithNamespaceToDirectory_IncludesSubdirectory()
        {
            var actual = TemporaryFileHelper.CreatePath(namespaceToDirectory: true);
            var expected = Path.Combine(
                Application.temporaryCachePath,
                SubdirectoryFromNamespace,
                nameof(CreatePath_WithNamespaceToDirectory_IncludesSubdirectory));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CreatePath_WithCreateDirectory_DirectoryIsCreated()
        {
            var baseDirectory = Path.Combine(Application.temporaryCachePath, SubdirectoryFromNamespace);
            if (Directory.Exists(baseDirectory))
            {
                Directory.Delete(baseDirectory, recursive: true);
            }

            Assume.That(baseDirectory, Does.Not.Exist);

            TemporaryFileHelper.CreatePath(baseDirectory: baseDirectory, createDirectory: true);
            Assert.That(baseDirectory, Does.Exist.IgnoreFiles);
        }

        [Test]
        public void CreatePath_WithoutCreateDirectory_DirectoryIsNotCreated()
        {
            var baseDirectory = Path.Combine(Application.temporaryCachePath, SubdirectoryFromNamespace);
            if (Directory.Exists(baseDirectory))
            {
                Directory.Delete(baseDirectory, recursive: true);
            }

            Assume.That(baseDirectory, Does.Not.Exist);

            TemporaryFileHelper.CreatePath(baseDirectory: baseDirectory, createDirectory: false);
            Assert.That(baseDirectory, Does.Not.Exist);
        }

        [Test]
        public void CreatePath_WithDeleteIfExists_FileIsDeleted()
        {
            var path = Path.Combine(
                Application.temporaryCachePath,
                nameof(CreatePath_WithDeleteIfExists_FileIsDeleted));
            File.WriteAllText(path, "test");
            Assume.That(path, Does.Exist);

            TemporaryFileHelper.CreatePath(deleteIfExists: true);
            Assert.That(path, Does.Not.Exist);
        }

        [Test]
        public void CreatePath_WithoutDeleteIfExists_FileIsNotDeleted()
        {
            var path = Path.Combine(
                Application.temporaryCachePath,
                nameof(CreatePath_WithoutDeleteIfExists_FileIsNotDeleted));
            File.WriteAllText(path, "test");
            Assume.That(path, Does.Exist);

            TemporaryFileHelper.CreatePath(deleteIfExists: false);
            Assert.That(path, Does.Exist);
        }

        [TestCase(2, 3.45f, "string")]
        public void CreatePath_Parameterized_ReplaceSpecialCharacters(int arg1, float arg2, string arg3)
        {
            var actual = TemporaryFileHelper.CreatePath();
            var arg2String = arg2.ToString(CultureInfo.InvariantCulture).Replace('.', '-') + "f";
            var expected = Path.Combine(
                Application.temporaryCachePath,
                $"{nameof(CreatePath_Parameterized_ReplaceSpecialCharacters)}_{arg1}-{arg2String}-{arg3}_");
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
