// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
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
            var expected = new Dictionary<string, string>
            {
                { "-flag1", string.Empty }, //
                { "-key1", "value1" },      //
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

        [TestCase("QVGA")]
        [TestCase("\"Full HD\"")]
        public void GetGameViewResolutionName_WithArgument_GotResolutionName(string name)
        {
            var actual = CommandLineArgs.GetGameViewResolutionName(new[] { "-testHelperGameViewResolution", name });
            Assert.That(actual, Is.EqualTo(name));
        }

        [Test]
        public void GetGameViewResolutionName_WithoutArgument_ReturnsNull()
        {
            var actual = CommandLineArgs.GetGameViewResolutionName(Array.Empty<string>());
            Assert.That(actual, Is.Null);
        }

        [Test]
        public void GetGameViewResolution_WithArguments_GotWidthAndHeight()
        {
            var (width, height) = CommandLineArgs.GetGameViewResolutionSize(new[]
            {
                "-testHelperGameViewWidth", "23",
                "-testHelperGameViewHeight", "57"
            });
            Assert.That(width, Is.EqualTo(23));
            Assert.That(height, Is.EqualTo(57));
        }

        [TestCase("23", "")]
        [TestCase("23", "string")]
        [TestCase("", "57")]
        [TestCase("string", "57")]
        public void GetGameViewResolution_InvalidArguments_ReturnsZero(string arg1, string arg2)
        {
            var (width, height) = CommandLineArgs.GetGameViewResolutionSize(new[]
            {
                "-testHelperGameViewWidth", arg1,
                "-testHelperGameViewHeight", arg2
            });
            Assert.That(width, Is.EqualTo(0));
            Assert.That(height, Is.EqualTo(0));
        }

        [TestCase("-testHelperGameViewWidth")]
        [TestCase("-testHelperGameViewHeight")]
        public void GetGameViewResolution_OneArgument_ReturnsZero(string key)
        {
            var (width, height) = CommandLineArgs.GetGameViewResolutionSize(new[] { key, "23" });
            Assert.That(width, Is.EqualTo(0));
            Assert.That(height, Is.EqualTo(0));
        }

        [Test]
        public void GetGameViewResolution_WithoutArgument_ReturnsZero()
        {
            var (width, height) = CommandLineArgs.GetGameViewResolutionSize(Array.Empty<string>());
            Assert.That(width, Is.EqualTo(0));
            Assert.That(height, Is.EqualTo(0));
        }
    }
}
