// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

#if ENABLE_INSTANT_REPLAY
using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using TestHelper.RuntimeInternals;

namespace TestHelper.Attributes
{
    [TestFixture]
    public class RecordVideoAttributeTest
    {
        private const string TestScene = "../../Scenes/ScreenshotTest.unity";
        private readonly string _defaultOutputDirectory = CommandLineArgs.GetScreenshotDirectory();

        private static string SubdirectoryFromNamespace =>
            nameof(TestHelper) + Path.DirectorySeparatorChar +
            nameof(Attributes) + Path.DirectorySeparatorChar +
            nameof(RecordVideoAttributeTest);

        [Test, Order(0)]
        [LoadScene(TestScene)]
        [RecordVideo]
        public async Task Attach_WithoutArguments()
        {
            var path = Path.Combine(_defaultOutputDirectory, $"{nameof(Attach_WithoutArguments)}.mp4");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            Assume.That(path, Does.Not.Exist);

            await Task.Delay(TimeSpan.FromSeconds(1D));

            // output video after running this test.
        }

        [Test, Order(1)]
        public void Attach_WithoutArguments_OutputVideoToDefaultPath()
        {
            var path = Path.Combine(_defaultOutputDirectory, $"{nameof(Attach_WithoutArguments)}.mp4");
            Assert.That(path, Does.Exist);
        }
    }
}
#endif
