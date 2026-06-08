---
name: ship-change
description: Ship a code change through this repo's GitFlow + fast-forward procedure — create a branch off develop, commit with Conventional Commits, open a PR, wait for CI to go green, fast-forward merge, then sync develop and clean up. Use whenever the user asks to apply/ship/land a change "the usual way", open a PR, or merge work into develop.
---

# Ship a change (branch → PR → green CI → fast-forward → sync)

This repo merges **by fast-forward only** (SHAs and signatures preserved). NEVER use GitHub's merge buttons. The full procedure below is mandatory and order-sensitive.

## Preconditions

- Work must target `develop` (features/fixes) — releases go through `release/*` → `main` and are CI-automated, not handled here.
- The change should already exist in the working tree (edited/staged), or be described well enough to make.

## Procedure

1. **Branch from `develop`.** Make sure you're starting from an up-to-date `develop`:
   ```bash
   git checkout develop && git pull --ff-only
   git checkout -b <type>/<short-kebab-desc>
   ```
   `<type>` is the Conventional-Commit type matching the work: `feat`, `fix`, `ci`, `chore`, `docs`, `test`, `style`, `refactor`, `perf`, `revert`, `build`.

2. **Stage only the relevant files** (never `git add -A` blindly) and verify with `git status`.

3. **Format before committing** if any `.cs`/`.vb` changed: `dotnet format`. The pre-commit hook rejects unformatted staged code (`--verify-no-changes --severity error`).

4. **Commit with a Conventional Commits message.** The `commit-msg` hook runs `dotnet commit-linter` and will reject anything else. End the message with:
   ```
   Co-Authored-By: Claude Opus 4.8 (1M context) <noreply@anthropic.com>
   ```
   The Husky hooks (encoding check, format check, commit-linter) run automatically — read their output; a failure means fix and re-commit.

5. **Push and open the PR to `develop`:**
   ```bash
   git push -u origin <branch>
   gh pr create --base develop --head <branch> --title "<conventional title>" --body "<what + why>"
   ```
   End the PR body with `🤖 Generated with [Claude Code](https://claude.com/claude-code)`.

6. **Wait for CI to be fully green BEFORE doing anything else.** This is a hard rule (see the user's standing instruction: never `/fast-forward` until the pipeline is green). Watch the Build run to completion and confirm every check passes:
   ```bash
   gh run watch <run-id> --exit-status --interval 15
   gh pr checks <pr-number>
   ```
   If the `widget` job is involved and `SELF_HOSTED=true`, it builds on the self-hosted runner — see the `widget-ci-watch` skill to confirm the runner and surface MSBuild errors. If CI fails, fix on the same branch and push again; do not proceed.

7. **Fast-forward merge** by commenting on the PR (the `fast-forward.yml` workflow pushes the target to the PR head):
   ```bash
   gh pr comment <pr-number> --body "/fast-forward"
   ```
   If `fast-forward-check` reported the merge is impossible, rebase the branch onto `develop` first (`git rebase origin/develop`), push `--force-with-lease`, re-check CI, then retry.

8. **Confirm the merge and sync local `develop`:**
   ```bash
   # wait until merged
   until [ "$(gh pr view <pr-number> --json state --jq .state)" = "MERGED" ]; do sleep 4; done
   git checkout develop && git pull --ff-only
   git branch -d <branch>
   ```
   The merge commit OID should equal the branch's last commit SHA (proof it was a true fast-forward).

## Notes

- Conversation language may be French, but commit messages, PR text, branch names and code stay **English** (repo-wide rule).
- The remote branch is usually auto-deleted by the fast-forward action; verify and delete if it lingers.
- Do not run ShipIt, tag, or back-merge manually — releases are CI-automated on push to `main`.
