// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TestHelper.RuntimeInternals;
using UnityEngine;

namespace TestHelper.Statistics
{
    /// <summary>
    /// Plot samples to pixel plot image.
    /// Can plot only for samples with value type.
    /// </summary>
    public class PixelPlot<T> where T : IComparable, IConvertible
    {
        internal Texture2D _pixelPlot;

        /// <summary>
        /// Constructor of PixelPlot.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if T is a reference type</exception>
        public PixelPlot()
        {
            if (!typeof(T).IsValueType)
            {
                throw new ArgumentException("T must be a value type.");
            }
        }

        /// <summary>
        /// Plot samples into PixelPlot.
        /// </summary>
        /// <param name="samples">Input samples</param>
        /// <param name="size">Sample size</param>
        /// <param name="min">Minimum value of samples. Plotted as white (transparent).</param>
        /// <param name="max">Maximum value of samples. Plotted as black.</param>
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public void Plot(IEnumerable<T> samples, ulong size = 0, T min = default, T max = default)
        {
            if (size == 0)
            {
                size = (ulong)samples.Count();
            }

            var minValue = Convert.ToDouble(min);
            var multiplier = 1.0 / (Convert.ToDouble(max) - minValue);
            // Note: Make min value as 0.0, max value as 1.0

            var pixel = new Color(0, 0, 0, 0);

            var edge = (int)Math.Ceiling(Math.Sqrt(size));
            _pixelPlot = new Texture2D(edge, edge, TextureFormat.Alpha8, false);

            using (var enumerator = samples.GetEnumerator())
            {
                for (var x = 0; x < edge; x++)
                {
                    for (var y = 0; y < edge; y++)
                    {
                        var value = enumerator.MoveNext() ? enumerator.Current : min;
                        if (value == null || value.CompareTo(min) < 0)
                        {
                            value = min;
                        }
                        else if (value.CompareTo(max) > 0)
                        {
                            value = max;
                        }

                        pixel.a = (float)((Convert.ToDouble(value) - minValue) * multiplier);
                        _pixelPlot.SetPixel(x, y, pixel);
                    }
                }
            }

            _pixelPlot.Apply();
        }

        /// <summary>
        /// Plot samples into PixelPlot.
        /// </summary>
        /// <param name="sampleSpace">Input sample space</param>
        public void Plot(ISampleSpace<T> sampleSpace)
        {
            Plot(sampleSpace.Samples, (ulong)sampleSpace.Samples.Count(), sampleSpace.Min, sampleSpace.Max);
        }

        /// <summary>
        /// Write a pixel plot image to file.
        /// </summary>
        /// <param name="directory">Directory to output pixel plot.
        /// If omitted, the directory specified by command line argument "-testHelperStatisticsDirectory" is used.
        /// If the command line argument is also omitted, <c>Application.persistentDataPath</c> + "/TestHelper/Statistics/" is used.</param>
        /// <param name="filename">Filename to output pixel plot.
        /// If omitted, <c>TestContext.Test.Name</c> + ".png" is used.</param>
        public void WriteToFile(string directory = null, string filename = null)
        {
            if (_pixelPlot == null)
            {
                throw new InvalidOperationException("PixelPlot is not plotted yet.");
            }

            if (string.IsNullOrEmpty(directory))
            {
                directory = CommandLineArgs.GetStatisticsDirectory();
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (string.IsNullOrEmpty(filename))
            {
                filename = $"{TestContext.CurrentContext.Test.Name}.png";
            }

            var path = Path.Combine(directory, filename);
            var bytes = _pixelPlot.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
        }
    }
}
