---
name: post-release-sync
description: Recover when develop has diverged from main after a CI-automated release, so the auto fast-forward of develop failed. Rebase develop onto main and force-push with lease (never back-merge in CI — rebased commits would be unsigned). Use when asked to sync develop after a release, fix a failed fast-forward of develop, or when develop and main have diverged post-release.
---

# Sync develop after a release (divergence recovery)

Releases are CI-automated: on push to `main`, the workflow runs ShipIt, commits `chore: release X.Y.Z`, tags `vX.Y.Z`, and **fast-forwards `develop` to `main`**. If features merged into `develop` during the release window, `develop` has diverged and that auto fast-forward **fails with a warning** — `develop` must be rebased onto `main` manually and locally (NEVER back-merge in CI: rebased commits would lose signatures / be unsigned).

## When to use

- The release workflow logged a fast-forward failure for `develop`.
- `git log` shows `develop` and `origin/main` have diverged after a `chore: release` commit.

## Diagnose first

```bash
git fetch origin
git log --oneline -3 origin/main
git log --oneline -3 origin/develop
git rev-list --left-right --count origin/develop...origin/main
```
- Left-count = commits on `develop` not on `main` (your features); right-count = release commit(s) on `main` not on `develop`.
- If left-count is `0`, `develop` is simply behind → a plain fast-forward suffices, no rebase needed:
  ```bash
  git checkout develop && git merge --ff-only origin/main && git push
  ```

## Rebase when truly diverged (both counts > 0)

```bash
git checkout develop
git rebase origin/main
# resolve conflicts if any, then:
git push --force-with-lease
```

## Rules

- Always `--force-with-lease`, never plain `--force`.
- Do the rebase **locally** (signatures are preserved); do not script a back-merge in CI.
- After pushing, confirm `git rev-list --left-right --count origin/develop...origin/main` shows the right-count is `0` (develop now contains the release commit) and re-check that CI on `develop` is green.
- Do not run ShipIt, tag, or re-release manually — only the develop/main reconciliation is manual.
