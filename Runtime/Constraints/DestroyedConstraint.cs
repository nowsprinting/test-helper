// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework.Constraints;
using UnityEngine;

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
    ///     // Note: When used with operators, use it in method style. e.g., `Is.Not.Destroyed()`
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

        public override ConstraintResult ApplyTo(object actual)
        {
            if (actual is Object actualObject)
            {
                return new ConstraintResult(this, actual, !(bool)actualObject);
            }

            return new ConstraintResult(this, actual, false);
        }
    }
}
