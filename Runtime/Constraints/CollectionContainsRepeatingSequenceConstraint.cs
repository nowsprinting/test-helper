// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using NUnit.Framework.Constraints;

namespace TestHelper.Constraints
{
    /// <summary>
    /// An NUnit test constraint class to check if a collection contains a repeating sequence.
    /// </summary>
    /// <example>
    /// <code>
    /// [Test]
    /// public void MyTestMethod()
    /// {
    ///   var actual = Experiment.Run(() => UnityEngine.Random.value, 1048576);
    ///   Assert.That(actual, Is.Not.ContainsRepeatingSequence());
    /// }
    /// </code>
    /// </example>
    public class CollectionContainsRepeatingSequenceConstraint : CollectionItemsEqualConstraint
    {
        /// <inheritdoc />
        public override string DisplayName => "ContainsRepeatingSequence";

        /// <inheritdoc />
        public override string Description
        {
            get => "collection containing repeating sequence";
        }

        private const int MinRepeatingCount = 5; // Minimum number of data to judge as a repeating sequence.
        // TODO: 引数に取るべき？（もしくはmodifier？）

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "CognitiveComplexity")]
        protected override bool Matches(IEnumerable actual)
        {
            ConvertToArray(actual, out var array);

            var compareIndex = MinRepeatingCount;
            while (compareIndex < array.Length / 2)
            {
                var repeating = true;
                for (var i = 0; i < compareIndex; i++)
                {
                    if (!array[i].Equals(array[compareIndex + i]))
                    {
                        repeating = false;
                        break;
                    }
                }

                if (repeating)
                {
                    var sequence = new StringBuilder("{");
                    sequence.Append(array[0]);
                    for (var i = 1; i < compareIndex; i++)
                    {
                        sequence.Append(",");
                        sequence.Append(array[i]);
                    }

                    sequence.Append("}");
                    Console.WriteLine($"Found repeating sequence. period={compareIndex}, sequence={sequence}");
                    return true;
                }

                compareIndex++;
            }

            return false;
        }

        internal static void ConvertToArray(IEnumerable enumerable, out IComparable[] array)
        {
            if (enumerable is Array and IComparable[])
            {
                array = (IComparable[])enumerable;
                return;
            }

            array = enumerable.Cast<IComparable>().ToArray();
        }
    }
}
