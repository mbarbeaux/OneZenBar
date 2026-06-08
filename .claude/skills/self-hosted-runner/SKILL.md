---
name: self-hosted-runner
description: Manage the self-hosted Windows runner toggle for the widget build — check whether DESKTOP-MAO7ED3 is online before flipping the SELF_HOSTED repo variable, and warn if SELF_HOSTED is true while the runner is offline (jobs would queue forever). Use when asked to switch the build to/from the self-hosted runner, enable/disable SELF_HOSTED, or check the runner's status.
---

# Manage the self-hosted runner toggle

The three Windows jobs (`widget` in `quality-gate.yml`, `publish` in `release-github.yml`, `submit` in `store-upload.yml`) pick their runner from the `SELF_HOSTED` **repository variable**. It is a **manual** toggle: GitHub has NO availability-based fallback — if `SELF_HOSTED=true` while the runner is offline, jobs **queue and wait indefinitely**.

The runner is `DESKTOP-MAO7ED3` (labels: `self-hosted`, `Windows`, `X64`).

## Always check runner status BEFORE setting SELF_HOSTED=true

```bash
gh api repos/{owner}/{repo}/actions/runners \
  --jq '.runners[] | {name, status, busy, labels: [.labels[].name]}'
```
- `status: "online"` → safe to enable.
- `status: "offline"` → do NOT set `true` (jobs would hang). Tell the user the runner is down; keep it `false`.

## Flip the toggle

```bash
gh variable set SELF_HOSTED --body true     # route Windows jobs to the self-hosted machine
gh variable set SELF_HOSTED --body false    # route them back to the GitHub-hosted image
gh variable list                            # confirm
```

## Rules and reminders

- **When enabling**: confirm online first (above). If online, mention it will take effect on the next triggering run.
- **When the machine is going down**: flip back to `false` so jobs don't queue. Proactively remind the user of this whenever you set it `true`.
- **Fork-PR guard**: even with `SELF_HOSTED=true`, fork PRs never use the self-hosted runner (the `widget` job's `runs-on` requires a push or same-repo PR). This is intentional security on the public repo.
- **Toolchain requirement**: the runner must match the hosted image — VS 2026 (MSBuild 18), UWP workload, Windows 10 SDK 26100, and a .NET 10 SDK ≥ 10.0.300 on PATH. `Setup .NET SDK` is skipped on self-hosted (`if: vars.SELF_HOSTED != 'true'`).
- To verify a build actually landed on the runner after enabling, use the `widget-ci-watch` skill.
