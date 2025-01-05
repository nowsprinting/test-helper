// Copyright (c) 2023-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace TestHelper.Attributes
{
    /// <summary>
    /// Create a new scene before running this test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CreateSceneAttribute : NUnitAttribute, IOuterUnityTestAction
    {
        private readonly bool _camera;
        private readonly bool _light;
        private readonly bool _unloadOthers;
        private Scene _beforeActiveScene;
        private Scene _newScene;

        /// <summary>
        /// Create a new scene before running this test.
        /// 
        /// This process runs after <c>OneTimeSetUp</c> and before <c>SetUp</c>.
        /// 
        /// This attribute has the following benefits:
        /// - Can be use same code for running Edit Mode tests, Play Mode tests in Editor, and on Player
        /// </summary>
        /// <param name="camera">true: create main camera object in new scene</param>
        /// <param name="light">true: create directional light object in new scene</param>
        /// <param name="unloadOthers">true: unload other scenes before creating a new scene</param>
        public CreateSceneAttribute(bool camera = false, bool light = false, bool unloadOthers = false)
        {
            _camera = camera;
            _light = light;
            _unloadOthers = unloadOthers;
        }

        /// <inheritdoc />
        public IEnumerator BeforeTest(ITest test)
        {
            var newSceneName = $"Scene of {TestContext.CurrentContext.Test.FullName}";

            _beforeActiveScene = SceneManager.GetActiveScene();

            if (Application.isPlaying)
            {
                // Play Mode tests
                _newScene = SceneManager.CreateScene(newSceneName);
                SceneManager.SetActiveScene(_newScene);
            }
            else
            {
#if UNITY_EDITOR
                // Edit Mode tests
                _newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
                _newScene.name = newSceneName;
#endif
            }

            if (_camera)
            {
                var camera = new GameObject("Main Camera").AddComponent<Camera>();
                camera.transform.position = new Vector3(0, 1, -10);
                camera.transform.LookAt(Vector3.zero);
            }

            if (_light)
            {
                var light = new GameObject("Directional Light").AddComponent<Light>();
                light.transform.position = new Vector3(0, 3, 0);
                light.transform.rotation = Quaternion.Euler(new Vector3(50, -30, 0));
                light.type = LightType.Directional;
                light.color = new Color(0xff, 0xf4, 0xd6);
            }

            if (_unloadOthers)
            {
                yield return UnloadOtherScenes();
            }
        }

        private IEnumerator UnloadOtherScenes()
        {
            var scenes = new List<Scene>();

            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene != _newScene && !scene.name.StartsWith("InitTestScene"))
                {
                    scenes.Add(scene);
                }
            }

            foreach (var scene in scenes)
            {
                var asyncOperation = SceneManager.UnloadSceneAsync(scene);
                while (asyncOperation != null && !asyncOperation.isDone)
                {
                    yield return null;
                }
            }
        }

        /// <inheritdoc />
        public IEnumerator AfterTest(ITest test)
        {
            if (_beforeActiveScene.isLoaded)
            {
                SceneManager.SetActiveScene(_beforeActiveScene);
            }

            if (SceneManager.GetActiveScene() != _newScene)
            {
                yield return SceneManager.UnloadSceneAsync(_newScene);
            }
        }
    }
}
