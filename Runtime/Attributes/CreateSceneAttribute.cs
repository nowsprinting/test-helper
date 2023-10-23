// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections;
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
    /// Create new scene before running test.
    ///
    /// Notes:
    ///  - Create scene run after <c>OneTimeSetUp</c> and before <c>SetUp</c>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CreateSceneAttribute : NUnitAttribute, IOuterUnityTestAction
    {
        private readonly bool _camera;
        private readonly bool _light;
        private string _newSceneName;

        /// <summary>
        /// Create new scene before running test.
        /// </summary>
        /// <param name="camera">true: create main camera object in new scene</param>
        /// <param name="light">true:  create directional light object in new scene</param>
        public CreateSceneAttribute(bool camera = false, bool light = false)
        {
            _camera = camera;
            _light = light;
        }

        /// <inheritdoc />
        public IEnumerator BeforeTest(ITest test)
        {
            _newSceneName = $"Scene of {TestContext.CurrentContext.Test.FullName}";

            if (Application.isPlaying)
            {
                // Play Mode tests
                var scene = SceneManager.CreateScene(_newSceneName);
                SceneManager.SetActiveScene(scene);
            }
            else
            {
#if UNITY_EDITOR
                // Edit Mode tests
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
                scene.name = _newSceneName;
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

            yield return null;
        }

        /// <inheritdoc />
        public IEnumerator AfterTest(ITest test)
        {
            if (Application.isPlaying && SceneManager.GetActiveScene().name == _newSceneName)
            {
                yield return SceneManager.UnloadSceneAsync(_newSceneName);
            }
        }
    }
}
