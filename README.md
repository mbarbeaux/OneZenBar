# One Zen Bar

[![Build (develop)](https://github.com/mbarbeaux/OneZenBar/actions/workflows/develop.yml/badge.svg?branch=develop)](https://github.com/mbarbeaux/OneZenBar/actions/workflows/develop.yml?query=branch%3Adevelop)
[![Release (main)](https://github.com/mbarbeaux/OneZenBar/actions/workflows/main.yml/badge.svg?branch=main)](https://github.com/mbarbeaux/OneZenBar/actions/workflows/main.yml?query=branch%3Amain)

**One Zen Bar** is an [Xbox Game Bar](https://support.xbox.com/help/games-apps/game-setup-and-play/getting-started-with-game-bar) widget for Windows, built on .NET 10. The UWP widget surfaces inside the Game Bar overlay (open with **Win + G**) and builds on the `OneZenBar.Core` business-logic library, distributed as a separate package.

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

- `src/OneZenBar.Widget` — the UWP Xbox Game Bar widget (x86/x64/ARM64). **Windows-only**, and localized in English, French, Spanish and German. This is the only buildable application in the repo.
- `src/OneZenBar.Version` — a tiny SDK-style "version anchor" project (ships nothing). The old-style UWP project cannot run MinVer, so it reads the repo version through this project to stamp the appx package version.
- `OneZenBar.Core` — the shared business logic, consumed as a separate NuGet package (not part of this repo).

## Prerequisites

- [.NET SDK 10.0](https://dotnet.microsoft.com/download/dotnet/10.0).
- Windows with Visual Studio and the *Universal Windows Platform* workload (Windows 10 SDK) to build the widget — it targets Windows 10 2004+ and is excluded from the "Any CPU" solution platform.

With [mise](https://mise.jdx.dev/) (recommended), the right .NET version is installed and selected automatically:

```bash
mise install
```

## Getting started

Open `OneZenBar.slnx` in Visual Studio on Windows (with the UWP workload), then deploy the `OneZenBar.Widget` project — it appears in the Xbox Game Bar (**Win + G**).

Restoring the widget pulls the private `OneZenBar.Core` package from GitHub Packages, which needs a credential: set the `GH_PACKAGES_PAT` environment variable to a fine-grained personal access token with the `read:packages` scope before building (`nuget.config` reads it).

## License

[MIT](LICENSE)
