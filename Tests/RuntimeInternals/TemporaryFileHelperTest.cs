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
        public void CreateTemporaryFilePath_WithoutBaseDirectory_UseTemporaryCachePath()
        {
            var actual = TemporaryFileHelper.CreateTemporaryFilePath();
            var expected = Path.Combine(
                Application.temporaryCachePath,
                nameof(CreateTemporaryFilePath_WithoutBaseDirectory_UseTemporaryCachePath));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CreateTemporaryFilePath_WithBaseDirectory_UseSpecifyDirectory()
        {
            var baseDirectory = Path.Combine(Application.temporaryCachePath, nameof(TemporaryFileHelperTest));

            var actual = TemporaryFileHelper.CreateTemporaryFilePath(baseDirectory: baseDirectory);
            var expected = Path.Combine(
                baseDirectory,
                nameof(CreateTemporaryFilePath_WithBaseDirectory_UseSpecifyDirectory));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CreateTemporaryFilePath_WithExtension_UseExtension()
        {
            var actual = TemporaryFileHelper.CreateTemporaryFilePath(extension: "txt");
            Assert.That(actual, Does.EndWith(".txt"));
        }

        [Test]
        public void CreateTemporaryFilePath_WithExtensionAndDot_UseExtension()
        {
            var actual = TemporaryFileHelper.CreateTemporaryFilePath(extension: ".png");
            Assert.That(actual, Does.EndWith(".png"));
        }

        [Test]
        public void CreateTemporaryFilePath_WithNamespaceToDirectory_IncludesSubdirectory()
        {
            var actual = TemporaryFileHelper.CreateTemporaryFilePath(namespaceToDirectory: true);
            var expected = Path.Combine(
                Application.temporaryCachePath,
                SubdirectoryFromNamespace,
                nameof(CreateTemporaryFilePath_WithNamespaceToDirectory_IncludesSubdirectory));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CreateTemporaryFilePath_WithCreateDirectory_DirectoryIsCreated()
        {
            var baseDirectory = Path.Combine(Application.temporaryCachePath, SubdirectoryFromNamespace);
            if (Directory.Exists(baseDirectory))
            {
                Directory.Delete(baseDirectory, recursive: true);
            }

            Assume.That(baseDirectory, Does.Not.Exist);

            TemporaryFileHelper.CreateTemporaryFilePath(baseDirectory: baseDirectory, createDirectory: true);
            Assert.That(baseDirectory, Does.Exist.IgnoreFiles);
        }

        [Test]
        public void CreateTemporaryFilePath_WithoutCreateDirectory_DirectoryIsNotCreated()
        {
            var baseDirectory = Path.Combine(Application.temporaryCachePath, SubdirectoryFromNamespace);
            if (Directory.Exists(baseDirectory))
            {
                Directory.Delete(baseDirectory, recursive: true);
            }

            Assume.That(baseDirectory, Does.Not.Exist);

            TemporaryFileHelper.CreateTemporaryFilePath(baseDirectory: baseDirectory, createDirectory: false);
            Assert.That(baseDirectory, Does.Not.Exist);
        }

        [Test]
        public void CreateTemporaryFilePath_WithDeleteIfExists_FileIsDeleted()
        {
            var path = Path.Combine(
                Application.temporaryCachePath,
                nameof(CreateTemporaryFilePath_WithDeleteIfExists_FileIsDeleted));
            File.WriteAllText(path, "test");
            Assume.That(path, Does.Exist);

            TemporaryFileHelper.CreateTemporaryFilePath(deleteIfExists: true);
            Assert.That(path, Does.Not.Exist);
        }

        [Test]
        public void CreateTemporaryFilePath_WithoutDeleteIfExists_FileIsNotDeleted()
        {
            var path = Path.Combine(
                Application.temporaryCachePath,
                nameof(CreateTemporaryFilePath_WithoutDeleteIfExists_FileIsNotDeleted));
            File.WriteAllText(path, "test");
            Assume.That(path, Does.Exist);

            TemporaryFileHelper.CreateTemporaryFilePath(deleteIfExists: false);
            Assert.That(path, Does.Exist);
        }

        [TestCase(2, 3.45f, "string")]
        public void CreateTemporaryFilePath_Parameterized_ReplaceSpecialCharacters(int arg1, float arg2, string arg3)
        {
            var actual = TemporaryFileHelper.CreateTemporaryFilePath();
            var arg2String = arg2.ToString(CultureInfo.InvariantCulture).Replace('.', '-') + "f";
            var expected = Path.Combine(
                Application.temporaryCachePath,
                $"{nameof(CreateTemporaryFilePath_Parameterized_ReplaceSpecialCharacters)}_{arg1}-{arg2String}-{arg3}_");
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
