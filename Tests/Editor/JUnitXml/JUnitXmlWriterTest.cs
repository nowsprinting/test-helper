// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System.IO;
using NUnit.Framework;
using TestHelper.Comparers;
using TestHelper.Editor.TestDoubles;
using TestHelper.RuntimeInternals;

namespace TestHelper.Editor.JUnitXml
{
    [TestFixture]
    public class JUnitXmlWriterTest
    {
        private const string TestResourcesPath = "Packages/com.nowsprinting.test-helper/Tests/Editor/TestResources";

        [Test, Order(0)]
        public void WriteTo_CreatedJUnitXmlFormatFile()
        {
            var nunitXmlPath = Path.Combine(TestResourcesPath, "nunit3.xml");
            var result = new FakeTestResultAdaptor(nunitXmlPath);
            var path = TemporaryFileHelper.CreatePath(extension: "xml", namespaceToDirectory: true);
            JUnitXmlWriter.WriteTo(result, path);

            Assume.That(path, Does.Exist);

            var actual = File.ReadAllText(path);
            var expected = File.ReadAllText(Path.Combine(TestResourcesPath, "junit.xml"));
            Assert.That(actual, Is.EqualTo(expected).Using(new XmlComparer()));
        }

        [Test]
        public void WriteTo_ExistFile_OverwriteFile()
        {
            var nunitXmlPath = Path.Combine(TestResourcesPath, "nunit3.xml");
            var result = new FakeTestResultAdaptor(nunitXmlPath);
            var path = TemporaryFileHelper.CreatePath(extension: "xml", namespaceToDirectory: true);

            // Destroy the output destination file.
            File.Copy(nunitXmlPath, path, true);

            JUnitXmlWriter.WriteTo(result, path);

            var actual = File.ReadAllText(path);
            var expected = File.ReadAllText(Path.Combine(TestResourcesPath, "junit.xml"));
            Assert.That(actual, Is.EqualTo(expected).Using(new XmlComparer()));
        }
    }
}
