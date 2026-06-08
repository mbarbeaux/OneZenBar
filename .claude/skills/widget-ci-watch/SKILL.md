---
name: widget-ci-watch
description: Watch the UWP widget CI build and report clearly — confirm which runner the widget job used (self-hosted DESKTOP-MAO7ED3 vs GitHub-hosted), surface MSBuild errors readably, and verify the signed .msixbundle was produced. Use when asked to check/watch a widget build, diagnose a failing widget job, or confirm a build ran on the self-hosted runner.
---

# Watch the widget CI build

The `widget` job (in `quality-gate.yml`, called by `build.yml` on PRs and `develop.yml` on push) builds the UWP Xbox Game Bar widget on Windows. This skill watches a run and reports the facts that matter.

## Find the run

```bash
# By PR branch:
gh run list --branch <branch> --limit 5
# Or list recent runs:
gh run list --limit 5
```

## Watch to completion

```bash
gh run watch <run-id> --exit-status --interval 15
gh run view <run-id> --json conclusion,jobs --jq '{conclusion, jobs: [.jobs[] | {name, conclusion}]}'
```

## Confirm WHICH runner the widget job used

Critical for the public-repo fork guard and the `SELF_HOSTED` toggle. The widget job uses the self-hosted runner only on push or same-repo PRs when `SELF_HOSTED=true`; fork PRs fall back to GitHub-hosted.

```bash
gh run view <run-id> --log | grep "build / widget" | grep -m1 "Runner name"
```
- `Runner name: 'DESKTOP-MAO7ED3'` → ran on the self-hosted machine.
- A `windows-2025-vs2026` style name → ran on the GitHub-hosted image (expected for fork PRs, or when `SELF_HOSTED != 'true'`).

## Surface MSBuild errors / confirm the bundle was produced

```bash
gh run view <run-id> --log | grep "build / widget" \
  | grep -iE "error|warning MSB|Build succeeded|\.msixbundle|\.msix|StampAppx|Build FAILED" | head -40
```
A successful build ends with a line like:
`Sample.Widget_<version>_x86_x64_ARM64.msixbundle` under `AppPackages/`, plus per-platform `.msix` (x86/x64/ARM64) and the language packs (`language-de`, `language-es`, `language-fr`).

## Common failure signals (and what they mean)

- `MSB4276` / pointing at `VS\2022\` → the runner is using MSBuild 17 (VS 2022); the widget needs **MSBuild 18 / VS 2026** (MSBuild 17 cannot load the .NET 10 SDK). Check the self-hosted toolchain.
- SDK resolution / `global.json` errors → the runner lacks a .NET 10 SDK ≥ 10.0.300 on PATH.
- Unsigned bundle when you expected signed → the signing secrets (`SIGNING_CERTIFICATE_*`) or Store identity secrets are missing for that run (normal on fork/Dependabot PRs, which cannot read secrets).
- Job queued and never starts with `SELF_HOSTED=true` → the self-hosted runner is offline (no availability fallback). See the `self-hosted-runner` skill.

## Report back

State plainly: pass/fail per job, the runner the widget job used, the produced bundle version (or the first real error with its log line). Don't claim success unless the bundle line is present and the job conclusion is `success`.
