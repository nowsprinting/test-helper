// Copyright (c) 2023 Koji Hasegawa.
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
        FourK_UHD
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
            return resolution switch
            {
                GameViewResolution.VGA => (640, 480, "VGA"),
                GameViewResolution.XGA => (1024, 768, "XGA"),
                GameViewResolution.WXGA => (1366, 768, "WXGA"),
                GameViewResolution.FullHD => (1920, 1080, "Full HD"),
                GameViewResolution.QHD => (2560, 1440, "QHD"),
                GameViewResolution.FourK_UHD => (3840, 2160, "4K UHD"),
                _ => (0, 0, "undefined")
            };
        }
    }
}
