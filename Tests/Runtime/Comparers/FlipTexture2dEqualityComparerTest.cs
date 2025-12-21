// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

#if ENABLE_FLIP
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using TestHelper.Attributes;
using TestHelper.RuntimeInternals;
using UnityEngine;
using UnityEngine.TestTools;

namespace TestHelper.Comparers
{
    public class FlipTexture2dEqualityComparerTest
    {
        private readonly string _defaultOutputDirectory = CommandLineArgs.GetScreenshotDirectory();
        private const string ErrorMapFileSuffix = ".diff.png";

        [LoadAsset("../../Images/Texture2dComparerTestExpected.png")]
        private Texture2D _expected;

        [LoadAsset("../../Images/Texture2dComparerTestActual.png")]
        private Texture2D _actual;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            LoadAssetAttribute.LoadAssets(this);
        }

        [Test]
        public void Compare_NotEqualTexture_AssertionFails()
        {
            var comparer = new FlipTexture2dEqualityComparer();

            Assert.That(() => { Assert.That(_actual, Is.EqualTo(_expected).Using(comparer)); },
                Throws.TypeOf<AssertionException>().With.Message.EqualTo(
                    "  Expected: <Texture2dComparerTestExpected (UnityEngine.Texture2D)>\n" +
                    "  But was:  <Texture2dComparerTestActual (UnityEngine.Texture2D)>\n"));
        }

        [Test]
        public void Compare_NotEqualTexture_LogDetails()
        {
            var comparer = new FlipTexture2dEqualityComparer();

            Assert.That(() => { Assert.That(_actual, Is.EqualTo(_expected).Using(comparer)); },
                Throws.TypeOf<AssertionException>());
            LogAssert.Expect(LogType.Log,
                new Regex(@"Mean FLIP error value: 0\.\d+\r?\nExceeds the specified tolerance 0.00001"));
        }

        [Test]
        public void Compare_NotEqualTexture_OutputsErrorMapToDefaultPath()
        {
            var path = Path.Combine(_defaultOutputDirectory, TestContext.CurrentContext.Test.Name + ErrorMapFileSuffix);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var comparer = new FlipTexture2dEqualityComparer();

            Assert.That(() => { Assert.That(_actual, Is.EqualTo(_expected).Using(comparer)); },
                Throws.TypeOf<AssertionException>());
            Assert.That(path, Does.Exist);
        }

        [Test]
        public void Compare_NotEqualTextureSize_AssertionFailsWithLogDetails()
        {
            var actual = new Texture2D(320, 240);

            var comparer = new FlipTexture2dEqualityComparer();

            Assert.That(() => { Assert.That(actual, Is.EqualTo(_expected).Using(comparer)); },
                Throws.TypeOf<AssertionException>());
            LogAssert.Expect(LogType.Log, "Texture sizes are different.\n" +
                                          "  Expected: 640x480\n" +
                                          "  But was:  320x240\n");
        }

        [Test]
        public void Compare_WithCustomDirectory_OutputsErrorMapToSpecifiedDirectory()
        {
            var customDirectory = Path.Combine(_defaultOutputDirectory, "CustomDir");
            var path = Path.Combine(customDirectory, TestContext.CurrentContext.Test.Name + ErrorMapFileSuffix);
            if (Directory.Exists(customDirectory))
            {
                Directory.Delete(customDirectory, recursive: true);
            }

            var comparer = new FlipTexture2dEqualityComparer(errorMapOutputDirectory: customDirectory);

            Assert.That(() => { Assert.That(_actual, Is.EqualTo(_expected).Using(comparer)); },
                Throws.TypeOf<AssertionException>());
            Assert.That(path, Does.Exist);
        }

        [Test]
        public void Compare_WithCustomFilename_OutputsErrorMapWithSpecifiedFilename()
        {
            const string CustomFilename = "CustomErrorMap.png";
            var path = Path.Combine(_defaultOutputDirectory, CustomFilename);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var comparer = new FlipTexture2dEqualityComparer(errorMapOutputFilename: CustomFilename);

            Assert.That(() => { Assert.That(_actual, Is.EqualTo(_expected).Using(comparer)); },
                Throws.TypeOf<AssertionException>());
            Assert.That(path, Does.Exist);
        }

        [Test]
        public void Compare_WithNamespaceToDirectory_OutputsErrorMapToDefaultPath()
        {
            var path = Path.Combine(_defaultOutputDirectory,
                nameof(TestHelper), nameof(Comparers), nameof(FlipTexture2dEqualityComparerTest),
                TestContext.CurrentContext.Test.Name + ErrorMapFileSuffix);
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }

            var comparer = new FlipTexture2dEqualityComparer(namespaceToDirectory: true);

            Assert.That(() => { Assert.That(_actual, Is.EqualTo(_expected).Using(comparer)); },
                Throws.TypeOf<AssertionException>());
            Assert.That(path, Does.Exist);
        }

        [Test]
        public void Compare_NotEqualTextureWithHighTolerance_Success()
        {
            var comparer = new FlipTexture2dEqualityComparer(meanErrorTolerance: 0.2f);

            Assert.That(_actual, Is.EqualTo(_expected).Using(comparer));
        }

        [Test]
        public void Compare_SameTexture_Success()
        {
            var actual = new Texture2D(_expected.width, _expected.height, _expected.format, _expected.mipmapCount > 1);
            actual.SetPixels32(_expected.GetPixels32());
            actual.Apply();

            var comparer = new FlipTexture2dEqualityComparer();
            Assert.That(actual, Is.EqualTo(_expected).Using(comparer));

            Object.Destroy(actual);
        }
    }
}
#endif
