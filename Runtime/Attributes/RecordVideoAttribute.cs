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
        public RecordVideoAttribute(string baseDirectory = null, bool namespaceToDirectory = false)
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
            _session = RealtimeInstantReplaySession.CreateDefault();
            yield break;
        }

        /// <inheritdoc/>
        public IEnumerator AfterTest(ITest test)
        {
            var outputPath = PathHelper.CreateFilePath(
                baseDirectory: _baseDirectory,
                extension: "mp4",
                namespaceToDirectory: _namespaceToDirectory);

            var task = _session.StopAndExportAsync(outputPath: outputPath);
            while (!task.IsCompleted)
            {
                yield return null;
            }

            _session.Dispose();

            var properties = TestContext.CurrentContext.Test.Properties;
            properties.Add("Video", outputPath);
        }
    }
}
#endif
