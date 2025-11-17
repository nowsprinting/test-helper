// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework.Constraints;

namespace TestHelper.Constraints
{
    public static class ConstraintExtensions
    {
        /// <summary>
        /// Create constraint to destroyed <see cref="UnityEngine.Object"/>.
        /// When used with operators, use it in method style. e.g., `Is.Not.Destroyed()`
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>constraint to destroyed <see cref="UnityEngine.Object"/></returns>
        public static DestroyedConstraint Destroyed(this ConstraintExpression expression)
        {
            var constraint = new DestroyedConstraint();
            expression.Append(constraint);
            return constraint;
        }
    }
}
