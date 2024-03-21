// Copyright (c) 2023-2024 Koji Hasegawa.
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
        [TearDown]
        public void TearDown()
        {
            CommandLineArgs.CachedCommandLineArgs = Environment.GetCommandLineArgs();
        }

        [Test]
        public void GetScreenshotDirectory_WithArgument_GotSpecifiedDirectory()
        {
            CommandLineArgs.CachedCommandLineArgs = new[] { "-testHelperScreenshotDirectory=Test" };
            var actual = CommandLineArgs.GetScreenshotDirectory();
            Assert.That(actual, Is.EqualTo("Test"));
        }

        [Test]
        public void GetScreenshotDirectory_WithoutArgument_GotDefaultDirectory()
        {
            CommandLineArgs.CachedCommandLineArgs = Array.Empty<string>();
            var actual = CommandLineArgs.GetScreenshotDirectory();
            Assert.That(actual, Is.EqualTo(Path.Combine(Application.persistentDataPath, "TestHelper", "Screenshots")));
        }
    }
}
