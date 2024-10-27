// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace TestHelper.Comparers
{
    [TestFixture]
    public class XmlComparerTest
    {
        private const string BaseXml =
            @"<root><child>value1</child><child attribute=""attr"" attribute2=""attr2"">value2</child></root>";

        private static IEnumerable<TestCaseData> s_TestCaseSource()
        {
            // Equal: Same
            yield return new TestCaseData(BaseXml, BaseXml, 0);

            // Equal: Same
            yield return new TestCaseData(null, null, 0);

            // Missing attribute in the left: Different
            yield return new TestCaseData(BaseXml,
                @"<root><child>value1</child><child attribute2=""attr2"">value2</child></root>", -1);

            // Missing attributes in the left: Different
            yield return new TestCaseData(BaseXml,
                @"<root><child>value1</child><child>value2</child></root>", -1);

            // Different attribute value: Different
            yield return new TestCaseData(BaseXml,
                @"<root><child>value1</child><child attribute=""bad attr"" attribute2=""attr2"">value2</child></root>",
                -1);

            // Different value: Different
            yield return new TestCaseData(BaseXml,
                @"<root><child>bad value</child><child attribute=""attr"" attribute2=""attr2"">value2</child></root>",
                -1);
        }

        private static IEnumerable<TestCaseData> s_TestCaseSourceReversible()
        {
            // Different element order: Same
            yield return new TestCaseData(BaseXml,
                @"<root><child attribute=""attr"" attribute2=""attr2"">value2</child><child>value1</child></root>", 0);

            // Different attribute order: Same
            yield return new TestCaseData(BaseXml,
                @"<root><child>value1</child><child attribute2=""attr2"" attribute=""attr"">value2</child></root>", 0);

            // Different comments: Same
            yield return new TestCaseData(BaseXml,
                @"<root><!-- comment --><child>value1</child><child attribute=""attr"" attribute2=""attr2"">value2</child></root>",
                0);

            // Different XML declaration: Same
            yield return new TestCaseData(BaseXml,
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<root><child>value1</child><child attribute=""attr"" attribute2=""attr2"">value2</child></root>", 0);

            // Different white space: Same
            yield return new TestCaseData(BaseXml, @"<root>
<child>
    value1
</child>
<child
        attribute=""attr""
        attribute2=""attr2"">
    value2
</child></root>", 0);

            // One side is null: Different
            yield return new TestCaseData(BaseXml, null, 1);

            // Missing element in the left: Different
            yield return new TestCaseData(BaseXml,
                @"<root><child>value1</child></root>", -1);
        }

        private static IEnumerable<TestCaseData> s_TestCaseSourceReverse()
        {
            return s_TestCaseSourceReversible().Select(x =>
                new TestCaseData(x.Arguments[1], x.Arguments[0], -1 * (int)x.Arguments[2]));
        }

        [TestCaseSource(nameof(s_TestCaseSource))]
        [TestCaseSource(nameof(s_TestCaseSourceReversible))]
        [TestCaseSource(nameof(s_TestCaseSourceReverse))]
        public void Compare_CompareAsXml(string x, string y, int expected)
        {
            var actual = new XmlComparer().Compare(x, y);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void UsingWithEqualTo_CompareAsXml()
        {
            const string XmlString = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
    <!-- comment -->
    <child
            attribute2=""attr2""
            attribute=""attr"">
        value2
    </child>
    <!-- comment -->
    <child>
        value1
    </child>
</root>";
            // with new-line, white-space, XML declaration, comments, and different order

            Assert.That(XmlString, Is.EqualTo(BaseXml).Using(new XmlComparer()));
        }
    }
}
