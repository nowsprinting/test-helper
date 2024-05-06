// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System.IO;
using NUnit.Framework;
using TestHelper.Editor.TestDoubles;

namespace TestHelper.Editor
{
    [TestFixture]
    public class JUnitXmlWriterTest
    {
        private const string TestResourcesPath = "Packages/com.nowsprinting.test-helper/Tests/Editor/TestResources";
        private const string TestOutputDirectoryPath = "Logs/JUnitXmlWriterTest";
        // Note: relative path from the project root directory.

        [Test, Order(0)]
        public void WriteTo_DirectoryDoesNotExist_CreateDirectoryAndWriteToFile()
        {
            if (Directory.Exists(TestOutputDirectoryPath))
            {
                Directory.Delete(TestOutputDirectoryPath, true);
            }

            var nunitXmlPath = Path.Combine(TestResourcesPath, "nunit3.xml");
            var result = new FakeTestResultAdaptor(nunitXmlPath);
            var path = Path.Combine(TestOutputDirectoryPath, TestContext.CurrentContext.Test.Name + ".xml");
            JUnitXmlWriter.WriteTo(result, path);

            Assert.That(path, Does.Exist);
        }

        [Test]
        public void WriteTo_WriteToFile()
        {
            var nunitXmlPath = Path.Combine(TestResourcesPath, "nunit3.xml");
            var result = new FakeTestResultAdaptor(nunitXmlPath);
            var path = Path.Combine(TestOutputDirectoryPath, TestContext.CurrentContext.Test.Name + ".xml");
            JUnitXmlWriter.WriteTo(result, path);

            var actual = File.ReadAllText(path);
            var expected = File.ReadAllText(Path.Combine(TestResourcesPath, "junit.xml"));
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
