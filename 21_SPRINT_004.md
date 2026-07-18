# Sprint 004 - First Fight

## Sprint Goal

Let the player fight one wolf, survive or respawn, and receive an experience reward without starting inventory, loot, skills, or quest completion.

## Tasks

### Task 1 - Document Sprint Scope

Create this sprint document before implementation.

Acceptance criteria:

- the first combat loop is small and visually testable;
- combat responsibilities are separated from movement and quest state;
- exclusions prevent inventory and quest scope from leaking into this sprint.

### Task 2 - Add Health And Combat Stats

Add reusable health and simple combat-stat components.

Acceptance criteria:

- player and wolf have adjustable maximum health, attack power, defense, attack range, and attack interval;
- damage uses `max(1, attack power - defense)`;
- health cannot fall below zero or rise above maximum;
- death is reported once.

### Task 3 - Add Player Basic Attack

Allow the player to select and fight a wolf with the existing point-and-click controls.

Acceptance criteria:

- clicking a living wolf once selects it;
- clicking an already selected wolf moves the player into melee range using NavMesh and starts combat, with a quick double-click acting as a shortcut;
- the player automatically performs basic attacks while the target is alive and in range;
- clicking the ground or using WASD stops combat movement and attacks but keeps the combat target selected;
- `Esc`, the target-panel close button, or selecting another creature clears or switches the target;
- NPC interaction and camera controls continue to work.

### Task 4 - Add First Wolf AI

Place one prototype wolf near the north road.

Acceptance criteria:

- the wolf begins idle at a fixed home position;
- the wolf detects or retaliates against the player, chases, and attacks in range;
- the wolf returns home if the player leaves its allowed area;
- the wolf stops acting after death;
- no patrol, pack behavior, special attack, or respawn is implemented yet.

### Task 5 - Add Death And Experience

Complete the first combat outcome.

Acceptance criteria:

- killing the wolf grants experience once;
- the player experience total is visible;
- player death restores health and returns the player to the village spawn point;
- death has simple prototype visual feedback.

### Task 6 - Add Minimal Combat HUD

Show enough information to understand the fight.

Acceptance criteria:

- player health is visible;
- selected wolf name and health are visible;
- current experience is visible;
- HUD is intentionally compact and does not replace the existing dialogue UI.

### Task 7 - Update Project Notes

Update README, roadmap, and changelog after implementation.

Acceptance criteria:

- controls explain how to start and cancel combat;
- current stage mentions Sprint 004;
- changelog records the first combat loop.

## Not In This Sprint

- inventory;
- loot drops;
- equipment;
- quest objective progress or completion;
- combat skills;
- critical hits;
- combat animations or final art;
- multiple enemy types;
- enemy respawn;
- save/load;
- multiplayer.
