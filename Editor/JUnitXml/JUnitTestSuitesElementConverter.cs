// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System.Xml.Linq;
using NUnit.Framework.Interfaces;

namespace TestHelper.Editor.JUnitXml
{
    /// <summary>
    /// Converter class of NUnit3 "test-run" element to JUnit "testsuites" element.
    /// </summary>
    internal class JUnitTestSuitesElementConverter : AbstractJUnitElementConverter
    {
        /// <inheritdoc/>
        public JUnitTestSuitesElementConverter(TNode node) : base(node)
        {
        }

        /// <inheritdoc/>
        public override XElement ToJUnitElement()
        {
            var element = new XElement(JUnitElementTestsuites);
            element.Add(new XAttribute(JUnitAttributeName, Name));
            element.Add(new XAttribute(JUnitAttributeDisabled, Disabled));
            element.Add(new XAttribute(JUnitAttributeErrors, Errors));
            element.Add(new XAttribute(JUnitAttributeFailures, Failures));
            element.Add(new XAttribute(JUnitAttributeTests, Tests));
            element.Add(new XAttribute(JUnitAttributeTime, Time));

            return element;
        }
    }
}
