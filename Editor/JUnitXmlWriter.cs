// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using UnityEditor.TestTools.TestRunner.Api;

namespace TestHelper.Editor
{
    public static class JUnitXmlWriter
    {
        private const string XsltPath = "Packages/com.nowsprinting.test-helper/Editor/nunit3-junit/nunit3-junit.xslt";
        // Note: This XSLT file is copied from https://github.com/nunit/nunit-transforms/tree/master/nunit3-junit

        public static void WriteTo(ITestResultAdaptor result, string path)
        {
            // Input
            var nunit3XmlStream = new MemoryStream();
            var nunit3Writer = XmlWriter.Create(nunit3XmlStream);
            result.ToXml().WriteTo(nunit3Writer);
            var nunit3Xml = new XPathDocument(nunit3XmlStream);

            // Create output directory if it does not exist.
            var directory = Path.GetDirectoryName(path);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Output (JUnit XML)
            var writer = XmlWriter.Create(path);

            // Execute the transformation.
            var transformer = new XslCompiledTransform();
            transformer.Load(XsltPath);
            transformer.Transform(nunit3Xml, writer);
        }
    }
}
