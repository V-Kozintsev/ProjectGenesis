# Sprint 022 - Death State And Respawn Choice

## Sprint Goal

Replace the automatic prototype respawn with an explicit death state: the character stays dead in place, gameplay input is blocked, the experience penalty is applied once, and the player returns to the village only after pressing a centered resurrection button.

## Task 1 - Separate Death State From Combat

Acceptance criteria:

- player death no longer starts an automatic timed respawn from `PlayerCombatController`;
- a dedicated death controller listens to `Health.Died` and owns the player death lifecycle;
- the current combat target, pending skill, pending loot pickup, pending NPC approach, and movement path are stopped on death;
- the player remains at the death position until a resurrection choice is confirmed;
- the experience penalty is applied once per death, not again when confirming resurrection.

## Task 2 - Block Gameplay While Dead

Acceptance criteria:

- ground movement, WASD movement, enemy clicks, NPC interaction, loot pickup, skills, and consumable use are unavailable while dead;
- health regeneration remains disabled while dead;
- camera controls can remain available because they do not change game state;
- existing target selection and UI windows do not break after death.

## Task 3 - Add Temporary Resurrection UI

Acceptance criteria:

- a centered temporary UI panel appears after player death;
- the panel explains that the character is dead and shows the experience loss from this death;
- the first available choice is `Воскреснуть в деревне`;
- pressing the choice moves the player to the configured spawn point, restores full health, re-enables gameplay, and hides the panel;
- no final art, animations, corpse resurrection, or alternative resurrection locations are required.

## Task 4 - Preserve And Verify Regressions

Acceptance criteria:

- starter-village builder wires the death controller and temporary UI;
- player prefab and scene have a configured respawn point path;
- an editor validator checks death-state data, one-time penalty, no automatic position change, no healing before confirmation, village resurrection, and input-lock state;
- relevant regression validators from combat, skills, inventory, equipment, loot, quests, and saving still pass;
- the user performs the ordinary Play Mode visual check.

## Not In This Sprint

- corpse animation, ragdoll, VFX, sound, countdowns, or final UI art;
- resurrection scrolls, resurrection skills, party resurrection, paid resurrection, or corpse-location resurrection;
- durability loss, item drop on death, PvP rules, karma, death recap, or combat log integration;
- server authority, multiplayer death ownership, or networked persistence;
- separate equipment paper doll, chat window, message feed, new quests, enemies, zones, items, or caves.
