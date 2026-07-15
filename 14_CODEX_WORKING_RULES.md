# Codex Working Rules

## General Rule

Codex should work like a careful teammate, not like a code generator dumping files into the project.

## Before Coding

For every meaningful task, Codex should:

- inspect existing files;
- read relevant docs;
- identify current patterns;
- keep changes small;
- avoid unrelated refactors.

## Task Splitting

Large goals must be split into tasks.

Example:

Bad task:

- Build the whole combat system.

Good tasks:

- Add Health component.
- Add basic melee damage.
- Add enemy death event.
- Add experience reward.
- Add loot roll.
- Add UI health bar.

## Documentation Updates

Update documentation when:

- architecture changes;
- a system contract changes;
- a milestone is completed;
- a decision affects future work.

## Commit Rule

Each commit should represent one understandable step.

Good commit examples:

- `Add player health and damage handling`
- `Add first wolf enemy prefab`
- `Document inventory data model`

## Backlog Rule

Interesting ideas that are not part of the current sprint must go to [17_BACKLOG.md](17_BACKLOG.md).

