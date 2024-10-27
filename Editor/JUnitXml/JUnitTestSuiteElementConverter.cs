// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System.Xml.Linq;
using NUnit.Framework.Interfaces;

namespace TestHelper.Editor.JUnitXml
{
    /// <summary>
    /// Converter class of NUnit3 "test-suite" element to JUnit "testsuite" element.
    /// </summary>
    internal class JUnitTestSuiteElementConverter : AbstractJUnitElementConverter
    {
        /// <inheritdoc/>
        public JUnitTestSuiteElementConverter(TNode node) : base(node)
        {
        }

        /// <inheritdoc/>
        public override XElement ToJUnitElement()
        {
            var element = new XElement(JUnitElementTestsuite);
            element.Add(new XAttribute(JUnitAttributeName, Name));
            element.Add(new XAttribute(JUnitAttributeTests, Tests));
            element.Add(new XAttribute(JUnitAttributeID, ID));
            element.Add(new XAttribute(JUnitAttributeDisabled, Disabled));
            element.Add(new XAttribute(JUnitAttributeErrors, Errors));
            element.Add(new XAttribute(JUnitAttributeFailures, Failures));
            element.Add(new XAttribute(JUnitAttributeSkipped, Skipped));
            element.Add(new XAttribute(JUnitAttributeTime, Time));
            element.Add(new XAttribute(JUnitAttributeTimestamp, Timestamp));

            if (Properties.Count > 0)
            {
                var propertiesElement = new XElement(JUnitElementProperties);
                foreach (var property in Properties)
                {
                    var propertyElement = new XElement(JUnitElementProperty);
                    propertyElement.Add(new XAttribute(JUnitAttributeName, property.Item1));
                    propertyElement.Add(new XAttribute(JUnitAttributeValue, property.Item2));
                    propertiesElement.Add(propertyElement);
                }

                element.Add(propertiesElement);
            }

            if (string.IsNullOrEmpty(SystemOut))
            {
                element.Add(new XElement(JUnitElementSystemOut, SystemOut));
            }

            return element;
        }
    }
}
