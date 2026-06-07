# Sample

[![Build (develop)](https://github.com/mbarbeaux/sample/actions/workflows/develop.yml/badge.svg?branch=develop)](https://github.com/mbarbeaux/sample/actions/workflows/develop.yml?query=branch%3Adevelop)
[![Release (main)](https://github.com/mbarbeaux/sample/actions/workflows/main.yml/badge.svg?branch=main)](https://github.com/mbarbeaux/sample/actions/workflows/main.yml?query=branch%3Amain)
[![Line coverage](https://mbarbeaux.github.io/sample/badge_linecoverage.svg)](https://mbarbeaux.github.io/sample/)

.NET 10 project — basic skeleton.

> The coverage badge and the [interactive coverage report](https://mbarbeaux.github.io/sample/) are published to GitHub Pages from `develop` (once the repository is public and the `COVERAGE_PAGES_ENABLED` repository variable is set).

## Prerequisites

- [.NET SDK 10.0](https://dotnet.microsoft.com/download/dotnet/10.0)

With [mise](https://mise.jdx.dev/) (recommended), the right version is installed and selected automatically:

```bash
mise install
```

## Getting started

```bash
# One-time setup (installs git hooks; also runs automatically at first build)
dotnet tool restore
dotnet husky install

dotnet build
```

## License

[GPL-3.0](LICENSE)
