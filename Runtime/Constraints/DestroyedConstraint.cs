// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using NUnit.Framework.Constraints;

namespace TestHelper.Constraints
{
    /// <summary>
    /// An NUnit test constraint class to destroyed <see cref="UnityEngine.Object"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// [TestFixture]
    /// public class MyTestClass
    /// {
    ///   [Test]
    ///   public void MyTestMethod()
    ///   {
    ///     var actual = GameObject.Find("Cube");
    ///     GameObject.DestroyImmediate(actual);
    ///
    ///     Assert.That(actual, Is.Destroyed);
    ///     // Note: Works with operators too, e.g., `Is.Not.Destroyed`. Method style (`Is.Not.Destroyed()`)
    ///     // also remains available.
    ///   }
    /// }
    /// </code>
    /// </example>
    public class DestroyedConstraint : Constraint
    {
        public DestroyedConstraint(params object[] args) : base(args)
        {
            base.Description = "destroyed UnityEngine.Object";
        }

        /// <summary>
        /// Requires both this constraint and the one that follows to be satisfied.
        /// </summary>
        public new TestHelperConstraintExpression And => this.AppendOperator(new AndOperator());

        /// <summary>
        /// Requires either this constraint or the one that follows to be satisfied.
        /// </summary>
        public new TestHelperConstraintExpression Or => this.AppendOperator(new OrOperator());

        /// <summary>
        /// Alias of <see cref="And"/>, matching NUnit's own <see cref="Constraint.With"/>.
        /// </summary>
        public new TestHelperConstraintExpression With => this.AppendOperator(new AndOperator());

        /// <inheritdoc/>
        /// <exception cref="System.ArgumentNullException"><paramref name="actual"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="actual"/> is not a
        /// <see cref="UnityEngine.Object"/>.</exception>
        public override ConstraintResult ApplyTo(object actual)
        {
            if (actual == null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            if (!(actual is UnityEngine.Object actualObject))
            {
                throw new ArgumentException(
                    $"{ConstraintMessageFormatter.DescribeActual(actual)} is not a UnityEngine.Object",
                    nameof(actual));
            }

            return new ConstraintResult(this, actual, !(bool)actualObject);
        }
    }
}
