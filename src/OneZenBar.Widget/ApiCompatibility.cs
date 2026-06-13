// Copyright (C) 2026 Michael Barbeaux. Licensed under the MIT License. See the LICENSE file for details.

using Windows.Foundation.Metadata;

namespace OneZenBar.Widget
{
    /// <summary>
    /// Runtime guards for Windows APIs newer than the minimum supported OS.
    ///
    /// <para>
    /// This project COMPILES against Windows SDK 26100 (<c>TargetPlatformVersion</c>) but RUNS on
    /// Windows 10 2004 and later (<c>TargetPlatformMinVersion</c> = 10.0.19041.0, which ships
    /// <c>Windows.Foundation.UniversalApiContract</c> version 10). Every API introduced AFTER
    /// build 19041 is visible at compile time but MISSING at runtime on older systems — calling
    /// one there throws (TypeLoadException, MissingMethodException or InvalidCastException).
    /// </para>
    ///
    /// <para>
    /// RULE: before calling any API documented as "Introduced in" a build above 19041
    /// (check the "Windows requirements" section of the API's Microsoft Learn page),
    /// wrap the call in one of the guards below, and always provide a 19041 fallback path.
    /// </para>
    ///
    /// <para>
    /// Windows build → UniversalApiContract version:
    /// <list type="bullet">
    ///   <item>10.0.19041 (Windows 10 2004…22H2) → 10 — the baseline: NO guard needed</item>
    ///   <item>10.0.22000 (Windows 11 21H2) → 14</item>
    ///   <item>10.0.22621 (Windows 11 22H2/23H2) → 15</item>
    ///   <item>10.0.26100 (Windows 11 24H2) → 19</item>
    /// </list>
    /// </para>
    ///
    /// <example>
    /// Guard by contract version (preferred when the API's contract version is known):
    /// <code>
    /// if (ApiCompatibility.IsContractPresent(14))
    /// {
    ///     // Windows 11+ only API here
    /// }
    /// else
    /// {
    ///     // Windows 10 19041 fallback here
    /// }
    /// </code>
    /// Guard by member (preferred for one-off properties/methods added to existing types):
    /// <code>
    /// if (ApiCompatibility.IsMethodPresent("Windows.UI.ViewManagement.ApplicationView", "SomeNewMethod"))
    /// {
    ///     ApplicationView.GetForCurrentView().SomeNewMethod();
    /// }
    /// </code>
    /// </example>
    /// </summary>
    internal static class ApiCompatibility
    {
        /// <summary>
        /// UniversalApiContract version guaranteed by TargetPlatformMinVersion (10.0.19041.0).
        /// Anything at or below this version never needs a guard.
        /// </summary>
        public const ushort BaselineContractVersion = 10;

        /// <summary>
        /// True when the running OS provides at least the given major version of
        /// Windows.Foundation.UniversalApiContract.
        /// </summary>
        public static bool IsContractPresent(ushort majorVersion)
        {
            return ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", majorVersion);
        }

        /// <summary>True when the given WinRT type exists on the running OS.</summary>
        public static bool IsTypePresent(string fullTypeName)
        {
            return ApiInformation.IsTypePresent(fullTypeName);
        }

        /// <summary>True when the given method exists on the running OS.</summary>
        public static bool IsMethodPresent(string fullTypeName, string methodName)
        {
            return ApiInformation.IsMethodPresent(fullTypeName, methodName);
        }

        /// <summary>True when the given property exists on the running OS.</summary>
        public static bool IsPropertyPresent(string fullTypeName, string propertyName)
        {
            return ApiInformation.IsPropertyPresent(fullTypeName, propertyName);
        }

        /// <summary>True when the given enum value exists on the running OS.</summary>
        public static bool IsEnumNamedValuePresent(string fullEnumTypeName, string valueName)
        {
            return ApiInformation.IsEnumNamedValuePresent(fullEnumTypeName, valueName);
        }
    }
}
