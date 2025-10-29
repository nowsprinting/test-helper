// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using TestHelper.RuntimeInternals;
using UnityEngine;
using UnityEngine.TestTools;

namespace TestHelper.Attributes
{
    /// <summary>
    /// Take a screenshot and save it to file after running this test.
    /// </summary>
    /// <seealso cref="ScreenshotHelper"/>
    [AttributeUsage(AttributeTargets.Method)]
    public class TakeScreenshotAttribute : NUnitAttribute, IOuterUnityTestAction
    {
        private readonly string _directory;
        private readonly string _filename;
        private readonly int _superSize;
        private readonly ScreenCapture.StereoScreenCaptureMode _stereoCaptureMode;
        private readonly bool _gizmos;
        private readonly bool _namespaceToDirectory;

        /// <summary>
        /// Take a screenshot and save it to file after running this test.
        /// <p/>
        /// Limitations:
        /// <list type="bullet">
        ///     <item>Do not place on Edit Mode tests.</item>
        ///     <item><c>GameView</c> must be visible. Use the <see cref="FocusGameViewAttribute"/> or <see cref="GameViewResolutionAttribute"/> if running on batch mode.</item>
        /// </list>
        /// If you want to take screenshots at any time, use the <see cref="ScreenshotHelper.TakeScreenshot"/> or <see cref="ScreenshotHelper.TakeScreenshotAsync"/> method.
        /// </summary>
        /// <param name="directory">Directory to save screenshots.
        /// If omitted, the directory specified by command line argument "-testHelperScreenshotDirectory" is used.
        /// If the command line argument is also omitted, <c>Application.persistentDataPath</c> + "/TestHelper/Screenshots/" is used.</param>
        /// <param name="filename">Filename to store screenshot.
        /// If omitted, default filename is <c>TestContext.Test.Name</c> + ".png".</param>
        /// <param name="superSize">The factor to increase resolution with.</param>
        /// <param name="stereoCaptureMode">The eye texture to capture when stereo rendering is enabled.</param>
        /// <param name="gizmos">Show Gizmos on <c>GameView</c> if true.</param>
        /// <param name="namespaceToDirectory">Insert subdirectory named from test namespace if true.</param>
        /// <remarks>
        /// When using MacOS with Metal Graphics API, the following warning appears at runtime. It seems that we should just ignore it.
        /// <list type="bullet">
        ///     <item>Ignoring depth surface load action as it is memoryless</item>
        ///     <item>Ignoring depth surface store action as it is memoryless</item>
        /// </list>
        /// </remarks>
        /// <seealso href="https://stackoverflow.com/questions/66062201/unity-warning-ignoring-depth-surface-load-action-as-it-is-memoryless"/>
        public TakeScreenshotAttribute(
            string directory = null,
            string filename = null,
            int superSize = 1,
            ScreenCapture.StereoScreenCaptureMode stereoCaptureMode = ScreenCapture.StereoScreenCaptureMode.LeftEye,
            bool gizmos = false,
            bool namespaceToDirectory = false)
        {
            _directory = directory;
            _filename = filename;
            _superSize = superSize;
            _stereoCaptureMode = stereoCaptureMode;
            _gizmos = gizmos;
            _namespaceToDirectory = namespaceToDirectory;
        }

        /// <inheritdoc />
        public IEnumerator BeforeTest(ITest test)
        {
            yield return null;
        }

        /// <inheritdoc />
        public IEnumerator AfterTest(ITest test)
        {
            var beforeGizmos = false;
            if (_gizmos)
            {
                beforeGizmos = GameViewControlHelper.GetGizmos();
                GameViewControlHelper.SetGizmos(true);
            }

            yield return ScreenshotHelper.TakeScreenshot(_directory, _filename, _superSize, _stereoCaptureMode, false,
                namespaceToDirectory: _namespaceToDirectory);

            if (_gizmos)
            {
                GameViewControlHelper.SetGizmos(beforeGizmos);
            }
        }
    }
}
