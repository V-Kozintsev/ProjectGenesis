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

Before proposing a sprint, Codex should also state:

- why this work is the current priority;
- which completed systems it builds on;
- what is explicitly outside the sprint.

The current priority in [32_DEVELOPMENT_PLAN_RU.md](32_DEVELOPMENT_PLAN_RU.md) takes precedence over an isolated future feature mentioned elsewhere in the documentation.

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

New ideas from the user must not be lost. Record them with their purpose and dependencies, then revisit them when choosing the next sprint. Do not interrupt active work unless the idea fixes a blocker or the user explicitly changes the priority.

## Focus Rule

- Do not create a location, landmark, final UI, or decorative pass only because it appears in a future feature list.
- Visual work must support a stable playable flow or an explicitly approved visual milestone.
- A cave is gameplay content, not just an entrance prop. It starts only after its combat, reward, world-transition, and technical dependencies are identified.
- Prefer one complete reusable system over several disconnected examples.

## User Testing Rule

Codex owns automated, deterministic, compilation, serialization, and difficult regression checks. The user normally owns ordinary Play Mode and visual evaluation, with short exact testing steps supplied by Codex.

## Task Handoff Rule

Codex is authorized to decide when a fresh Codex task is useful and to prepare and open it without waiting for the user to write the handoff prompt.

Create a fresh task when:

- a sprint is complete and committed;
- the work changes to a substantially different system;
- the current conversation has accumulated enough context to risk losing the active goal.

Prefer a clean committed repository boundary. Do not split ownership while meaningful uncommitted implementation is still in progress unless recovery requires it.

Every handoff must include the repository path, branch, latest commit, current goal, relevant decisions, exclusions, test responsibilities, known unrelated files, and the rule that no final commit is created without separate user approval. Use [18_CODEX_FIRST_MESSAGE.md](18_CODEX_FIRST_MESSAGE.md) as the checklist.
