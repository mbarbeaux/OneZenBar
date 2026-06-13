// Copyright (C) 2026 Michael Barbeaux. Licensed under the MIT License. See the LICENSE file for details.

using System.Reflection;

namespace OneZenBar.Core;

/// <summary>
/// Exposes build-time information about this library.
/// </summary>
public static class LibraryInfo
{
    /// <summary>
    /// Returns the version of this library as stamped by MinVer at build time
    /// (the informational version, e.g. "0.1.0" on a release or "0.1.1-alpha.0.5" between releases).
    /// </summary>
    /// <returns>The library version, or "unknown" when the attribute is missing.</returns>
    public static string GetVersion()
    {
        return GetVersion(typeof(LibraryInfo).Assembly);
    }

    /// <summary>
    /// Testable core of <see cref="GetVersion()"/>: reads the informational version of the given assembly.
    /// The SemVer build metadata (everything after the first '+', i.e. the git commit id appended by the
    /// .NET SDK) is stripped so the displayed version stays clean (e.g. "0.3.0", not "0.3.0+abc1234").
    /// The prerelease suffix (e.g. "-alpha.0.5") is preserved.
    /// </summary>
    internal static string GetVersion(Assembly assembly)
    {
        AssemblyInformationalVersionAttribute? attribute =
            assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        if (attribute is null)
        {
            return "unknown";
        }

        string version = attribute.InformationalVersion;
        int metadataSeparator = version.IndexOf('+');
        return metadataSeparator >= 0 ? version.Substring(0, metadataSeparator) : version;
    }
}