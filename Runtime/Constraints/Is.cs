// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

namespace TestHelper.Constraints
{
    /// <inheritdoc />
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Is : UnityEngine.TestTools.Constraints.Is
    {
        /// <summary>
        /// Create constraint to destroyed GameObject.
        /// </summary>
        public static DestroyedConstraint Destroyed => new DestroyedConstraint();

        /// <summary>
        /// Create constraint to check if a collection contains a repeating sequence.
        /// </summary>
        /// <remarks>
        /// I wanted to add it under <c>Does</c>, but it was sealed, so I added it under <c>Is</c>.
        /// </remarks>
        public static CollectionContainsRepeatingSequenceConstraint ContainsRepeatingSequence =>
            new CollectionContainsRepeatingSequenceConstraint();
    }
}
