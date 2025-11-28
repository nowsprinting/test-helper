// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using TestHelper.RuntimeInternals;
using UnityEngine.TestTools;

namespace TestHelper.Attributes
{
    /// <summary>
    /// Show/ hide Gizmos on <c>GameView</c> during this test running.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class GizmosShowOnGameViewAttribute : NUnitAttribute, IOuterUnityTestAction
    {
        private readonly bool _show;
        private bool _beforeShow;

        /// <summary>
        /// Show/ hide Gizmos on <c>GameView</c> during this test running.
        /// </summary>
        /// <param name="show">True: show Gizmos, False: hide Gizmos.</param>
        public GizmosShowOnGameViewAttribute(bool show = true)
        {
            _show = show;
        }

        /// <inheritdoc />
        public IEnumerator BeforeTest(ITest test)
        {
            _beforeShow = GameViewControlHelper.GetGizmos();
            GameViewControlHelper.SetGizmos(_show);
            yield return null;
        }

        /// <inheritdoc />
        public IEnumerator AfterTest(ITest test)
        {
            GameViewControlHelper.SetGizmos(_beforeShow);
            yield return null;
        }
    }
}
