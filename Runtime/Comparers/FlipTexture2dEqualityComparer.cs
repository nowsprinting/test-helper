// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

#if ENABLE_FLIP_BINDING
using System.Collections.Generic;
using System.IO;
using FlipBinding.CSharp;
using TestHelper.RuntimeInternals;
using UnityEngine;

namespace TestHelper.Comparers
{
    /// <summary>
    /// Compares two <see cref="Texture2D"/> using <see href="https://github.com/NVlabs/flip">FLIP</see>.
    /// </summary>
    public class FlipTexture2dEqualityComparer : IComparer<Texture2D>
    {
        private const float DefaultMeanErrorTolerance = 1E-05f;
        private const float DefaultPpd = 67.0206451f;

        // Comparison parameters
        private readonly float _meanErrorTolerance;
        private readonly string _errorMapOutputDirectory;
        private readonly string _errorMapOutputFilename;
        private readonly bool _namespaceToDirectory;

        // FLIP parameters
        private readonly bool _useHdr;
        private readonly float _ppd;
        private readonly Tonemapper _tonemapper;
        private readonly float _startExposure;
        private readonly float _stopExposure;
        private readonly int _numExposures;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="meanErrorTolerance">Mean FLIP error value in the range [0,1]. Lower values indicate more similar images.</param>
        /// <param name="errorMapOutputDirectory">Directory to output error map.
        /// If omitted, the directory specified by command line argument <c>-testHelperScreenshotDirectory</c> is used.
        /// If the command line argument is also omitted, <c>Application.persistentDataPath</c> + "/TestHelper/Screenshots/" is used.</param>
        /// <param name="errorMapOutputFilename">Filename to output error map.
        /// If omitted, default filename is <c>TestContext.Test.Name</c> + ".diff.png".</param>
        /// <param name="namespaceToDirectory">Insert subdirectory named from test namespace if true.</param>
        /// <param name="useHdr">Whether to use HDR mode. LDR: values in [0,1], HDR: values can exceed [0,1].</param>
        /// <param name="ppd">Pixels per degree. Default is 67 (4K display at 0.7m viewing distance). You can calculate PPD with <see cref="Flip.CalculatePpd"/></param>
        /// <param name="tonemapper">Tonemapper for HDR-FLIP processing. Ignored when useHdr is false.</param>
        /// <param name="startExposure">Start exposure for HDR-FLIP. Use float.PositiveInfinity for auto-calculation.</param>
        /// <param name="stopExposure">Stop exposure for HDR-FLIP. Use float.PositiveInfinity for auto-calculation.</param>
        /// <param name="numExposures">Number of exposures for HDR-FLIP. Use -1 for auto-calculation.</param>
        public FlipTexture2dEqualityComparer(
            float meanErrorTolerance = DefaultMeanErrorTolerance,
            string errorMapOutputDirectory = null,
            string errorMapOutputFilename = null,
            bool namespaceToDirectory = false,
            bool useHdr = false,
            float ppd = DefaultPpd,
            Tonemapper tonemapper = Tonemapper.Aces,
            float startExposure = float.PositiveInfinity,
            float stopExposure = float.PositiveInfinity,
            int numExposures = -1)
        {
            _meanErrorTolerance = meanErrorTolerance;
            _errorMapOutputDirectory = errorMapOutputDirectory;
            _errorMapOutputFilename = errorMapOutputFilename;
            _namespaceToDirectory = namespaceToDirectory;
            _useHdr = useHdr;
            _ppd = ppd;
            _tonemapper = tonemapper;
            _startExposure = startExposure;
            _stopExposure = stopExposure;
            _numExposures = numExposures;
        }

        /// <inheritdoc/>
        public int Compare(Texture2D x, Texture2D y)
        {
            if (x!.width != y!.width || x.height != y.height)
            {
                Debug.Log("Texture sizes are different.\n" +
                          $"  Expected: {x.width}x{x.height}\n" +
                          $"  But was:  {y.width}x{y.height}\n");
                return -1;
            }

            var referenceRgb = ConvertToRgbArray(y);
            var testRgb = ConvertToRgbArray(x);

            var result = Flip.Evaluate(
                referenceRgb,
                testRgb,
                y.width,
                y.height,
                _useHdr,
                _ppd,
                _tonemapper,
                _startExposure,
                _stopExposure,
                _numExposures,
                applyMagmaMap: true);

            if (result.MeanError <= _meanErrorTolerance)
            {
                return 0;
            }

            Debug.Log($"Mean FLIP error value: {result.MeanError}\n" +
                      $"Exceeds the specified tolerance {_meanErrorTolerance:F10}".TrimEnd('0'));
            SaveErrorMap(result);
            return -1;
        }

        private static float[] ConvertToRgbArray(Texture2D texture)
        {
            var pixels = texture.GetPixels();
            var rgb = new float[pixels.Length * 3];

            for (var i = 0; i < pixels.Length; i++)
            {
                var linear = pixels[i].linear;
                rgb[i * 3] = linear.r;
                rgb[i * 3 + 1] = linear.g;
                rgb[i * 3 + 2] = linear.b;
            }

            return rgb;
        }

        private void SaveErrorMap(FlipResult result)
        {
            var path = GetErrorMapPath();
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var texture = new Texture2D(result.Width, result.Height, TextureFormat.RGB24, false);
            var colors = new Color32[result.Width * result.Height];

            for (var i = 0; i < colors.Length; i++)
            {
                var (r, g, b) = result.GetPixelRgb(i % result.Width, i / result.Width);
                colors[i] = new Color(r, g, b);
            }

            texture.SetPixels32(colors);
            texture.Apply();

            var bytes = texture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);

            Debug.Log($"Error map save to: {path}");
            Object.Destroy(texture);
        }

        private string GetErrorMapPath()
        {
            string directory;
            if (_errorMapOutputDirectory != null)
            {
                directory = Path.GetFullPath(_errorMapOutputDirectory);
            }
            else
            {
                directory = CommandLineArgs.GetScreenshotDirectory();
            }

            string path;
            if (_errorMapOutputFilename == null)
            {
                path = PathHelper.CreateFilePath(
                    baseDirectory: directory,
                    extension: ".diff.png",
                    namespaceToDirectory: _namespaceToDirectory);
            }
            else
            {
                path = Path.Combine(directory, _errorMapOutputFilename);
                if (!path.EndsWith(".png"))
                {
                    path += ".png";
                }
            }

            return path;
        }
    }
}
#endif
