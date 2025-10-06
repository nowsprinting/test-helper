// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

#if ENABLE_INSTANT_REPLAY
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using TestHelper.RuntimeInternals;
using UnityEngine;
using UnityEngine.UI;

namespace TestHelper.Attributes
{
    [TestFixture]
    [GameViewResolution(GameViewResolution.VGA)]
    public class RecordVideoAttributeTest
    {
        private const string TestScene = "../../Scenes/ScreenshotTest.unity";
        private const string SpecifyRelativeDirectory = "Logs/TestHelper.Tests/" + nameof(RecordVideoAttributeTest);
        private readonly string _defaultOutputDirectory = CommandLineArgs.GetScreenshotDirectory();

        private static string SubdirectoryFromNamespace =>
            nameof(TestHelper) + Path.DirectorySeparatorChar +
            nameof(Attributes) + Path.DirectorySeparatorChar +
            nameof(RecordVideoAttributeTest);

        [SetUp]
        public void SetUp()
        {
            var textObject = GameObject.Find("Text");
            Assume.That(textObject, Is.Not.Null);

            var text = textObject.GetComponent<Text>();
            text.text = TestContext.CurrentContext.Test.Name;
        }

        private static async Task RotateCube()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(0, -0.5f, 0);

            var startTime = Time.time;
            while (Time.time - startTime < 1.0f)
            {
                cube.transform.Rotate(new Vector3(0, 90, 0) * Time.deltaTime);
                await Task.Yield();
            }

            Object.Destroy(cube);
        }

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

            await RotateCube();

            // output video after running this test.
        }

        [Test, Order(1)]
        public void Attach_WithoutArguments_OutputVideoToDefaultPath()
        {
            var path = Path.Combine(_defaultOutputDirectory, $"{nameof(Attach_WithoutArguments)}.mp4");
            Assert.That(path, Does.Exist);
        }

        [Test, Order(0)]
        [LoadScene(TestScene)]
        [RecordVideo(baseDirectory: SpecifyRelativeDirectory)]
        public async Task Attach_WithBaseDirectory()
        {
            var path = Path.Combine(
                Path.GetFullPath(SpecifyRelativeDirectory),
                $"{nameof(Attach_WithBaseDirectory)}.mp4");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            Assume.That(path, Does.Not.Exist);

            await RotateCube();

            // output video after running this test.
        }

        [Test, Order(1)]
        public void Attach_WithBaseDirectory_OutputVideoToSpecifiedPath()
        {
            var path = Path.Combine(
                Path.GetFullPath(SpecifyRelativeDirectory),
                $"{nameof(Attach_WithBaseDirectory)}.mp4");
            Assert.That(path, Does.Exist);
        }

        [Test, Order(0)]
        [LoadScene(TestScene)]
        [RecordVideo(namespaceToDirectory: true)]
        public async Task Attach_WithNamespaceToDirectory()
        {
            var path = Path.Combine(_defaultOutputDirectory,
                SubdirectoryFromNamespace,
                $"{nameof(Attach_WithNamespaceToDirectory)}.mp4");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            Assume.That(path, Does.Not.Exist);

            await RotateCube();

            // output video after running this test.
        }

        [Test, Order(1)]
        public void Attach_WithNamespaceToDirectory_OutputVideoToSpecifiedPath()
        {
            var path = Path.Combine(_defaultOutputDirectory,
                SubdirectoryFromNamespace,
                $"{nameof(Attach_WithNamespaceToDirectory)}.mp4");
            Assert.That(path, Does.Exist);
        }

        [Test]
        [LoadScene(TestScene)]
        [RecordVideo(gizmos: true)]
        public async Task Attach_WithGizmos_OutputVideoWithGizmos()
        {
            await RotateCube();
            // output video after running this test.
        }

        [Test]
        [LoadScene(TestScene)]
        [RecordVideo(width: 320, height: 240, fpsHint: 5)]
        public async Task Attach_WithSizeWQVGA_OutputVideoWQVGA()
        {
            await RotateCube();
            // output video after running this test.
        }
    }
}
#endif
