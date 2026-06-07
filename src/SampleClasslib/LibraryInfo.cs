// Copyright (C) 2026 Michael Barbeaux. Licensed under the GNU General Public License v3.0 or later. See the LICENSE file for details.

using System.Reflection;

namespace SampleClasslib;

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
    /// </summary>
    internal static string GetVersion(Assembly assembly)
    {
        AssemblyInformationalVersionAttribute? attribute =
            assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        return attribute?.InformationalVersion ?? "unknown";
    }
}