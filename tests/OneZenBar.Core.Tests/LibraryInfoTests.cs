// Copyright (C) 2026 Michael Barbeaux. Licensed under the MIT License. See the LICENSE file for details.

using System.Reflection;
using System.Reflection.Emit;

namespace OneZenBar.Core.Tests;

public class LibraryInfoTests
{
    [Fact]
    public void GetVersion_ReturnsUnknown_WhenInformationalVersionAttributeIsMissing()
    {
        // A dynamically emitted assembly carries no AssemblyInformationalVersionAttribute
        AssemblyBuilder assemblyWithoutAttribute = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName("OneZenBar.Core.Tests.NoAttributes"), AssemblyBuilderAccess.Run);

        string version = LibraryInfo.GetVersion(assemblyWithoutAttribute);

        Assert.Equal("unknown", version);
    }

    [Fact]
    public void GetVersion_ReturnsNonEmptyValue()
    {
        string version = LibraryInfo.GetVersion();

        Assert.False(string.IsNullOrWhiteSpace(version));
    }

    [Fact]
    public void GetVersion_StartsWithSemverCore()
    {
        string version = LibraryInfo.GetVersion();

        // MinVer always produces Major.Minor.Patch, optionally followed by a prerelease suffix
        Assert.Matches(@"^\d+\.\d+\.\d+", version);
    }

    [Fact]
    public void GetVersion_StripsBuildMetadata()
    {
        // The .NET SDK appends the git commit id as SemVer build metadata ("+<sha>") to the
        // informational version; it must not leak into the displayed version.
        string version = LibraryInfo.GetVersion();

        Assert.DoesNotContain('+', version);
    }

    [Theory]
    [InlineData("1.2.3+abc1234", "1.2.3")]                      // build metadata present -> stripped
    [InlineData("1.2.3", "1.2.3")]                              // no metadata -> returned unchanged
    [InlineData("1.2.3-alpha.0.5+def5678", "1.2.3-alpha.0.5")]  // prerelease kept, metadata stripped
    [InlineData("1.2.3-alpha.0.5", "1.2.3-alpha.0.5")]          // prerelease kept, no metadata
    public void GetVersion_StripsOnlyBuildMetadata(string informationalVersion, string expected)
    {
        // Drive BOTH branches of the metadata-stripping ternary deterministically: the real
        // assembly only exercises one of them (whichever the current build's version matches).
        Assembly assembly = AssemblyWithInformationalVersion(informationalVersion);

        Assert.Equal(expected, LibraryInfo.GetVersion(assembly));
    }

    private static Assembly AssemblyWithInformationalVersion(string value)
    {
        AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName($"OneZenBar.Core.Tests.Info_{Guid.NewGuid():N}"), AssemblyBuilderAccess.Run);
        ConstructorInfo constructor =
            typeof(AssemblyInformationalVersionAttribute).GetConstructor(new[] { typeof(string) })!;
        assembly.SetCustomAttribute(new CustomAttributeBuilder(constructor, new object[] { value }));
        return assembly;
    }
}