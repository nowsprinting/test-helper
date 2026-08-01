// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

namespace TestHelper.Constraints
{
    /// <summary>
    /// Carries a human-readable diagnostic message as the actual value of a constraint result.
    /// </summary>
    /// <remarks>
    /// Not a <see cref="string"/> because NUnit's default value formatter quotes and clips string actual
    /// values; wrapping the message in this type lets it print verbatim via <see cref="ToString"/>.
    /// </remarks>
    internal sealed class ConstraintReport
    {
        private readonly string _message;

        internal ConstraintReport(string message)
        {
            _message = message;
        }

        public override string ToString()
        {
            return _message;
        }
    }
}
