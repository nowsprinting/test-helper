// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework.Constraints;

namespace TestHelper.Constraints
{
    public static class ConstraintExtensions
    {
        /// <summary>
        /// Create constraint to destroyed <c>GameObject</c>.
        /// When used with operators, use it in method style. e.g., `Is.Not.Destroyed()`
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>constraint to destroyed <c>GameObject</c></returns>
        public static DestroyedConstraint Destroyed(this ConstraintExpression expression)
        {
            var constraint = new DestroyedConstraint();
            expression.Append(constraint);
            return constraint;
        }
    }
}
