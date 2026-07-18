# Sprint 007 - First Zone Loop

## Sprint Goal

Turn the northern combat blockout into a small repeatable hunting area with several wolves, respawn, chance-based loot, a collection quest, and visible quest tracking.

## Tasks

### Task 1 - Add Repeatable Wolf Spawns

Acceptance criteria:

- the northern combat area contains three independent wolf spawn points;
- each wolf uses the existing selection, approach, combat, leash, and death behavior;
- a dead wolf leaves its body for the configured corpse lifetime;
- a replacement wolf appears at the same spawn point after a configurable respawn delay;
- respawned NavMesh agents are placed safely on the runtime NavMesh.

### Task 2 - Add Chance-Based Loot

Acceptance criteria:

- the wolf has independently configurable chances for a rusty sword and a quest item;
- the rusty sword is no longer guaranteed on every kill;
- the quest trophy is rolled only while its matching quest objective is active and incomplete;
- a successful trophy roll advances objective progress immediately without creating a world pickup;
- regular item drops still appear in the world and use the existing click-to-approach flow;
- dropped regular items are transient world state and are not persisted.

### Task 3 - Add A Wolf-Tail Collection Quest

Acceptance criteria:

- the Village Elder asks for five wolf tails;
- a successful tail roll advances progress by one immediately after the kill;
- killing a wolf without a successful tail roll does not advance progress;
- tails do not consume normal inventory slots;
- progress is capped at `5 / 5`, and further kills cannot award tails before turn-in;
- progress survives Play mode restarts through the existing automatic persistence;
- the quest becomes ready to turn in at `5 / 5` and can still be rewarded only once.

### Task 4 - Add A Compact Quest Tracker

Acceptance criteria:

- an accepted quest appears in a compact panel on the right side of the screen;
- the panel shows the quest name and current objective progress;
- the panel changes to a return instruction when the objective is complete;
- the panel hides before acceptance and after turn-in;
- the tracker does not overlap the target panel, dialogue window, combat HUD, or inventory button.

### Task 5 - Preserve The Existing First Loop

Acceptance criteria:

- one click selects an NPC or wolf and a second click performs its action;
- ground movement, camera controls, WASD fallback, combat, inventory, equipment, level up, and automatic persistence still work;
- the local-profile reset command still produces a clean first-level character.

## Initial Tuning

- wolf count: 3;
- corpse lifetime: 6 seconds;
- respawn delay: 12 seconds;
- wolf-tail requirement: 5;
- wolf-tail drop chance: 70%;
- rusty-sword drop chance: 35%.

All values are prototype tuning and should remain adjustable in the Inspector or scene builder.

## Not In This Sprint

- general-purpose authored loot-table assets;
- item stacks or a full quest inventory window;
- enemy population persistence across sessions;
- rare-quality tiers, guaranteed-pity systems, or personal/group loot;
- additional enemy families or quests;
- regeneration, skills, vendors, crafting, or multiplayer networking.
