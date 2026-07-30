// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

using NUnit.Framework.Constraints;

namespace TestHelper.Constraints
{
    /// <summary>
    /// A <see cref="ConstraintResult"/> that writes its actual value verbatim, so the diagnostic message
    /// survives even when negated by <c>Is.Not</c> (NUnit rebuilds a plain <see cref="ConstraintResult"/>
    /// from <c>ActualValue</c> in that case, discarding any subclass).
    /// </summary>
    internal sealed class ReportingConstraintResult : ConstraintResult
    {
        internal ReportingConstraintResult(IConstraint constraint, object actualValue, bool isSuccess)
            : base(constraint, actualValue, isSuccess)
        {
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            writer.Write(ActualValue == null ? "null" : ActualValue.ToString());
        }
    }
}
