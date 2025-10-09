// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System.Globalization;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace TestHelper.RuntimeInternals
{
    [TestFixture]
    public class PathHelperTest
    {
        private static string SubdirectoryFromNamespace =>
            nameof(TestHelper) + Path.DirectorySeparatorChar +
            nameof(RuntimeInternals) + Path.DirectorySeparatorChar +
            nameof(PathHelperTest);

        [Test]
        public void CreateTemporaryFilePath_UseTemporaryCachePath()
        {
            var actual = PathHelper.CreateTemporaryFilePath();
            var expected = Path.Combine(
                Application.temporaryCachePath,
                nameof(CreateTemporaryFilePath_UseTemporaryCachePath));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CreateTemporaryFilePath_WithExtension_UseExtension()
        {
            var actual = PathHelper.CreateTemporaryFilePath(extension: "txt");
            Assert.That(actual, Does.EndWith(".txt"));
        }

        [Test]
        public void CreateTemporaryFilePath_WithExtensionAndDot_UseExtension()
        {
            var actual = PathHelper.CreateTemporaryFilePath(extension: ".png");
            Assert.That(actual, Does.EndWith(".png"));
        }

        [Test]
        public void CreateTemporaryFilePath_WithNamespaceToDirectory_IncludesSubdirectory()
        {
            var actual = PathHelper.CreateTemporaryFilePath(namespaceToDirectory: true);
            var expected = Path.Combine(
                Application.temporaryCachePath,
                SubdirectoryFromNamespace,
                nameof(CreateTemporaryFilePath_WithNamespaceToDirectory_IncludesSubdirectory));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CreateTemporaryFilePath_WithCreateDirectory_DirectoryIsCreated()
        {
            var directory = Path.Combine(Application.temporaryCachePath, SubdirectoryFromNamespace);
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }

            Assume.That(directory, Does.Not.Exist);

            PathHelper.CreateTemporaryFilePath(namespaceToDirectory: true, createDirectory: true);
            Assert.That(directory, Does.Exist.IgnoreFiles);
        }

        [Test]
        public void CreateTemporaryFilePath_WithoutCreateDirectory_DirectoryIsNotCreated()
        {
            var directory = Path.Combine(Application.temporaryCachePath, SubdirectoryFromNamespace);
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }

            Assume.That(directory, Does.Not.Exist);

            PathHelper.CreateTemporaryFilePath(namespaceToDirectory: true, createDirectory: false);
            Assert.That(directory, Does.Not.Exist);
        }

        [Test]
        public void CreateTemporaryFilePath_WithDeleteIfExists_FileIsDeleted()
        {
            var path = Path.Combine(
                Application.temporaryCachePath,
                nameof(CreateTemporaryFilePath_WithDeleteIfExists_FileIsDeleted));
            File.WriteAllText(path, "test");
            Assume.That(path, Does.Exist);

            PathHelper.CreateTemporaryFilePath(deleteIfExists: true);
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

            PathHelper.CreateTemporaryFilePath(deleteIfExists: false);
            Assert.That(path, Does.Exist);
        }

        [TestCase(2, 3.45f, "string")]
        public void CreateTemporaryFilePath_Parameterized_ReplaceSpecialCharacters(int arg1, float arg2, string arg3)
        {
            var actual = PathHelper.CreateTemporaryFilePath();
            var arg2String = arg2.ToString(CultureInfo.InvariantCulture).Replace('.', '-') + "f";
            var expected = Path.Combine(
                Application.temporaryCachePath,
                $"{nameof(CreateTemporaryFilePath_Parameterized_ReplaceSpecialCharacters)}_{arg1}-{arg2String}-{arg3}_");
            Assert.That(actual, Is.EqualTo(expected));
        }

        #region Internal methods tests

        [Test]
        public void CreateFilePath_WithBaseDirectory_UseSpecifyDirectory()
        {
            var baseDirectory = Path.Combine(Application.temporaryCachePath, nameof(PathHelperTest));

            var actual = PathHelper.CreateFilePath(baseDirectory: baseDirectory);
            var expected = Path.Combine(
                baseDirectory,
                nameof(CreateFilePath_WithBaseDirectory_UseSpecifyDirectory));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CreateFilePath_WithCreateDirectory_BaseDirectoryIsCreated()
        {
            var baseDirectory = Path.Combine(Application.temporaryCachePath, SubdirectoryFromNamespace);
            if (Directory.Exists(baseDirectory))
            {
                Directory.Delete(baseDirectory, recursive: true);
            }

            Assume.That(baseDirectory, Does.Not.Exist);

            PathHelper.CreateFilePath(baseDirectory: baseDirectory, createDirectory: true);
            Assert.That(baseDirectory, Does.Exist.IgnoreFiles);
        }

        [Test]
        public void CreateFilePath_WithoutCreateDirectory_BaseDirectoryIsNotCreated()
        {
            var baseDirectory = Path.Combine(Application.temporaryCachePath, SubdirectoryFromNamespace);
            if (Directory.Exists(baseDirectory))
            {
                Directory.Delete(baseDirectory, recursive: true);
            }

            Assume.That(baseDirectory, Does.Not.Exist);

            PathHelper.CreateFilePath(baseDirectory: baseDirectory, createDirectory: false);
            Assert.That(baseDirectory, Does.Not.Exist);
        }

        #endregion
    }
}
