// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework.Constraints;

namespace TestHelper.Constraints
{
    /// <summary>
    /// Common base class for this package's custom constraints. Hides <see cref="Constraint.And"/>,
    /// <see cref="Constraint.Or"/>, and <see cref="Constraint.With"/> so a chain that starts at one custom
    /// constraint keeps property style for the next one too (e.g., <c>Is.Not.Destroyed.And.WithinScreen</c>),
    /// instead of dropping into <see cref="ConstraintExtensions"/>'s method style.
    /// </summary>
    public abstract class TestHelperConstraint : Constraint
    {
        protected TestHelperConstraint(params object[] args) : base(args)
        {
        }

        /// <summary>
        /// Requires both this constraint and the one that follows to be satisfied.
        /// </summary>
        public new TestHelperConstraintExpression And => AppendOperator(new AndOperator());

        /// <summary>
        /// Requires either this constraint or the one that follows to be satisfied.
        /// </summary>
        public new TestHelperConstraintExpression Or => AppendOperator(new OrOperator());

        /// <summary>
        /// Alias of <see cref="And"/>, matching NUnit's own <see cref="Constraint.With"/>.
        /// </summary>
        public new TestHelperConstraintExpression With => AppendOperator(new AndOperator());

        private TestHelperConstraintExpression AppendOperator(ConstraintOperator op)
        {
            // Same shape as NUnit's own Constraint.And/Or/With: a constraint written on its own (e.g.,
            // `Is.WithinScreen`) has no Builder yet, so start one and push this constraint onto it first.
            var builder = Builder;
            if (builder == null)
            {
                builder = new ConstraintBuilder();
                builder.Append(this);
            }

            builder.Append(op);
            return new TestHelperConstraintExpression(builder);
        }
    }
}
