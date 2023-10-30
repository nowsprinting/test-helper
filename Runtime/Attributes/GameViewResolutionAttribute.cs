// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using TestHelper.RuntimeInternals;

namespace TestHelper.Attributes
{
    /// <summary>
    /// Set <c>GameView</c> resolution before SetUp test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public class GameViewResolutionAttribute : NUnitAttribute, IApplyToContext
    {
        private readonly uint _width;
        private readonly uint _height;
        private readonly string _name;

        /// <summary>
        /// Set <c>GameView</c> resolution before SetUp test.
        /// </summary>
        /// <param name="width">GameView width [px]</param>
        /// <param name="height">GameView height [px]</param>
        /// <param name="name">GameViewSize name</param>
        public GameViewResolutionAttribute(uint width, uint height, string name)
        {
            _width = width;
            _height = height;
            _name = name;
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
