# Test helper library for Unity Test Framework

[![Meta file check](https://github.com/nowsprinting/test-helper/actions/workflows/metacheck.yml/badge.svg)](https://github.com/nowsprinting/test-helper/actions/workflows/metacheck.yml)
[![Test](https://github.com/nowsprinting/test-helper/actions/workflows/test.yml/badge.svg)](https://github.com/nowsprinting/test-helper/actions/workflows/test.yml)
[![openupm](https://img.shields.io/npm/v/com.nowsprinting.test-helper?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.nowsprinting.test-helper/)

Provides custom attributes, comparers, and constraints useful for testing with Unity Test Framework.

Required Unity 2019 LTS or later.



## Features

### Attributes

#### FocusGameView

`FocusGameViewAttribute` is an NUnit test attribute class to focus `GameView` or `SimulatorWindow` before running the test.

This attribute can attach to test method, test class (`TestFixture`), and test assembly.
Can be used with sync Test, async Test, and UnityTest.

Usage:

```csharp
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

`GameViewResolutionAttribute` is an NUnit test attribute class to set custom resolution to `GameView` before running the test.

This attribute can attach to test method, test class (`TestFixture`), and test assembly.
Can be used with async Test and UnityTest.

Usage:

```csharp
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
[TestFixture]
public class MyTestClass
{
    [Test]
    [GizmosShowOnGameView(true)]
    public void MyTestMethod()
    {
        // Show Gizmos on GameView during the test running.
    }
}
```

> [!NOTE]  
> In batchmode, open `GameView` window.


#### IgnoreBatchMode

`IgnoreBatchModeAttribute` is an NUnit test attribute class to skip the test execution when run tests with `-batchmode` from the commandline.

This attribute can attach to test method, test class (`TestFixture`), and test assembly.
Can be used with sync Test, async Test, and UnityTest.

Usage:

```csharp
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

`IgnoreWindowModeAttribute` is an NUnit test attribute class to skip the test execution when run tests on Unity editor window.

This attribute can attach to test method, test class (`TestFixture`), and test assembly.
Can be used with sync Test, async Test, and UnityTest.

Usage:

```csharp
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

`UnityVersionAttribute` is an NUnit test attribute class to skip the test run if Unity version is older and/or newer than specified.

This attribute can attach to test method, test class (`TestFixture`), and test assembly.
Can be used with sync Test, async Test, and UnityTest.

Usage:

```csharp
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

`CreateSceneAttribute` is an NUnit test attribute class to create a new scene before running the test.

It has the following benefits:

- Can be use same code for running Edit Mode tests, Play Mode tests in Editor, and on Player

This attribute can attach to test method only.
Can be used with sync Test, async Test, and UnityTest.

Usage:

```csharp
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
> This process runs after `OneTimeSetUp` and before `SetUp`

> [!NOTE]  
> Create or not `Main Camera` and `Directional Light` can be specified with parameters (default is not create)


#### LoadScene

`LoadSceneAttribute` is a NUnit test attribute class that loads a scene before running the test.

It has the following benefits:

- Can be use same code for running Edit Mode tests, Play Mode tests in Editor, and on Player.
- Can be specified scenes that are **NOT** in "Scenes in Build".
- Can be specified scene path by [glob](https://en.wikipedia.org/wiki/Glob_(programming)) pattern. However, there are restrictions, top level and scene name cannot be omitted.
- Can be specified scene path by relative path from the test class file.

This attribute can attach to the test method only.
It can be used with sync Tests, async Tests, and UnityTest.

Usage:

```csharp
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
> This process runs after `OneTimeSetUp` and before `SetUp`.
> If you want to load during `SetUp` and testing, use [BuildSceneAttribute](#BuildScene) and [SceneManagerHelper](#SceneManagerHelper) method instead.

> [!NOTE]  
> Scene file path is starts with `Assets/` or `Packages/` or `.`. And package name using `name` instead of `displayName`, when scenes in the package. (e.g., `Packages/com.nowsprinting.test-helper/Tests/Scenes/Scene.unity`)


#### BuildScene

`BuildSceneAttribute` is a NUnit test attribute class that build a scene before running the test on player.

It has the following benefits:

- Can be specified scenes that are **NOT** in "Scenes in Build".
- Can be specified scene path by [glob](https://en.wikipedia.org/wiki/Glob_(programming)) pattern. However, there are restrictions, top level and scene name cannot be omitted.
- Can be specified scene path by relative path from the test class file.

This attribute can attach to test method, test class (`TestFixture`), and test assembly.
It can be used with sync Tests, async Tests, and UnityTest.

Usage:

```csharp
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
> Scene file path is starts with `Assets/` or `Packages/` or `.`. And package name using `name` instead of `displayName`, when scenes in the package. (e.g., `Packages/com.nowsprinting.test-helper/Tests/Scenes/Scene.unity`)


#### TakeScreenshot

`TakeScreenshotAttribute` is an NUnit test attribute class to take a screenshot and save it to a file after running the test.

Default save path is "`Application.persistentDataPath`/TestHelper/Screenshots/`CurrentTest.Name`.png".
You can specify the save directory and/or filename by arguments.
Directory can also be specified by command line arguments `-testHelperScreenshotDirectory`.

This attribute can attach to test method only.
Can be used with sync Test, async Test, and UnityTest.

Usage:
    
```csharp
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
> Do not attach to Edit Mode tests.

> [!WARNING]  
> `GameView` must be visible. Use [FocusGameViewAttribute](#FocusGameView) or [GameViewResolutionAttribute](#GameViewResolution) if running on batchmode.

> [!NOTE]  
> If you want to take screenshots at any time, use the [ScreenshotHelper](#ScreenshotHelper) class.


#### TimeScale

`TimeScaleAttribute` is an NUnit test attribute class to change the [Time.timeScale](https://docs.unity3d.com/ScriptReference/Time-timeScale.html) during the test running.

This attribute can attach to test method only.
Can be used with sync Test, async Test, and UnityTest.

Usage:

```csharp
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

`GameObjectNameComparer` is a NUnit test comparer class that compares two `GameObject` by name.

Usage:

```csharp
[TestFixture]
public class MyTestClass
{
    [Test]
    public void MyTestMethod()
    {
        var actual = GameObject.FindObjectsOfType<GameObject>();
        Assert.That(actual, Does.Contain(new GameObject("test")).Using(new GameObjectNameComparer()));
    }
}
```


#### XDocumentComparer

`XDocumentComparer` is a NUnit test comparer class that loosely compares `XDocument`.

It only compares the attributes and values of each element in the document unordered.
XML declarations and comments are ignored.

Usage:

```csharp
[TestFixture]
public class MyTestClass
{
    [Test]
    public void MyTestMethod()
    {
        var x = XDocument.Parse(@"<root><child>value1</child><child attribute=""attr"">value2</child></root>");
        var y = XDocument.Parse(@"<?xml version=""1.0"" encoding=""utf-8""?>
<root><!-- comment --><child attribute=""attr"">value2</child><!-- comment --><child>value1</child></root>");
        // with XML declaration, comments, and different order

        Assert.That(x, Is.EqualTo(y).Using(new XDocumentComparer()));
    }
}
```


#### XmlComparer

`XmlComparer` is a NUnit test comparer class that compares two `string` as an XML document.

It only compares the attributes and values of each element in the document unordered.
XML declarations and comments are ignored, and white spaces, tabs, and newlines before and after the value are ignored.

Usage:

```csharp
[TestFixture]
public class MyTestClass
{
    [Test]
    public void MyTestMethod()
    {
        var x = @"<root><child>value1</child><child attribute=""attr"">value2</child></root>";
        var y = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <!-- comment -->
  <child attribute=""attr"">
    value2
  </child>
  <!-- comment -->
  <child>
    value1
  </child>
</root>";

        Assert.That(x, Is.EqualTo(y).Using(new XmlComparer()));
    }
}
```


### Constraints

#### Destroyed

`DestroyedConstraint` tests that a `GameObject` is destroyed.

Usage:

```csharp
using Is = TestHelper.Constraints.Is;

[TestFixture]
public class MyTestClass
{
    [Test]
    public void MyTestMethod()
    {
        var actual = GameObject.Find("Cube");
        GameObject.DestroyImmediate(actual);

        Assert.That(actual, Is.Destroyed);
    }
}
```

> [!NOTE]  
> When used with operators, use it in method style. e.g., `Is.Not.Destroyed()`


### Runtime APIs

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
> Do not attach to Edit Mode tests.
> And must be called from main thread.

> [!WARNING]  
> `GameView` must be visible. Use [FocusGameViewAttribute](#FocusGameView) or [GameViewResolutionAttribute](#GameViewResolution) if running on batchmode.

> [!WARNING]  
> Files with the same name will be overwritten. Please specify filename argument when calling over twice in one method.

> [!WARNING]  
> `UniTask` is required to be used from the async method. And also needs coroutineRunner (any MonoBehaviour) because TakeScreenshot method uses `WaitForEndOfFrame` inside. See more information: https://github.com/Cysharp/UniTask#ienumeratortounitask-limitation


#### SceneManagerHelper

`SceneManagerHelper` is a utility class to load the scene file.

It has the following benefits:

- Can be use same code for running Edit Mode tests, Play Mode tests in Editor, and on Player.
- Can be specified scene path by [glob](https://en.wikipedia.org/wiki/Glob_(programming)) pattern. However, there are restrictions, top level and scene name cannot be omitted.
- Can be specified scene path by relative path from the test class file.

Usage:

```csharp
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
> Scene file path is starts with `Assets/` or `Packages/` or `.`. And package name using `name` instead of `displayName`, when scenes in the package. (e.g., `Packages/com.nowsprinting.test-helper/Tests/Scenes/Scene.unity`)

> [!NOTE]  
> When loading the scene that is not in "Scenes in Build", use [BuildSceneAttribute](#BuildScene).


### Editor Extensions

#### Open Persistent Data Directory

Select menu item **Window > Test Helper > Open Persistent Data Directory**, which opens the directory pointed to by [Application.persistentDataPath](https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html) in the Finder/ File Explorer.

#### Open Temporary Cache Directory

Select menu item **Window > Test Helper > Open Temporary Cache Directory**, which opens the directory pointed to by [Application.temporaryCachePath](https://docs.unity3d.com/ScriptReference/Application-temporaryCachePath.html) in the Finder/ File Explorer.


### JUnit XML format report

If you specify path with `-testHelperJUnitResults` command line option, the test result will be written in JUnit XML format when the tests are finished.

> [!NOTE]  
> The JUnit XML format is the so-called "Legacy." It does not support the "Open Test Reporting format" introduced in JUnit 5.



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
