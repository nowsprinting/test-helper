// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System.Xml.Linq;
using NUnit.Framework;

namespace TestHelper.Comparers
{
    /// <summary>
    /// Test only using constraint with <c>Using</c> modifier in this class.
    /// Other test cases are implemented in <see cref="XmlComparerTest"/>.
    /// </summary>
    [TestFixture]
    public class XDocumentComparerTest
    {
        [Test]
        public void UsingWithEqualTo_Compare()
        {
            var x = XDocument.Parse(@"<root><child>value1</child><child attribute=""attr"">value2</child></root>");
            var y = XDocument.Parse(@"<?xml version=""1.0"" encoding=""utf-8""?>
<root><!-- comment --><child attribute=""attr"">value2</child><!-- comment --><child>value1</child></root>");
            // with XML declaration, comments, and different order

            Assert.That(x, Is.EqualTo(y).Using(new XDocumentComparer()));
        }

        [Test]
        public void CreateComparisonDictionary()
        {
            const string Xml = @"<root><child/><child/><child><grandchild/></child></root>";

            var sut = XDocument.Parse(Xml);
            var actual = XDocumentComparer.CreateComparisonDictionary(sut.Root);

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Count, Is.EqualTo(3));
            Assert.That(actual.ContainsKey("root"), Is.True);
            Assert.That(actual.ContainsKey("root/child"), Is.True);
            Assert.That(actual.ContainsKey("root/child/grandchild"), Is.True);
            Assert.That(actual["root/child"].Count, Is.EqualTo(3));
            Assert.That(actual["root/child/grandchild"].Count, Is.EqualTo(1));
        }
    }
}
