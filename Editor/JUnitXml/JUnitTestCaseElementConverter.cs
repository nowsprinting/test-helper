// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System.Xml.Linq;
using NUnit.Framework.Interfaces;

namespace TestHelper.Editor.JUnitXml
{
    /// <summary>
    /// Converter class of NUnit3 "test-case" element to JUnit "testcase" element.
    /// </summary>
    internal class JUnitTestCaseElementConverter : AbstractJUnitElementConverter
    {
        /// <inheritdoc/>
        public JUnitTestCaseElementConverter(TNode node) : base(node)
        {
        }

        /// <inheritdoc/>
        public override XElement ToJUnitElement()
        {
            var element = new XElement(JUnitElementTestcase);
            element.Add(new XAttribute(JUnitAttributeName, Name));
            element.Add(new XAttribute(JUnitAttributeClassname, ClassName));
            element.Add(new XAttribute(JUnitAttributeAssertions, Assertions));
            element.Add(new XAttribute(JUnitAttributeTime, Time));
            element.Add(new XAttribute(JUnitAttributeStatus, Status));

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

            if (IsTestCaseSkipped)
            {
                var skippedNode = new XElement(JUnitAttributeSkipped, Reason);
                element.Add(skippedNode);
            }

            if (Failures > 0)
            {
                var failure = new XElement(JUnitElementFailure);
                failure.Add(new XAttribute(JUnitAttributeMessage, Failure.Item1));
                failure.Add(new XAttribute(JUnitAttributeType, string.Empty));
                element.Add(failure);
            }

            if (string.IsNullOrEmpty(SystemOut))
            {
                element.Add(new XElement(JUnitElementSystemOut, SystemOut));
            }

            return element;
        }
    }
}
