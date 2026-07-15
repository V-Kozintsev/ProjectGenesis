# Sprint 003 - First Contact

## Sprint Goal

Let the player talk to the first NPC and accept the first quest without starting combat or inventory systems.

## Tasks

### Task 1 - Document Sprint Scope

Create this sprint document before implementation.

Acceptance criteria:

- sprint goal is clear;
- current exclusions are listed;
- acceptance criteria describe a small playable result.

### Task 2 - Add First NPC

Place a simple Village Elder prototype in the starter village.

Acceptance criteria:

- NPC is visible in the scene;
- NPC is named `Village Elder`;
- player can identify when interaction is available.

### Task 3 - Add Basic Interaction

Allow the player to interact with the NPC when nearby.

Acceptance criteria:

- interaction is range-limited;
- single-clicking the NPC moves the player toward the NPC;
- double-clicking the NPC opens dialogue, but only when the player is close enough;
- double-clicking from farther away moves the player toward the NPC and opens dialogue when close enough;
- interaction radius, approach distance, maximum click-to-approach distance, and double-click timing are adjustable in the player interaction component;
- `E` remains available as a temporary fallback;
- movement controls from previous sprints remain unchanged.

### Task 4 - Add Dialogue Window

Create a simple non-final dialogue UI.

Acceptance criteria:

- dialogue shows the NPC name;
- dialogue shows short story text;
- dialogue can be closed;
- UI stays intentionally simple.

### Task 5 - Add First Quest State

Let the player accept the first quest from the Village Elder.

Acceptance criteria:

- quest starts in `NotStarted`;
- accepting the quest changes it to `Active`;
- accepting the same quest twice is prevented;
- no combat objective is implemented yet.

### Task 6 - Update Project Notes

Update README, roadmap, and changelog after implementation.

Acceptance criteria:

- controls mention NPC interaction;
- current stage mentions Sprint 003;
- changelog records what changed.

## Not In This Sprint

- combat;
- enemies;
- quest completion;
- rewards;
- inventory;
- trading;
- branching dialogue;
- save/load.
