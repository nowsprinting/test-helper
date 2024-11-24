// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEditor;
using UnityEngine;

namespace TestHelper.Editor
{
    /// <summary>
    /// Open temporary cache directory in the Finder.
    /// <see href="https://docs.unity3d.com/ScriptReference/Application-temporaryCachePath.html">Application.temporaryCachePath</see>
    /// </summary>
    public static class OpenTemporaryCachePathMenu
    {
        [MenuItem("Window/Test Helper/Open Temporary Cache Directory")]
        private static void OpenTemporaryCachePathMenuItem()
        {
            EditorUtility.RevealInFinder(Application.temporaryCachePath);
        }
    }
}
