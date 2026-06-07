// Copyright (C) 2026 Michael Barbeaux. Licensed under the GNU General Public License v3.0 or later. See the LICENSE file for details.

namespace SampleClasslib.Tests;

public class LibraryInfoTests
{
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
}