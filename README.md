# Test helper library for Unity Test Framework

[![Meta file check](https://github.com/nowsprinting/test-helper/actions/workflows/metacheck.yml/badge.svg)](https://github.com/nowsprinting/test-helper/actions/workflows/metacheck.yml)
[![Test](https://github.com/nowsprinting/test-helper/actions/workflows/test.yml/badge.svg)](https://github.com/nowsprinting/test-helper/actions/workflows/test.yml)
[![openupm](https://img.shields.io/npm/v/com.nowsprinting.test-helper?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.nowsprinting.test-helper/)

Provides custom attributes, comparers, and constraints useful for testing with Unity Test Framework.

Required Unity 2019 LTS or later.



## Features

### Attributes

#### FocusGameView

`FocusGameViewAttribute` is an NUnit test attribute class to focus `GameView` or `SimulatorWindow` before run test.

This attribute can attach to test method, test class (`TestFixture`), and test assembly.
Can be used with sync Test, async Test, and UnityTest.

Usage:

```csharp
using NUnit.Framework;
using TestHelper.Attributes;

[TestFixture]
public class MyTestClass
{
    [Test]
    [FocusGameView]
    public void MyTestMethod()
    {
        // e.g., Test using InputEventTrace of Input System package.
    }
}
```

> [!NOTE]  
> In batchmode, open `GameView` window.


#### GameViewResolution

`GameViewResolutionAttribute` is an NUnit test attribute class to set custom resolution to `GameView` before run test.

This attribute can attach to test method, test class (`TestFixture`), and test assembly.
Can be used with async Test and UnityTest.

Usage:

```csharp
using System.Collections;
using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine.TestTools;

[TestFixture]
public class MyTestClass
{
    [UnityTest]
    [GameViewResolution(640, 480, "VGA")]
    public IEnumerator MyTestMethod()
    {
        yield return null; // Wait for one frame to apply resolution.

        // e.g., Test using GraphicRaycaster.
    }
}
```

> [!WARNING]  
> Wait for one frame to apply resolution.
> However, if used with [CreateSceneAttribute](#CreateScene) or [LoadSceneAttribute](#LoadScene), wait is not necessary.

> [!NOTE]  
> In batchmode, open `GameView` window.


#### GizmosShowOnGameView

`GizmosShowOnGameViewAttribute` is an NUnit test attribute class to show/hide Gizmos on `GameView` during the test running.

This attribute can attach to test method only.
Can be used with sync Test, async Test, and UnityTest.

Usage:

```csharp
using NUnit.Framework;
using TestHelper.Attributes;

[TestFixture]
public class MyTestClass
{
    [Test]
    [GizmosShowOnGameView(true)]
    public void MyTestMethod()
    {
        // Show Gizmos on GameView.
    }
}
```

> [!NOTE]  
> In batchmode, open `GameView` window.


#### IgnoreBatchMode

`IgnoreBatchModeAttribute` is an NUnit test attribute class to skip test execution when run tests with `-batchmode` from the commandline.

This attribute can attach to test method, test class (`TestFixture`), and test assembly.
Can be used with sync Test, async Test, and UnityTest.

Usage:

```csharp
using System.Collections;
using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Graphics;

[TestFixture]
public class MyTestClass
{
    [UnityTest]
    [IgnoreBatchMode("Using WaitForEndOfFrame.")]
    public IEnumerator MyTestMethod()
    {
        // e.g., Test needs to take a screenshot.

        yield return new WaitForEndOfFrame();
        ImageAssert.AreEqual(expectedTexture, Camera.main, settings);
    }
}
```


#### IgnoreWindowMode

`IgnoreWindowModeAttribute` is an NUnit test attribute class to skip test execution when run tests on Unity editor window.

This attribute can attach to test method, test class (`TestFixture`), and test assembly.
Can be used with sync Test, async Test, and UnityTest.

Usage:

```csharp
using System;
using NUnit.Framework;
using TestHelper.Attributes;

[TestFixture]
public class MyTestClass
{
    [Test]
    [IgnoreWindowMode("Requires command line arguments")]
    public void MyTestMethod()
    {
        var args = Environment.GetCommandLineArgs();
        Assert.That(args, Does.Contain("-arg1"));
    }
}
```


#### UnityVersion

`UnityVersionAttribute` is an NUnit test attribute class to skip test run if Unity version is older and/or newer than specified.

This attribute can attach to test method, test class (`TestFixture`), and test assembly.
Can be used with sync Test, async Test, and UnityTest.

Usage:

```csharp
using System;
using NUnit.Framework;
using TestHelper.Attributes;

[TestFixture]
public class MyTestClass
{
    [Test]
    [UnityVersion(newerThanOrEqual: "2022")]
    public void MyTestMethod()
    {
        // Test run only for Unity 2022.1.0f1 or later.
    }

    [Test]
    [UnityVersion(olderThan: "2019.4.0f1")]
    public void MyTestMethod()
    {
        // Test run only for Unity older than 2019.4.0f1.
    }
}
```


#### CreateScene

`CreateSceneAttribute` is an NUnit test attribute class to create new scene before running test.

It has the following benefits:

- Can be use same code for running Edit Mode tests, Play Mode tests in Editor, and on Player

This attribute can attach to test method only.
Can be used with sync Test, async Test, and UnityTest.

Usage:

```csharp
using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine;

[TestFixture]
public class MyTestClass
{
    [Test]
    [CreateScene(camera: true, light: true)]
    public void MyTestMethod()
    {
        var camera = GameObject.Find("Main Camera");
        Assert.That(camera, Is.Not.Null);

        var light = GameObject.Find("Directional Light");
        Assert.That(light, Is.Not.Null);
    }
}
```

> [!NOTE]  
> - Create scene run after `OneTimeSetUp` and before `SetUp`
> - Create or not `Main Camera` and `Directional Light` can be specified with parameters (default is not create)


#### LoadScene

`LoadSceneAttribute` is a NUnit test attribute class that loads the scene before running the test.

It has the following benefits:

- Can be use same code for running Edit Mode tests, Play Mode tests in Editor, and on Player.
- Can be specified scenes that are **NOT** in "Scenes in Build".
- Can be specified scene path by [glob](https://en.wikipedia.org/wiki/Glob_(programming)) pattern. However, there are restrictions, top level and scene name cannot be omitted.
- Can be specified scene path by relative path from the test class file.

This attribute can attach to the test method only.
It can be used with sync Tests, async Tests, and UnityTest.

Usage:

```csharp
using NUnit.Framework;
using TestHelper.Attributes;
using UnityEngine;

[TestFixture]
public class MyTestClass
{
    [Test]
    [LoadScene("Assets/MyTests/Scenes/TestScene.unity")]
    public void MyTestMethod()
    {
        var cube = GameObject.Find("Cube in TestScene");
        Assert.That(cube, Is.Not.Null);
    }

    [Test]
    [LoadScene("Packages/YourPackageName/**/SampleScene.unity")]
    public void UsingGlobPattern()
    {
        // snip
    }

    [Test]
    [LoadScene("../../Scenes/SampleScene.unity")]
    public void UsingRelativePath()
    {
        // snip
    }
}
```

> [!NOTE]  
> - Load scene run after `OneTimeSetUp` and before `SetUp`. If you want to setup before loading Use [BuildSceneAttribute](#BuildScene) and [SceneManagerHelper](#SceneManagerHelper) method instead.
> - Scene file path is starts with `Assets/` or `Packages/` or `.`. And package name using `name` instead of `displayName`, when scenes in the package. (e.g., `Packages/com.nowsprinting.test-helper/Tests/Scenes/Scene.unity`)


#### BuildScene

`BuildSceneAttribute` is a NUnit test attribute class that build the scene before running the test on player.

It has the following benefits:

- Can be specified scenes that are **NOT** in "Scenes in Build".
- Can be specified scene path by [glob](https://en.wikipedia.org/wiki/Glob_(programming)) pattern. However, there are restrictions, top level and scene name cannot be omitted.
- Can be specified scene path by relative path from the test class file.

This attribute can attach to test method, test class (`TestFixture`), and test assembly.
It can be used with sync Tests, async Tests, and UnityTest.

Usage:

```csharp
using NUnit.Framework;
using TestHelper.Attributes;
using TestHelper.RuntimeInternals;
using UnityEngine;

[TestFixture]
public class MyTestClass
{
    [Test]
    [BuildScene("../../Scenes/SampleScene.unity")]
    public void MyTestMethod()
    {
        // Setup before load scene

        // Load scene
        await SceneManagerHelper.LoadSceneAsync("../../Scenes/SampleScene.unity");

        // Excercise the test
    }
}
```

> [!NOTE]
> - Scene file path is starts with `Assets/` or `Packages/` or `.`. And package name using `name` instead of `displayName`, when scenes in the package. (e.g., `Packages/com.nowsprinting.test-helper/Tests/Scenes/Scene.unity`)


#### TakeScreenshot

`TakeScreenshotAttribute` is an NUnit test attribute class to take a screenshot and save it to a file after running test.

Default save path is "`Application.persistentDataPath`/TestHelper/Screenshots/`CurrentTest.Name`.png".
You can specify the save directory and/or filename by arguments.
Directory can also be specified by command line arguments `-testHelperScreenshotDirectory`.

This attribute can attach to test method only.
Can be used with sync Test, async Test, and UnityTest.

Usage:
    
```csharp
using NUnit.Framework;
using TestHelper.Attributes;

[TestFixture]
public class MyTestClass
{
    [Test]
    [TakeScreenshot]
    public void MyTestMethod()
    {
        // Take screenshot after running the test.
    }
}
```

> [!WARNING]  
> - Do not attach to Edit Mode tests.
> - `GameView` must be visible. Use [FocusGameViewAttribute](#FocusGameView) or [GameViewResolutionAttribute](#GameViewResolution) if running on batch mode.

> [!NOTE]  
> If you want to take screenshots at any time, use the [ScreenshotHelper](#ScreenshotHelper) class.


#### TimeScale

`TimeScaleAttribute` is an NUnit test attribute class to change the [Time.timeScale](https://docs.unity3d.com/ScriptReference/Time-timeScale.html) during the test running.

This attribute can attach to test method only.
Can be used with sync Test, async Test, and UnityTest.

Usage:

```csharp
using NUnit.Framework;
using TestHelper.Attributes;

[TestFixture]
public class MyTestClass
{
    [Test]
    [TimeScale(2.0f)]
    public void MyTestMethod()
    {
        // Running at 2x speed.
    }
}
```


### Comparers

#### GameObjectNameComparer

`GameObjectNameComparer` is an NUnit test comparer class to compare GameObjects by name.

Usage:

```csharp
using NUnit.Framework;
using TestHelper.Comparers;
using UnityEngine;

[TestFixture]
public class GameObjectNameComparerTest
{
    [Test]
    public void UsingGameObjectNameComparer_CompareGameObjectsByName()
    {
        var actual = GameObject.FindObjectsOfType<GameObject>();
        Assert.That(actual, Does.Contain(new GameObject("test")).Using(new GameObjectNameComparer()));
    }
}
```


### Constraints

#### Destroyed

`DestroyedConstraint` tests that a `GameObject` is destroyed.

Usage:

```csharp
using NUnit.Framework;
using UnityEngine;
using Is = TestHelper.Constraints.Is;

[TestFixture]
public class MyTestClass
{
    [Test]
    public void DestroyImmediate_DestroyedGameObject()
    {
        var actual = GameObject.Find("Cube");
        GameObject.DestroyImmediate(actual);

        Assert.That(actual, Is.Destroyed);
    }
}
```

> [!NOTE]  
> When used with operators, use it in method style. e.g., `Is.Not.Destroyed()`


### RuntimeInternals

`TestHelper.RuntimeInternals` assembly can be used from the runtime code because it does not depend on test-framework.

> [!NOTE]
> The "Define Constraints" is set to `UNITY_INCLUDE_TESTS || COM_NOWSPRINTING_TEST_HELPER_ENABLE` in this assembly definition files, so it is generally excluded from release builds.
> To use the feature in release builds, add `COM_NOWSPRINTING_TEST_HELPER_ENABLE` to the "Define Symbols" at build time.

#### ScreenshotHelper

`ScreenshotHelper` is a utility class to take a screenshot and save it to a file.

Default save path is "`Application.persistentDataPath`/TestHelper/Screenshots/`CurrentTest.Name`.png".
(Replace `CurrentTest.Name` to caller method name when run in runtime context.)
You can specify the save directory and/or filename by arguments.
Directory can also be specified by command line arguments `-testHelperScreenshotDirectory`.

Usage:

```csharp
using System.Collections;
using NUnit.Framework;
using TestHelper.RuntimeInternals;
using UnityEngine.TestTools;

[TestFixture]
public class MyTestClass
{
    [UnityTest]
    public IEnumerator MyTestMethod()
    {
        yield return ScreenshotHelper.TakeScreenshot();
    }

    [Test]
    public async Task MyTestMethodAsync()
    {
        var coroutineRunner = new GameObject().AddComponent<CoroutineRunner>();
        await ScreenshotHelper.TakeScreenshot().ToUniTask(coroutineRunner);
    }

    private class CoroutineRunner : MonoBehaviour { }
}
```

> [!WARNING]  
> - Do not call from Edit Mode tests.
> - Must be called from main thread.
> - `GameView` must be visible. Use [FocusGameViewAttribute](#FocusGameView) or [GameViewResolutionAttribute](#GameViewResolution) if running on batch mode.
> - Files with the same name will be overwritten. Please specify filename argument when calling over twice in one method.
> - UniTask is required to be used from the async method. And also needs coroutineRunner (any MonoBehaviour) because TakeScreenshot method uses WaitForEndOfFrame inside. See more information: https://github.com/Cysharp/UniTask#ienumeratortounitask-limitation


#### SceneManagerHelper

`SceneManagerHelper` is a utility class to load the scene file.

It has the following benefits:

- Can be use same code for running Edit Mode tests, Play Mode tests in Editor, and on Player.
- Can be specified scene path by [glob](https://en.wikipedia.org/wiki/Glob_(programming)) pattern. However, there are restrictions, top level and scene name cannot be omitted.
- Can be specified scene path by relative path from the test class file.

Usage:

```csharp
using NUnit.Framework;
using TestHelper.RuntimeInternals;
using UnityEngine;

[TestFixture]
public class MyTestClass
{
    [Test]
    public void MyTestMethod()
    {
        // Setup before load scene

        // Load scene
        await SceneManagerHelper.LoadSceneAsync("../../Scenes/SampleScene.unity");
        
        // Excercise the test
    }
}
```

> [!NOTE]
> - Scene file path is starts with `Assets/` or `Packages/` or `.`. And package name using `name` instead of `displayName`, when scenes in the package. (e.g., `Packages/com.nowsprinting.test-helper/Tests/Scenes/Scene.unity`)
> - When loading the scene that is not in "Scenes in Build", use [BuildSceneAttribute](#BuildScene).


### Editor Extensions

#### Open Persistent Data Directory

Select **Window** > **Open Persistent Data Directory**, which opens the directory pointed to by [Application.persistentDataPath](https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html) in the Finder/ File Explorer.



## Installation

You can choose from two typical installation methods.

### Install via Package Manager window

1. Open the **Package Manager** tab in Project Settings window (**Editor > Project Settings**)
2. Click **+** button under the **Scoped Registries** and enter the following settings (figure 1.):
    1. **Name:** `package.openupm.com`
    2. **URL:** `https://package.openupm.com`
    3. **Scope(s):** `com.nowsprinting`
3. Open the Package Manager window (**Window > Package Manager**) and select **My Registries** in registries drop-down list (figure 2.)
4. Click **Install** button on the `com.nowsprinting.test-helper` package

**Figure 1.** Package Manager tab in Project Settings window.

![](Documentation~/ProjectSettings_Dark.png#gh-dark-mode-only)
![](Documentation~/ProjectSettings_Light.png#gh-light-mode-only)

**Figure 2.** Select registries drop-down list in Package Manager window.

![](Documentation~/PackageManager_Dark.png/#gh-dark-mode-only)
![](Documentation~/PackageManager_Light.png/#gh-light-mode-only)


### Install via OpenUPM-CLI

If you installed [openupm-cli](https://github.com/openupm/openupm-cli), run the command below:

```bash
openupm add com.nowsprinting.test-helper
```


### Add assembly reference

1. Open your test assembly definition file (.asmdef) in **Inspector** window
2. Add **TestHelper** into **Assembly Definition References**



## License

MIT License



## How to contribute

Open an issue or create a pull request.

Be grateful if you could label the PR as `enhancement`, `bug`, `chore`, and `documentation`.
See [PR Labeler settings](.github/pr-labeler.yml) for automatically labeling from the branch name.



## How to development

Add this repository as a submodule to the Packages/ directory in your project.

```bash
git submodule add https://github.com/nowsprinting/test-helper.git Packages/com.nowsprinting.test-helper
```

> [!WARNING]  
> Required install packages for running tests (when adding to the `testables` in package.json), as follows:
> - [UniTask](https://github.com/Cysharp/UniTask) package v2.3.3 or later

Generate a temporary project and run tests on each Unity version from the command line.

```bash
make create_project
UNITY_VERSION=2019.4.40f1 make -k test
```



## Release workflow

Run **Actions > Create release pull request > Run workflow** and merge created pull request.
(Or bump version in package.json on default branch)

Then, Will do the release process automatically by [Release](.github/workflows/release.yml) workflow.
And after tagging, OpenUPM retrieves the tag and updates it.

Do **NOT** manually operation the following operations:

- Create a release tag
- Publish draft releases
