// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework.Interfaces;
using UnityEditor.TestTools.TestRunner.Api;

namespace TestHelper.Editor.JUnitXml
{
    /// <summary>
    /// Convert NUnit3 test result to JUnit XML format (legacy) and write it to a file.
    /// </summary>
    /// <remarks>
    /// Not supported "Open Test Reporting format" introduced in JUnit 5.
    /// </remarks>
    /// <seealso href="https://docs.nunit.org/articles/nunit/technical-notes/usage/Test-Result-XML-Format.html"/>
    /// <seealso href="https://github.com/jenkinsci/benchmark-plugin/blob/master/doc/JUnit%20format/JUnit.txt"/>
    public static class JUnitXmlWriter
    {
        public static void WriteTo(ITestResultAdaptor result, string path)
        {
            // Convert NUnit3 XML to JUnit XML.
            var junitDocument = new XDocument { Declaration = new XDeclaration("1.0", "utf-8", null) };
            var junitRoot = Convert(result.ToXml());
            junitDocument.Add(junitRoot);

            // Create output directory if it does not exist.
            var directory = Path.GetDirectoryName(path);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Output JUnit XML to the file.
            using (var writer = XmlWriter.Create(path))
            {
                junitDocument.WriteTo(writer);
                writer.Flush();
            }
        }

        private static XElement Convert(TNode node)
        {
            AbstractJUnitElementConverter converter;
            switch (node.Name)
            {
                case AbstractJUnitElementConverter.NUnitTestRun:
                    converter = new JUnitTestSuitesElementConverter(node);
                    break;
                case AbstractJUnitElementConverter.NUnitTestSuite:
                    converter = new JUnitTestSuiteElementConverter(node);
                    break;
                case AbstractJUnitElementConverter.NUnitTestCase:
                    converter = new JUnitTestCaseElementConverter(node);
                    break;
                default:
                    throw new ArgumentException($"Unsupported node name: {node.Name}");
            }

            var junitElement = converter.ToJUnitElement();

            // Recursively convert child nodes.
            node.ChildNodes.ForEach(child =>
            {
                if (child.Name == AbstractJUnitElementConverter.NUnitTestSuite ||
                    child.Name == AbstractJUnitElementConverter.NUnitTestCase)
                {
                    junitElement.Add(Convert(child));
                }
            });

            return junitElement;
        }
    }
}
