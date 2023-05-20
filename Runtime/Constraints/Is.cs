// Copyright (c) 2023 Koji Hasegawa.
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
    }
}
