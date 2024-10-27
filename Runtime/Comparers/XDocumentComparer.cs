// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace TestHelper.Comparers
{
    /// <summary>
    /// Compare XML documents.
    ///
    /// It only compares the attributes and values of each element in the document unordered.
    /// XML declarations and comments are ignored.
    /// </summary>
    public class XDocumentComparer : IComparer<XDocument>
    {
        /// <inheritdoc/>
        public int Compare(XDocument x, XDocument y)
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

            // Note: Declaration is not compared.

            var comparisonDictionary = CreateComparisonDictionary(y.Root);
            // Note: key is XPath, value is List<XElement>. Any nodes found are deleted one by one.

            var current = x.Root;
            while (current != null)
            {
                // Find same element in y.
                var foundElement = FindElementAndRemove(current, ref comparisonDictionary);
                if (foundElement == null)
                {
                    return -1; // The element exists only in x.
                }

                current = GetChildOrNextElement(current);
            }

            if (comparisonDictionary.Any())
            {
                return 1; // The element exists only in y.
            }

            return 0;
        }

        internal static Dictionary<string, List<XElement>> CreateComparisonDictionary(XElement root)
        {
            var comparisonDictionary = new Dictionary<string, List<XElement>>();
            var current = root;
            while (current != null)
            {
                var xPath = GetXPath(current);
                if (comparisonDictionary.TryGetValue(xPath, out var elements))
                {
                    elements.Add(current);
                }
                else
                {
                    comparisonDictionary.Add(xPath, new List<XElement> { current });
                }

                current = GetChildOrNextElement(current);
            }

            return comparisonDictionary;
        }

        private static XElement GetChildOrNextElement(XElement element)
        {
            if (element.HasElements)
            {
                return element.Elements().First();
            }

            var nextNode = element.NextNode;
            while (nextNode != null)
            {
                if (nextNode.NodeType == XmlNodeType.Element)
                {
                    return nextNode as XElement;
                }

                nextNode = nextNode.NextNode;
            }

            return null;
        }

        /// <summary>
        /// Find element in comparison dictionary.
        /// The entries found are removed from the dictionary.
        /// </summary>
        /// <returns>XElement if found, Null if not found.</returns>
        private static XElement FindElementAndRemove(XElement target, ref Dictionary<string, List<XElement>> dictionary)
        {
            var xPath = GetXPath(target);
            var elements = dictionary.GetValueOrDefault(xPath);
            if (elements == null)
            {
                return null;
            }

            foreach (var element in elements)
            {
                var compare = Compare(target, element);
                if (compare != 0)
                {
                    continue;
                }

                elements.Remove(element);
                if (elements.Count == 0)
                {
                    dictionary.Remove(xPath);
                }

                return element;
            }

            return null;
        }

        /// <summary>
        /// Compare two elements not recursively.
        /// Do not check child elements.
        /// </summary>
        private static int Compare(XElement x, XElement y)
        {
            if (GetXPath(x) != GetXPath(y))
            {
                return -1;
            }

            if (x.HasAttributes != y.HasAttributes)
            {
                return -1;
            }

            if (x.HasAttributes && y.HasAttributes)
            {
                var compareAttributes = Compare(x.Attributes(), y.Attributes());
                if (compareAttributes != 0)
                {
                    return compareAttributes;
                }
            }

            if (x.HasElements && y.HasElements)
            {
                return 0;
            }

            return Compare(x.Value, y.Value);
        }

        private static string GetXPath(XElement element)
        {
            var path = new List<string>();
            var current = element;
            while (current != null)
            {
                path.Add(current.Name.LocalName);
                current = current.Parent;
            }

            path.Reverse();
            return string.Join("/", path);
        }

        /// <summary>
        /// Compare two attribute collections.
        /// </summary>
        private static int Compare([NotNull] IEnumerable<XAttribute> x, [NotNull] IEnumerable<XAttribute> y)
        {
            var comparisonList = y.ToList();

            foreach (var xAttribute in x)
            {
                var yAttribute = comparisonList.FirstOrDefault(attribute => attribute.Name == xAttribute.Name);
                if (yAttribute == null)
                {
                    return -1;
                }

                if (xAttribute.Value != yAttribute.Value)
                {
                    return -1;
                }

                comparisonList.Remove(yAttribute);
            }

            if (comparisonList.Any())
            {
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Compare two strings.
        /// </summary>
        private static int Compare([NotNull] string x, [NotNull] string y)
        {
            return string.Compare(x.Trim(), y.Trim(), StringComparison.CurrentCulture);
        }
    }
}
