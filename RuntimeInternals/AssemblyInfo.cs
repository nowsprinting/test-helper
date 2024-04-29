// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle("Internal implementation for the TestHelper package")]
[assembly: AssemblyDescription(
    @"This assembly can be used from the runtime code because it does not depend on test-framework.
This assembly is named ""Internal"", however, the included classes are public.")]

[assembly: InternalsVisibleTo("TestHelper.Editor")]
[assembly: InternalsVisibleTo("TestHelper.RuntimeInternals.Tests")]
