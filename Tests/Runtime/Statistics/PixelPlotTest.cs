// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;
using TestHelper.RuntimeInternals;
using TestHelper.Statistics.RandomGenerators;
using UnityEngine;
using Random = System.Random;

namespace TestHelper.Statistics
{
    [TestFixture]
    [SuppressMessage("Assertion", "NUnit2045:Use Assert.Multiple")]
    [SuppressMessage("ReSharper", "RedundantExplicitArrayCreation")]
    public class PixelPlotTest
    {
        [Test]
        public void Constructor_ValueType_Created()
        {
            Assert.That(new PixelPlot<int>(), Is.Not.Null);
        }

        [Test]
        public void Constructor_NotValueType_ThrownArgumentException()
        {
            Assert.That(() => new PixelPlot<string>(), Throws.ArgumentException);
        }

        [Test]
        public void Plot_4Samples_Create2x2Texture()
        {
            var sut = new PixelPlot<int>();
            sut.Plot(new int[] { 1, 2, 3, 4 }, size: 4, min: 0, max: 5);

            Assume.That(sut._pixelPlot, Is.Not.Null);
            Assert.That(sut._pixelPlot.width, Is.EqualTo(2));
            Assert.That(sut._pixelPlot.height, Is.EqualTo(2));
            Assert.That(sut._pixelPlot.GetPixel(0, 0).a, Is.EqualTo(0.2f));
            Assert.That(sut._pixelPlot.GetPixel(0, 1).a, Is.EqualTo(0.4f));
            Assert.That(sut._pixelPlot.GetPixel(1, 0).a, Is.EqualTo(0.6f));
            Assert.That(sut._pixelPlot.GetPixel(1, 1).a, Is.EqualTo(0.8f));
        }

        [Test]
        public void Plot_5Samples_Create3x3Texture()
        {
            var sut = new PixelPlot<int>();
            sut.Plot(new int[] { 1, 2, 3, 4, 5 }, size: 5, min: 0, max: 5);

            Assume.That(sut._pixelPlot, Is.Not.Null);
            Assert.That(sut._pixelPlot.width, Is.EqualTo(3));
            Assert.That(sut._pixelPlot.height, Is.EqualTo(3));
            Assert.That(sut._pixelPlot.GetPixel(0, 0).a, Is.EqualTo(0.2f));
            Assert.That(sut._pixelPlot.GetPixel(0, 1).a, Is.EqualTo(0.4f));
            Assert.That(sut._pixelPlot.GetPixel(0, 2).a, Is.EqualTo(0.6f));
            Assert.That(sut._pixelPlot.GetPixel(1, 0).a, Is.EqualTo(0.8f));
            Assert.That(sut._pixelPlot.GetPixel(1, 1).a, Is.EqualTo(1.0f));
            Assert.That(sut._pixelPlot.GetPixel(1, 2).a, Is.EqualTo(0.0f)); // no sample
            Assert.That(sut._pixelPlot.GetPixel(2, 0).a, Is.EqualTo(0.0f)); // no sample
            Assert.That(sut._pixelPlot.GetPixel(2, 1).a, Is.EqualTo(0.0f)); // no sample
            Assert.That(sut._pixelPlot.GetPixel(2, 2).a, Is.EqualTo(0.0f)); // no sample
        }

        [Test]
        public void Plot_DoubleSamplesIncludeMinusValue_CreateTexture()
        {
            var sut = new PixelPlot<double>();
            sut.Plot(new double[] { -10d, -5d, 0d, 10d }, size: 4, min: -10d, max: 10d);

            Assume.That(sut._pixelPlot, Is.Not.Null);
            Assume.That(sut._pixelPlot.width, Is.EqualTo(2));
            Assume.That(sut._pixelPlot.height, Is.EqualTo(2));
            Assert.That(sut._pixelPlot.GetPixel(0, 0).a, Is.EqualTo(0.0f).Within(0.01f));
            Assert.That(sut._pixelPlot.GetPixel(0, 1).a, Is.EqualTo(0.25f).Within(0.01f));
            Assert.That(sut._pixelPlot.GetPixel(1, 0).a, Is.EqualTo(0.5f).Within(0.01f));
            Assert.That(sut._pixelPlot.GetPixel(1, 1).a, Is.EqualTo(1.0f).Within(0.01f));
        }

        [Test]
        public void Plot_LessThanMinValue_Alpha0()
        {
            var sut = new PixelPlot<int>();
            sut.Plot(new int[] { -1 }, size: 1, min: 0, max: 5);

            Assume.That(sut._pixelPlot, Is.Not.Null);
            Assume.That(sut._pixelPlot.width, Is.EqualTo(1));
            Assume.That(sut._pixelPlot.height, Is.EqualTo(1));
            Assert.That(sut._pixelPlot.GetPixel(0, 0).a, Is.EqualTo(0.0f));
        }

