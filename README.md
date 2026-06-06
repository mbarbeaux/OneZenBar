# Sample

.NET 10 project — basic skeleton.

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
