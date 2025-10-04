// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using TestHelper.Attributes;
using TestHelper.RuntimeInternals;

namespace TestHelper.Editor
{
    public static class GameViewResolutionSwitcher
    {
        public static void ParseArgumentsAndSwitchIfNeeded()
        {
            // Try by resolution name
            var resolutionName = CommandLineArgs.GetGameViewResolutionName();
            if (!string.IsNullOrEmpty(resolutionName))
            {
                resolutionName = resolutionName.Replace("\"", "").ToLower();
                foreach (GameViewResolution resolution in Enum.GetValues(typeof(GameViewResolution)))
                {
                    var (width, height, name) = resolution.GetParameter();
                    if (name.ToLower() == resolutionName)
                    {
                        GameViewControlHelper.SetResolution(width, height, name);
                        return;
                    }
                }
            }

            // Try by width and height
            var (w, h) = CommandLineArgs.GetGameViewResolutionSize();
            if (w > 0 && h > 0)
            {
                GameViewControlHelper.SetResolution(w, h, GetDefaultName(w, h));
            }
        }

        private static string GetDefaultName(uint width, uint height)
        {
            foreach (GameViewResolution resolution in Enum.GetValues(typeof(GameViewResolution)))
            {
                var (w, h, name) = resolution.GetParameter();
                if (w == width && h == height)
                {
                    return $"{name} ({width}x{height})";
                }
            }

            return $"{width}x{height}";
        }
    }
}
