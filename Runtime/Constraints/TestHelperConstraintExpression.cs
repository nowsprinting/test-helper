// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework.Constraints;

namespace TestHelper.Constraints
{
    /// <summary>
    /// A <see cref="ConstraintExpression"/> that exposes this package's custom constraints as properties, so
    /// they can be written in property style after an operator (e.g., <c>Is.Not.Destroyed</c>) instead of the
    /// method style required by <see cref="ConstraintExtensions"/> (e.g., <c>Is.Not.Destroyed()</c>).
    /// </summary>
    public class TestHelperConstraintExpression : ConstraintExpression
    {
        // Note: kept alongside the base class's own (non-public) builder field instead of reading it back,
        // since relying on that field's existence/accessibility would couple this class to an NUnit
        // implementation detail rather than its public contract.
        private readonly ConstraintBuilder _builder;

        public TestHelperConstraintExpression()
            : this(new ConstraintBuilder())
        {
        }

        public TestHelperConstraintExpression(ConstraintBuilder builder) : base(builder)
        {
            _builder = builder;
        }

        /// <summary>
        /// Negates the constraint that follows.
        /// </summary>
        public new TestHelperConstraintExpression Not
        {
            get
            {
                _builder.Append(new NotOperator());
                return this;
            }
        }

        /// <summary>
        /// Requires the constraint that follows to be satisfied by every item of a collection.
        /// </summary>
        public new TestHelperConstraintExpression All
        {
            get
            {
                _builder.Append(new AllOperator());
                return this;
            }
        }

        /// <summary>
        /// Create constraint to destroyed <see cref="UnityEngine.Object"/>.
        /// </summary>
        public DestroyedConstraint Destroyed
        {
            get
            {
                var constraint = new DestroyedConstraint();
                _builder.Append(constraint);
                return constraint;
            }
        }

        /// <summary>
        /// Create constraint to <see cref="UnityEngine.RectTransform"/> within the screen.
        /// </summary>
        public WithinScreenConstraint WithinScreen
        {
            get
            {
                var constraint = new WithinScreenConstraint();
                _builder.Append(constraint);
                return constraint;
            }
        }

        /// <summary>
        /// Create constraint to a collection of <see cref="UnityEngine.RectTransform"/> where any pair overlaps.
        /// </summary>
        public OverlappingConstraint Overlapping
        {
            get
            {
                var constraint = new OverlappingConstraint();
                _builder.Append(constraint);
                return constraint;
            }
        }

        /// <summary>
        /// Create constraint to text overflowing its own <see cref="UnityEngine.RectTransform"/>.
        /// </summary>
        public TextOverflowingConstraint TextOverflowing
        {
            get
            {
                var constraint = new TextOverflowingConstraint();
                _builder.Append(constraint);
                return constraint;
            }
        }

        // Note: WithinContainer is intentionally NOT exposed here. It takes a RectTransform argument, so it is
        // always written in method style regardless (e.g., `Is.Not.WithinContainer(container)`); the existing
        // ConstraintExtensions.WithinContainer(this ConstraintExpression, RectTransform) already binds to this
        // type without any redeclaration.
    }
}
