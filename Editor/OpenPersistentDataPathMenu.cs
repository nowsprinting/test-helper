// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEditor;
using UnityEngine;

namespace TestHelper.Editor
{
    /// <summary>
    /// Open persistent data directory in the Finder.
    /// <see href="https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html">Application.persistentDataPath</see>
    /// </summary>
    public static class OpenPersistentDataPathMenu
    {
        [MenuItem("Window/Test Helper/Open Persistent Data Directory")]
        private static void OpenPersistentDataPathMenuItem()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }
    }
}
