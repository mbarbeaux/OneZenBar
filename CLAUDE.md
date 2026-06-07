# Sample

.NET 10 project.

## Stack

- .NET SDK 10.0 — version pinned in **both** `mise.toml` (for [mise](https://mise.jdx.dev/) users, `mise install` sets everything up) and `global.json` (for CI and non-mise setups). Keep the two in sync when upgrading.
- Solution: `Sample.slnx` (XML solution format — readable, no GUIDs, merge-friendly)
- Projects: `src/SampleClasslib` (netstandard2.0, AnyCPU) and `src/Sample.Widget` (UWP Xbox Game Bar widget, old-style csproj, x86/x64/ARM64 only)
- **`Sample.Widget` builds ONLY on Windows with the Visual Studio UWP workload** (Windows 10 SDK 19041). It is excluded from the "Any CPU" solution platform in `Sample.slnx`, so `dotnet build`/`restore`/`format`/`test` keep working on macOS/Linux and in CI (the project is skipped). Its XAML targets import is guarded by `Exists()` so the project still evaluates everywhere. The appx package version is stamped from MinVer by the `StampAppxManifestVersion` target when building with `/p:StampPackageVersion=true` (Major.Minor.Patch.0 — appx forbids prerelease suffixes); `Package.appxmanifest` carries neutral dev placeholders — the real Microsoft Store identity (Partner Center > Product identity) is **deliberately never committed**: CI injects it at build time from the `STORE_IDENTITY_NAME`/`STORE_IDENTITY_PUBLISHER`/`STORE_PUBLISHER_DISPLAY_NAME`/`STORE_APP_DISPLAY_NAME` secrets (`StampAppxStoreIdentity` target). Do not write those values into any tracked file or commit message.
- **Widget signing**: the CI msixbundle is signed with the self-signed dev certificate when BOTH the `SIGNING_CERTIFICATE_BASE64`/`SIGNING_CERTIFICATE_PASSWORD` secrets AND the Store identity secrets exist (the certificate Subject equals the injected — secret — Store Publisher, so signing without the identity would mismatch; otherwise the bundle stays unsigned, also the case on Dependabot PRs which cannot read secrets). Installing the bundle requires the public `.cer` in the target machine's `LocalMachine\TrustedPeople` store. Self-signed = dev/sideload only; Store submission re-signs with Microsoft's certificate.
- **Widget API compatibility rule**: the widget compiles against Windows SDK 26100 (`TargetPlatformVersion`) but must run on Windows 10 2004 (`TargetPlatformMinVersion` 10.0.19041.0 = UniversalApiContract v10). Any API introduced after build 19041 compiles but **crashes at runtime** on older systems — ALWAYS guard such calls with the `ApiCompatibility` helper (`src/Sample.Widget/ApiCompatibility.cs`, wraps `ApiInformation`) and provide a 19041 fallback. Check the "Windows requirements" section on the API's Microsoft Learn page; contract map: 19041→10 (baseline, no guard), 22000→14, 22621→15, 26100→19.

## Commands

```bash
# One-time setup
mise install          # Install the toolchain (.NET SDK) at the pinned versions
dotnet tool restore   # Required before committing — installs git hook tools (also runs automatically at first build)
dotnet husky install

dotnet build          # Build the solution
dotnet test           # Run tests
dotnet test /p:CollectCoverage=true   # Run tests with coverage — fails below the threshold set in tests/Directory.Build.props (CI always runs this)
dotnet format         # Format code (the pre-commit hook runs this with --verify-no-changes --severity error)
```

## Conventions

- **English only across the entire project**: documentation, comments, identifiers (variables, functions, classes, namespaces), commit messages, config files. No French anywhere in the repository.
- Commit messages **must follow Conventional Commits** — the `commit-msg` hook runs `dotnet commit-linter` (EasyBuild.CommitLinter, default rules: `feat`, `fix`, `ci`, `chore`, `docs`, `test`, `style`, `refactor`, `perf`, `revert`, `build`).
- Code style is defined in `.editorconfig` — always follow it. The pre-commit hook rejects staged `.cs`/`.vb` files that `dotnet format --verify-no-changes --severity error` would change; run `dotnet format` before committing.
- **Encoding**: all text files are UTF-8 without BOM. Enforced at commit time by a Husky.Net pre-commit hook (`.husky/csx/check-encoding.csx`), installed automatically at first restore/build, or manually with `dotnet tool restore && dotnet husky install`.
- **Line endings**: LF everywhere on every OS — normalized by `.gitattributes`, don't override it locally.
- **License**: project is GPL-3.0-or-later. Every `.cs` file MUST start with this header (the build fails otherwise — rule IDE0073):
  ```csharp
  // Copyright (C) 2026 Michael Barbeaux. Licensed under the GNU General Public License v3.0 or later. See the LICENSE file for details.
  ```
  `dotnet format` automatically adds a missing header.
- Add any new project to the solution: `dotnet sln add <path/Project.csproj>`
- **After scaffolding with `dotnet new`, always run `dotnet format`**: templates generate files with a UTF-8 BOM and without the license header — both rejected by the pre-commit hook. `dotnet format` fixes everything in one pass (strips BOMs, adds the header, applies `.editorconfig`).
- Target structure:
  - `src/` — application projects
  - `tests/` — test projects

## Releases

- Branching follows GitFlow: work happens on `develop` and `feature/*`; releases go through `release/x.y.z` branches merged to `main`.
- `CHANGELOG.md` is generated by [EasyBuild.ShipIt](https://github.com/easybuild-org/EasyBuild.ShipIt) from Conventional Commits — never edit it manually (only the front matter at the top is editable). It also computes the next semver version.
- Assembly versions are stamped at build time by [MinVer](https://github.com/adamralph/minver) (configured in `Directory.Build.props`) from the latest reachable `vX.Y.Z` tag created by ShipIt — **never hardcode `Version`/`AssemblyVersion`/`FileVersion` in a csproj**. Between releases, builds get a pre-release version with commit height (`X.Y.Z-alpha.0.N`); with no tag yet, `0.0.0-alpha.0.N`. CI checkouts need `fetch-depth: 0` (history + tags) for MinVer to see the tags.
- The release is automated by CI (`.github/workflows/main.yml`): on every push to `main` (i.e. when a `release/*` or `hotfix/*` branch is merged), the workflow runs ShipIt, commits `chore: release X.Y.Z` directly to `main` through the GitHub API (commit shows as Verified), tags it `vX.Y.Z`, and fast-forwards `develop` to `main`. Do NOT run ShipIt, tag, or back-merge manually.
- If `develop` has diverged from `main` (features merged during the release), the fast-forward fails with a warning — rebase locally: `git checkout develop && git rebase origin/main && git push --force-with-lease` (never back-merge in CI: rebased commits would be unsigned).
- CI workflows: `build.yml` (PRs to `develop`/`main` only — no push trigger on feature or release branches, open a PR to get CI) and `develop.yml` (push to `develop`) run the same quality gate (`dotnet format --verify-no-changes --severity error`, build, tests with coverage), shared via the reusable workflow `quality-gate.yml` (`workflow_call`) — change the gate there, not in the callers. Tests run with `--logger GitHubActions` (failure annotations + report in the job summary) and `/p:CollectCoverage=true` — coverlet.msbuild **fails the gate if line coverage drops below the `Threshold` set in `tests/Directory.Build.props`** (80%, inherited by every test project under `tests/`, along with the coverlet/logger packages — new test projects get all of it for free). ReportGenerator (local dotnet tool) renders the coverage summary into the job summary, and a sticky PR comment (`marocchino/sticky-pull-request-comment`) keeps it visible on pull requests. The full interactive HTML report is uploaded as the `coverage-report` artifact on every run; the raw cobertura files as `coverage-data`. Note: neither `env` nor secrets propagate from caller to reusable workflow — the CI env vars (`HUSKY: 0`, etc.) live in `quality-gate.yml`, and both callers pass `secrets: inherit` (required by the MSIX signing secrets).
- Microsoft Store submission: the `store-upload` job in `main.yml` runs after the release tag is created, builds the `.msixupload` (StoreUpload mode, unsigned — the Store re-signs) with the injected Store identity and MinVer version, and submits it via the `microsoft/microsoft-store-apppublisher` CLI action. Auth uses the `AAD_TENANT_ID`/`AAD_CLIENT_ID`/`AAD_CLIENT_SECRET`/`PARTNER_CENTER_SELLER_ID`/`STORE_APP_ID` secrets. Gated behind the `STORE_UPLOAD_ENABLED` repository variable — enable it only AFTER the first manual submission in Partner Center (the API updates packages but cannot create the initial listing/age rating).
- Coverage GitHub Pages site: the `coverage-pages` job in `develop.yml` publishes the HTML report (with trend charts, history persisted via `actions/cache`) and the SVG badges referenced by the README. It is skipped until the `COVERAGE_PAGES_ENABLED` repository variable is `true` — set it once the repo is public and Pages is enabled (Settings > Pages > Source: GitHub Actions).
- PRs are merged by **fast-forward only** (no merge commits, SHAs and signatures preserved): comment `/fast-forward` on the PR — the `fast-forward.yml` workflow (sequoia-pgp/fast-forward) pushes the target branch to the PR head. Never use GitHub's merge buttons ("Rebase and merge" rewrites SHAs and drops signatures; merge/squash create new commits). `fast-forward-check.yml` comments on the PR when fast-forwarding is impossible (then rebase the PR branch onto its target first).
- The fast-forward push uses the `FAST_FORWARD_PAT` secret (fine-grained PAT scoped to this repo with **Contents: read/write** AND **Pull requests: read/write** — the action reads PR metadata and posts result comments through the API, so Contents alone fails with an empty `clone_url`). The default `GITHUB_TOKEN` would not trigger `main.yml`/`develop.yml` after the merge.
- ShipIt details: `--skip-merge-commit` is required (GitFlow merge commits are not Conventional Commits); `--allow-branch` needs the exact branch name (globs are not supported, default is `main` only).
