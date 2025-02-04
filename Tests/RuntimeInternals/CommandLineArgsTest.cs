// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace TestHelper.RuntimeInternals
{
    [TestFixture]
    public class CommandLineArgsTest
    {
        [Test]
        public void DictionaryFromCommandLineArgs()
        {
            var args = new[] { "-flag1", "-key1", "value1", "-flag2" };
            var expected = new System.Collections.Generic.Dictionary<string, string>
            {
                { "-flag1", string.Empty }, //
                { "-key1", "value1" }, //
                { "-flag2", string.Empty },
            };
            var actual = CommandLineArgs.DictionaryFromCommandLineArgs(args);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetScreenshotDirectory_WithArgument_GotSpecifiedDirectory()
        {
            var actual = CommandLineArgs.GetScreenshotDirectory(new[] { "-testHelperScreenshotDirectory", "Test" });
            Assert.That(actual, Is.EqualTo("Test"));
        }

        [Test]
        public void GetScreenshotDirectory_WithoutArgument_GotDefaultDirectory()
        {
            var actual = CommandLineArgs.GetScreenshotDirectory(Array.Empty<string>());
            Assert.That(actual, Is.EqualTo(Path.Combine(Application.persistentDataPath, "TestHelper", "Screenshots")));
        }

        [Test]
        public void GetStatisticsDirectory_WithArgument_GotSpecifiedDirectory()
        {
            var actual = CommandLineArgs.GetStatisticsDirectory(new[] { "-testHelperStatisticsDirectory", "Test" });
            Assert.That(actual, Is.EqualTo("Test"));
        }

        [Test]
        public void GetStatisticsDirectory_WithoutArgument_GotDefaultDirectory()
        {
            var actual = CommandLineArgs.GetStatisticsDirectory(Array.Empty<string>());
            Assert.That(actual, Is.EqualTo(Path.Combine(Application.persistentDataPath, "TestHelper", "Statistics")));
        }

        [Test]
        public void GetJUnitResultsPath_WithArgument_GotSpecifiedPath()
        {
            var actual = CommandLineArgs.GetJUnitResultsPath(new[] { "-testHelperJUnitResults", "Test" });
            Assert.That(actual, Is.EqualTo("Test"));
        }

        [Test]
        public void GetJUnitResultsPath_WithoutArgument_ReturnsNull()
        {
            var actual = CommandLineArgs.GetJUnitResultsPath(Array.Empty<string>());
            Assert.That(actual, Is.Null);
        }
    }
}
