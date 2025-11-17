// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections.Generic;
using System.Xml.Linq;

namespace TestHelper.Comparers
{
    /// <summary>
    /// Compare two <c>string</c> as an XML document.
    /// <br/>
    /// It only compares the attributes and values of each element in the document unordered.
    /// XML declarations and comments are ignored, and white spaces, tabs, and newlines before and after the value are ignored.
    /// </summary>
    /// <remarks>
    /// Internal using <see cref="XDocumentComparer"/>  for comparing <see cref="XDocument"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// [TestFixture]
    ///     public class MyTestClass
    ///     {
    ///         [Test]
    ///         public void MyTestMethod()
    ///         {
    ///             var x = @"<root><child>value1</child><child attribute="attr">value2</child></root>";
    ///             var y = @"<?xml version="1.0" encoding="utf-8"?>
    /// <root>
    ///   <!-- comment -->
    ///   <child attribute="attr">
    ///     value2
    ///   </child>
    ///   <!-- comment -->
    ///   <child>
    ///     value1
    ///   </child>
    /// </root>";
    ///
    ///             Assert.That(x, Is.EqualTo(y).Using(new XmlComparer()));
    ///         }
    ///     }
    /// </code>
    /// </example>
    public class XmlComparer : IComparer<string>
    {
        /// <inheritdoc/>
        public int Compare(string x, string y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            return new XDocumentComparer().Compare(XDocument.Parse(x), XDocument.Parse(y));
        }
    }
}