        [Test]
        public void Plot_GreaterThanMaxValue_Alpha1()
        {
            var sut = new PixelPlot<int>();
            sut.Plot(new int[] { 10 }, size: 1, min: 0, max: 5);

            Assume.That(sut._pixelPlot, Is.Not.Null);
            Assume.That(sut._pixelPlot.width, Is.EqualTo(1));
            Assume.That(sut._pixelPlot.height, Is.EqualTo(1));
            Assert.That(sut._pixelPlot.GetPixel(0, 0).a, Is.EqualTo(1.0f));
        }

        [Test]
        public void WriteToFile_NoArguments_WrittenToDefaultFilePath()
        {
            var path = TemporaryFileHelper.CreatePath(
                baseDirectory: CommandLineArgs.GetStatisticsDirectory(),
                extension: "png");

            var sut = new PixelPlot<int>();
            sut.Plot(new int[] { 0, 1, 2, 3 }, size: 4, min: 0, max: 3);
            sut.WriteToFile();

            Assert.That(new FileInfo(path), Does.Exist);
        }

        [Test]
        public void WriteToFile_WithDirectory_WrittenToSpecifiedPath()
        {
            var path = TemporaryFileHelper.CreatePath(extension: "png");

            var sut = new PixelPlot<int>();
            sut.Plot(new int[] { 0, 1, 2, 3 }, size: 4, min: 0, max: 3);
            sut.WriteToFile(directory: Path.GetDirectoryName(path));

            Assert.That(new FileInfo(path), Does.Exist);
        }

        [Test]
        public void WriteToFile_WithFilename_WrittenToSpecifiedPath()
        {
            var filename = $"{TestContext.CurrentContext.Test.Name}-specified.png";
            var path = Path.Combine(CommandLineArgs.GetStatisticsDirectory(), filename);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var sut = new PixelPlot<int>();
            sut.Plot(new int[] { 0, 1, 2, 3 }, size: 4, min: 0, max: 3);
            sut.WriteToFile(filename: filename);

            Assert.That(new FileInfo(path), Does.Exist);
        }

        [Test]
        public void WriteToFile_WithDirectoryAndFilename_WrittenToSpecifiedPath()
        {
            var directory = Application.temporaryCachePath;
            var filename = $"{TestContext.CurrentContext.Test.Name}-specified.png";
            var path = Path.Combine(directory, filename);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var sut = new PixelPlot<int>();
            sut.Plot(new int[] { 0, 1, 2, 3 }, size: 4, min: 0, max: 3);
            sut.WriteToFile(directory: directory, filename: filename);

            Assert.That(new FileInfo(path), Does.Exist);
        }

        [TestFixture]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public class PRNGsSampling
        {
            [Test]
            public void SystemRandom()
            {
                var random = new Random();

                var sampleSpace = Experiment.Run(
                    () => random.Next(),
                    1 << 16); // 256x256

                var pixelPlot = new PixelPlot<int>();
                pixelPlot.Plot(sampleSpace);
                pixelPlot.WriteToFile();
            }

            [Test]
            public void UnityEngineRandom()
            {
                var sampleSpace = Experiment.Run(
                    () => UnityEngine.Random.value,
                    1 << 16); // 256x256

                var pixelPlot = new PixelPlot<float>();
                pixelPlot.Plot(sampleSpace);
                pixelPlot.WriteToFile();
            }

            [Test]
            public void Lcg()
            {
                var lcg = new Lcg(Environment.TickCount);

                var sampleSpace = Experiment.Run(
                    () => lcg.Next(),
                    1 << 16); // 256x256

                var pixelPlot = new PixelPlot<int>();
                pixelPlot.Plot(sampleSpace);
                pixelPlot.WriteToFile();
            }

            [Test]
            public void Lcg_ShortPeriod()
            {
                var lcg = new Lcg(Environment.TickCount, 75, 74, 1 << 8);

                var sampleSpace = Experiment.Run(
                    () => lcg.Next(),
                    1 << 16); // 256x256

                var pixelPlot = new PixelPlot<int>();
                pixelPlot.Plot(sampleSpace);
                pixelPlot.WriteToFile();
            }

            [Test]
            public void Xorshift()
            {
                var xorshift = new Xorshift((uint)Environment.TickCount);

                var sampleSpace = Experiment.Run(
                    () => xorshift.Next(),
                    1 << 16); // 256x256

                var pixelPlot = new PixelPlot<uint>();
                pixelPlot.Plot(sampleSpace);
                pixelPlot.WriteToFile();
            }
        }
    }
}
