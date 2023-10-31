// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using TestHelper.RuntimeInternals;
using UnityEngine;
using UnityEngine.TestTools;
using ScreenshotHelper = TestHelper.Utils.ScreenshotHelper;

namespace TestHelper.Attributes
{
    /// <summary>
    /// Take a screenshot and save it to file after running the test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TakeScreenshotAttribute : NUnitAttribute, IOuterUnityTestAction
    {
        private readonly string _directory;
        private readonly string _filename;
        private readonly int _superSize;
        private readonly ScreenCapture.StereoScreenCaptureMode _stereoCaptureMode;
        private readonly bool _gizmos;

        /// <summary>
        /// Take a screenshot and save it to file after running the test.
        /// Default save path is $"{Application.persistentDataPath}/TestHelper/Screenshots/{CurrentTest.Name}.png".
        /// If you want to take screenshots at any time, use the <c>ScreenshotHelper</c> class.
        /// </summary>
        /// <remarks>
        /// Limitations:
        ///  - Do not attach to Edit Mode tests.
        ///  - <c>GameView</c> must be visible. Use <c>FocusGameViewAttribute</c> or <c>GameViewResolutionAttribute</c> if running on batch mode.
        /// <br/>
        /// Using <c>ScreenCapture.CaptureScreenshotAsTexture</c> internally.
        /// </remarks>
        /// <param name="directory">Directory to save screenshots.</param>
        /// <param name="filename">Filename to store screenshot.</param>
        /// <param name="superSize">The factor to increase resolution with.</param>
        /// <param name="stereoCaptureMode">The eye texture to capture when stereo rendering is enabled.</param>
        /// <param name="gizmos">True: show Gizmos on GameView</param>
        public TakeScreenshotAttribute(
            string directory = null,
            string filename = null,
            int superSize = 1,
            ScreenCapture.StereoScreenCaptureMode stereoCaptureMode = ScreenCapture.StereoScreenCaptureMode.LeftEye,
            bool gizmos = false)
        {
            _directory = directory;
            _filename = filename;
            _superSize = superSize;
            _stereoCaptureMode = stereoCaptureMode;
            _gizmos = gizmos;
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

            yield return ScreenshotHelper.TakeScreenshot(_directory, _filename, _superSize, _stereoCaptureMode);

            if (_gizmos)
            {
                GameViewControlHelper.SetGizmos(beforeGizmos);
            }
        }
    }
}
