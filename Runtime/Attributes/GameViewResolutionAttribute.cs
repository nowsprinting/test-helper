// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using TestHelper.RuntimeInternals;

namespace TestHelper.Attributes
{
    /// <summary>
    /// Set custom resolution to <c>GameView</c> before running this test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public class GameViewResolutionAttribute : NUnitAttribute, IApplyToContext
    {
        private readonly uint _width;
        private readonly uint _height;
        internal readonly string _name;

        /// <summary>
        /// Set <c>GameView</c> resolution before running this test.
        ///
        /// This process runs after <c>OneTimeSetUp</c> and before <c>SetUp</c>.
        /// </summary>
        /// <param name="width">GameView width [px]</param>
        /// <param name="height">GameView height [px]</param>
        /// <param name="name">GameViewSize name</param>
        public GameViewResolutionAttribute(uint width, uint height, string name = null)
        {
            _width = width;
            _height = height;
            _name = name ?? GetDefaultName(width, height);
        }

        private static string GetDefaultName(uint width, uint height)
        {
            foreach (GameViewResolution resolution in Enum.GetValues(typeof(GameViewResolution)))
            {
                var (w, h, name) = resolution.GetParameter();
                if (w == width && h == height)
                {
                    return $"{name} ({width}x{height})";
                }
            }

            return $"{width}x{height}";
        }

        /// <summary>
        /// Set <c>GameView</c> resolution before SetUp test.
        /// </summary>
        /// <param name="resolution">GameView resolutions enum</param>
        public GameViewResolutionAttribute(GameViewResolution resolution)
        {
            (_width, _height, _name) = resolution.GetParameter();
        }

        /// <inheritdoc />
        public void ApplyToContext(ITestExecutionContext context)
        {
            GameViewControlHelper.SetResolution(_width, _height, _name);
        }
    }
}
