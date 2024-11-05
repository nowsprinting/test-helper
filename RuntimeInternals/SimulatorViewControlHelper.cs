// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEngine;
#if UNITY_EDITOR && UNITY_2022_2_OR_NEWER
using UnityEditor;
using Screen = UnityEngine.Device.Screen; // defined in UnityEditor.DeviceSimulatorModule module
#endif

namespace TestHelper.RuntimeInternals
{
    /// <summary>
    /// <c>SimulatorView</c> control helper.
    /// This class can be used from the runtime code because it does not depend on test-framework.
    /// This class works only with Unity 2022.2 or newer. (not support device simulator package)
    /// </summary>
    public static class SimulatorViewControlHelper
    {
        /// <summary>
        /// Focus <c>SimulatorView</c>.
        /// </summary>
        public static void Focus()
        {
#if UNITY_EDITOR && UNITY_2022_2_OR_NEWER
            PlayModeWindow.SetViewType(PlayModeWindow.PlayModeViewTypes.SimulatorView);
#else
            Debug.LogError("SimulatorView is not supported.");
#endif
        }

        /// <summary>
        /// Set <c>ScreenOrientation</c> on <c>SimulatorView</c>.
        /// </summary>
        /// <param name="orientation">Screen orientation. However, <c>AutoRotation</c> is ignored</param>
        public static void SetScreenOrientation(ScreenOrientation orientation)
        {
#if UNITY_EDITOR && UNITY_2022_2_OR_NEWER
            PlayModeWindow.SetViewType(PlayModeWindow.PlayModeViewTypes.SimulatorView);
            Screen.orientation = orientation; // UnityEngine.Device.Screen
#else
            Debug.LogError("SimulatorView is not supported.");
#endif
        }
    }
}
