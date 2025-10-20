// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

#if ENABLE_INSTANT_REPLAY
using System;
using System.Collections;
using System.IO;
using InstantReplay;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using TestHelper.RuntimeInternals;
using UniEnc;
using UnityEngine;
using UnityEngine.TestTools;

namespace TestHelper.Attributes
{
    /// <summary>
    /// Record a video and save it to file after running this test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RecordVideoAttribute : NUnitAttribute, IOuterUnityTestAction, IDisposable
    {
        private readonly string _baseDirectory;
        private readonly bool _namespaceToDirectory;
        private readonly uint _width;
        private readonly uint _height;
        private readonly uint _fpsHint;

        private bool _beforeGizmos;

        private RealtimeInstantReplaySession _session;

        /// <summary>
        /// Record a video and save it to file after running this test.
        /// <p/>
        /// Requires <see href="https://github.com/CyberAgentGameEntertainment/InstantReplay">Instant Replay for Unity</see> package v1.0.0 or newer.
        /// </summary>
        /// <param name="baseDirectory">Directory to save video.
        /// If omitted, the directory specified by command line argument "-testHelperScreenshotDirectory" is used.
        /// If the command line argument is also omitted, <c>Application.persistentDataPath</c> + "/TestHelper/Screenshots/" is used.</param>
        /// <param name="namespaceToDirectory">Insert subdirectory named from test namespace if true.</param>
        /// <param name="width">Video output max width. If omitted (0), use <see cref="Screen.width"/>.</param>
        /// <param name="height">Video output max height. If omitted (0), use <see cref="Screen.height"/>.</param>
        /// <param name="fpsHint">Video output frame rate hint.</param>
        public RecordVideoAttribute(
            string baseDirectory = null,
            bool namespaceToDirectory = false,
            uint width = 0,
            uint height = 0,
            uint fpsHint = 30)
        {
            if (baseDirectory != null)
            {
                _baseDirectory = Path.GetFullPath(baseDirectory);
            }
            else
            {
                _baseDirectory = CommandLineArgs.GetScreenshotDirectory();
            }

            _namespaceToDirectory = namespaceToDirectory;
            _width = width != 0 ? width : (uint)Screen.width;
            _height = height != 0 ? height : (uint)Screen.height;
            _fpsHint = fpsHint;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _session?.Dispose();
        }

        /// <inheritdoc/>
        public IEnumerator BeforeTest(ITest test)
        {
            _session?.Dispose();
            _session = new RealtimeInstantReplaySession(CreateOptions());
            yield break;
        }

        /// <inheritdoc/>
        public IEnumerator AfterTest(ITest test)
        {
            var outputPath = PathHelper.CreateFilePath(
                baseDirectory: _baseDirectory,
                extension: "mp4",
                namespaceToDirectory: _namespaceToDirectory);

            try
            {
                var task = _session.StopAndExportAsync(outputPath: outputPath);
                while (!task.IsCompleted)
                {
                    yield return null;
                }

                var properties = TestContext.CurrentContext.Test.Properties;
                properties.Add("Video", outputPath);
            }
            finally
            {
                _session.Dispose();
            }
        }

        private RealtimeEncodingOptions CreateOptions()
        {
            return new RealtimeEncodingOptions
            {
                VideoOptions = new VideoEncoderOptions
                {
                    Width = Math.Min(_width, (uint)Screen.width),
                    Height = Math.Min(_height, (uint)Screen.height),
                    FpsHint = _fpsHint,
                    Bitrate = 2500000 // 2.5 Mbps
                },
                AudioOptions = new AudioEncoderOptions
                {
                    SampleRate = 44100,
                    Channels = 2,
                    Bitrate = 128000 // 128 kbps
                },
                MaxMemoryUsageBytes = 20 * 1024 * 1024, // 20 MiB
                FixedFrameRate = 30.0, // null if not using fixed frame rate
                VideoInputQueueSize = 5, // Maximum number of raw frames to keep before encoding
                AudioInputQueueSize = 60, // Maximum number of raw audio sample frames to keep before encoding
            };
        }
    }
}
#endif
