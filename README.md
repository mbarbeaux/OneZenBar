# One Zen Bar

[![Build (develop)](https://github.com/mbarbeaux/OneZenBar/actions/workflows/develop.yml/badge.svg?branch=develop)](https://github.com/mbarbeaux/OneZenBar/actions/workflows/develop.yml?query=branch%3Adevelop)
[![Release (main)](https://github.com/mbarbeaux/OneZenBar/actions/workflows/main.yml/badge.svg?branch=main)](https://github.com/mbarbeaux/OneZenBar/actions/workflows/main.yml?query=branch%3Amain)
[![Line coverage](https://mbarbeaux.github.io/OneZenBar/badge_linecoverage.svg)](https://mbarbeaux.github.io/OneZenBar/)

**One Zen Bar** is an [Xbox Game Bar](https://support.xbox.com/help/games-apps/game-setup-and-play/getting-started-with-game-bar) widget for Windows, built on .NET 10. It pairs a cross-platform .NET class library with a UWP widget that surfaces it inside the Game Bar overlay (open with **Win + G**).

> The coverage badge and the [interactive coverage report](https://mbarbeaux.github.io/OneZenBar/) are published to GitHub Pages from `develop` (once the repository is public and the `COVERAGE_PAGES_ENABLED` repository variable is set).

## Download

Get it from the Microsoft Store:

<!-- Official Microsoft Store badge (get.microsoft.com). The <picture> swaps the badge per theme:
     the light/white badge on dark backgrounds, the dark badge on light ones. -->
<a href="https://apps.microsoft.com/detail/9PFCS5X4W77L">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="https://get.microsoft.com/images/en-us%20light.svg">
    <img src="https://get.microsoft.com/images/en-us%20dark.svg" width="220" alt="Get it from the Microsoft Store">
  </picture>
</a>

Alternatively, download the signed `.msixbundle` from the [Releases page](https://github.com/mbarbeaux/OneZenBar/releases) and sideload it — each release ships the bundle, the public certificate to trust and step-by-step install instructions.

## Project layout

- `src/OneZenBar.Core` — `netstandard2.0` class library holding the shared logic (AnyCPU; builds on every OS).
- `src/OneZenBar.Widget` — the UWP Xbox Game Bar widget (x86/x64/ARM64). **Windows-only**, and localized in English, French, Spanish and German.
- `tests/` — tests for the class library.

## Prerequisites

- [.NET SDK 10.0](https://dotnet.microsoft.com/download/dotnet/10.0) — builds the class library and tests on any OS.
- To build the **widget**: Windows with Visual Studio and the *Universal Windows Platform* workload (Windows 10 SDK). The widget targets Windows 10 2004+ and is excluded from the "Any CPU" solution platform, so `dotnet build` / `dotnet test` keep working on macOS and Linux (the widget is simply skipped there).

With [mise](https://mise.jdx.dev/) (recommended), the right .NET version is installed and selected automatically:

```bash
mise install
```

## Getting started

```bash
# One-time setup (installs git hooks; also runs automatically at first build)
dotnet tool restore
dotnet husky install

# Class library + tests (the Windows-only widget is skipped off-Windows)
dotnet build
dotnet test
```

To build or run the widget itself, open `OneZenBar.slnx` in Visual Studio on Windows (with the UWP workload) and deploy the `OneZenBar.Widget` project; it then appears in the Xbox Game Bar.

## License

[GPL-3.0](LICENSE)
