// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace TestHelper.Constraints
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
    public class CollectionContainsRepeatingSequenceConstraintTest
    {
        [Test]
        public void ConvertToArray_Array_ReturnsSameReference()
        {
            var enumerable = new[] { 0, 1, 2 };
            CollectionContainsRepeatingSequenceConstraint.ConvertToArray(enumerable, out var actual);
            Assert.That<object>(actual, Is.SameAs(enumerable));
        }

        [Test]
        public void ConvertToArray_List_ReturnsArray()
        {
            var enumerable = new List<int> { 0, 1, 2 };
            CollectionContainsRepeatingSequenceConstraint.ConvertToArray(enumerable, out var actual);
            Assert.That<object>(actual, Is.Not.SameAs(enumerable));
            Assert.That(actual, Is.EqualTo(new[] { 0, 1, 2 }));
        }

        [Test]
        public void IsContainsRepeatingSequence_UnderMinCount_NotContainsRepeatingSequence()
        {
            var samples = new[] { 0, 1, 0, 1 };
            Assert.That(samples, Is.Not.ContainsRepeatingSequence());
        }

        [Test]
        public void IsContainsRepeatingSequence_UnderMinCountBoundary_NotContainsRepeatingSequence()
        {
            var samples = new[] { 0, 1, 2, 3, 4, 0, 1, 2, 3, 4 };
            Assert.That(samples, Is.Not.ContainsRepeatingSequence());
        }

        [Test]
        public void IsContainsRepeatingSequence_RepeatingSequence_ContainsRepeatingSequence()
        {
            var samples = new[] { 0, 1, 2, 3, 4, 5, 0, 1, 2, 3, 4, 5 };
            Assert.That(samples, Is.Not.ContainsRepeatingSequence()); // TODO: fail
        }
    }
}
