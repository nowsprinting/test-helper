// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System.Diagnostics.CodeAnalysis;

namespace TestHelper.Attributes
{
    /// <summary>
    /// Enum for <c>GameView</c> resolution.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "MissingXmlDoc")]
    public enum GameViewResolution
    {
        VGA,
        XGA,
        WXGA,
        FullHD,
        QHD,
        FourK_UHD,
        QVGA,
        WQVGA,
        WVGA,
    }

    /// <summary>
    /// Extension methods for <c>GameViewResolution</c>.
    /// </summary>
    public static class GameViewResolutionExtensions
    {
        /// <summary>
        /// Get resolution parameter.
        /// </summary>
        /// <param name="resolution"></param>
        /// <returns>width, height, and name</returns>
        public static (uint, uint, string) GetParameter(this GameViewResolution resolution)
        {
            switch (resolution)
            {
                case GameViewResolution.QVGA:
                    return (320, 240, "QVGA");
                case GameViewResolution.WQVGA:
                    return (400, 240, "WQVGA");
                case GameViewResolution.VGA:
                    return (640, 480, "VGA");
                case GameViewResolution.WVGA:
                    return (800, 480, "WVGA");
                case GameViewResolution.XGA:
                    return (1024, 768, "XGA");
                case GameViewResolution.WXGA:
                    return (1366, 768, "WXGA");
                case GameViewResolution.FullHD:
                    return (1920, 1080, "Full HD");
                case GameViewResolution.QHD:
                    return (2560, 1440, "QHD");
                case GameViewResolution.FourK_UHD:
                    return (3840, 2160, "4K UHD");
                default:
                    return (0, 0, "undefined");
            }
        }
    }
}
