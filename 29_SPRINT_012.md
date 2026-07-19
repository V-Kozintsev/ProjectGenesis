# Sprint 012 - Player Death Penalty

## Sprint Goal

Make player death affect progression without losing inventory, quests, or the reliable village respawn flow.

## Task 1 - Add Configurable Experience Loss

Acceptance criteria:

- death experience-loss rate and minimum loss are editable on the player prefab;
- the default penalty is 10% of the current level requirement with a minimum of 10 experience;
- only experience actually owned by the player can be removed;
- setting both values to zero disables the penalty.

## Task 2 - Support Level Loss

Acceptance criteria:

- experience loss can cross into the previous level when current-level experience is insufficient;
- reaching exactly zero experience keeps the current level until a later loss crosses the boundary;
- the player can never fall below level 1 or below zero experience;
- health and attack bonuses are recalculated after a lost level without reviving the dead player early.

## Task 3 - Preserve Respawn And Persistence

Acceptance criteria:

- the penalty is applied once per player death before the normal delayed respawn;
- the player still respawns in the village with full health;
- inventory, equipment, and quests are not removed or reset by death;
- the existing automatic profile stores the resulting level and experience without a format change.

## Task 4 - Keep The Feature Verifiable

Acceptance criteria:

- the starter-village builder reproduces the player-prefab defaults;
- an editor validator checks prefab tuning, level-boundary loss, the level-1 floor, and disabled penalties;
- the project compiles and the validator passes in Unity batch mode.

## Task 5 - Preserve Visible Combat Feedback

Acceptance criteria:

- the player health bar shrinks and grows together with its numeric health value;
- the selected enemy health bar shrinks and grows together with its numeric health value;
- reaching zero health hides the corresponding fill without changing the existing labels or panels.

## Not In This Sprint

- item, currency, durability, quest-progress, or equipped-item loss;
- corpse running, resurrection services, gravestones, or recovery debt;
- a death summary window, floating text, animation, audio, or final effects;
- PvP-specific penalties or server-authoritative death validation;
- final penalty balancing for the live economy.
