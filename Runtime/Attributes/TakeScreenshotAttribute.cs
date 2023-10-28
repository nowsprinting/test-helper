// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using TestHelper.Utils;
using UnityEngine;
using UnityEngine.TestTools;

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
        /// <param name="directory">Directory to save screenshots relative to project path. Only effective in Editor.</param>
        /// <param name="filename">Filename to store screenshot.</param>
        /// <param name="superSize">The factor to increase resolution with.</param>
        /// <param name="stereoCaptureMode">The eye texture to capture when stereo rendering is enabled.</param>
        public TakeScreenshotAttribute(
            string directory = null,
            string filename = null,
            int superSize = 1,
            ScreenCapture.StereoScreenCaptureMode stereoCaptureMode = ScreenCapture.StereoScreenCaptureMode.LeftEye
        )
        {
            _directory = directory;
            _filename = filename;
            _superSize = superSize;
            _stereoCaptureMode = stereoCaptureMode;
        }

        /// <inheritdoc />
        public IEnumerator BeforeTest(ITest test)
        {
            yield return null;
        }

        /// <inheritdoc />
        public IEnumerator AfterTest(ITest test)
        {
            yield return ScreenshotHelper.TakeScreenshot(_directory, _filename, _superSize, _stereoCaptureMode);
        }
    }
}
