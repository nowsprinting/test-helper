# Test helper library for Unity Test Framework

[![Meta file check](https://github.com/nowsprinting/test-helper/actions/workflows/metacheck.yml/badge.svg)](https://github.com/nowsprinting/test-helper/actions/workflows/metacheck.yml)
[![Test](https://github.com/nowsprinting/test-helper/actions/workflows/test.yml/badge.svg)](https://github.com/nowsprinting/test-helper/actions/workflows/test.yml)
[![openupm](https://img.shields.io/npm/v/com.nowsprinting.test-helper?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.nowsprinting.test-helper/)

Provides attributes and constraints useful for testing.

Required Unity 2019 LTS or later.



## Features

### Attributes

#### FocusGameView

`FocusGameViewAttribute` is an NUnit test attribute class to focus `GameView` or `SimulatorWindow` before run test.

This attribute can attached to test method, test class (`TestFixture`) and test assembly.

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
        // e.g., test using InputEventTrace of Input System package.
    }
}
```

> **Note**  
> In batchmode, open `GameView` window.

#### GameViewResolution

`GameViewResolutionAttribute` is an NUnit test attribute class to set custom resolution to `GameView` before run test.

This attribute can attached to test method, test class (`TestFixture`) and test assembly.

Usage:

```csharp
using NUnit.Framework;
using TestHelper.Attributes;

[TestFixture]
public class MyTestClass
{
    [UnityTest]
    [GameViewResolution(640, 480, "VGA")]
    public IEnumerator MyTestMethod()
    {
        yield return null; // wait for one frame to apply resolution.

        // e.g., test using GraphicRaycaster.
    }
}
```

> **Warning**  
> Wait for one frame to apply resolution.

> **Note**  
> In batchmode, open `GameView` window.

#### IgnoreBatchMode

`IgnoreBatchModeAttribute` is an NUnit test attribute class to skip test execution when run tests with `-batchmode` from the commandline.

This attribute can attached to test method, test class (`TestFixture`) and test assembly.

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
        // e.g., test needs to take a screenshot.

        yield return new WaitForEndOfFrame();
        ImageAssert.AreEqual(expectedTexture, Camera.main, settings);
    }
}
```

#### IgnoreWindowMode

`IgnoreWindowModeAttribute` is an NUnit test attribute class to skip test execution when run tests on Unity editor window.

This attribute can attached to test method, test class (`TestFixture`) and test assembly.

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

> **Note**  
> When used with operators, use it in method style. e.g., `Is.Not.Destroyed()`



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

Run the command below:

```bash
git submodule add https://github.com/nowsprinting/test-helper.git Packages/com.nowsprinting.test-helper
```



## Release workflow

Run **Actions > Create release pull request > Run workflow** and merge created pull request.
(Or bump version in package.json on default branch)

Then, Will do the release process automatically by [Release](.github/workflows/release.yml) workflow.
And after tagging, OpenUPM retrieves the tag and updates it.

Do **NOT** manually operation the following operations:

- Create a release tag
- Publish draft releases
