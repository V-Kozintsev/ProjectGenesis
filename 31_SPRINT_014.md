# Sprint 014 - First Enemy Variety

## Sprint Goal

Prove that the shared combat, AI, level, and experience systems support more than one authored enemy type.

## Task 1 - Add A Forest Boar

Acceptance criteria:

- the project contains a reusable `PF_Enemy_Boar` prefab with a distinct brown blockout;
- the forest boar is level 2 and awards 30 base experience;
- it has 60 health, 10 attack, 2 defense, and its own movement and recovery tuning;
- its display name and level appear through the existing selected-target panel;
- it reuses `EnemyBrain`, selection, combat, territory, death, cleanup, and respawn behavior.

## Task 2 - Preserve Encounter Readability

Acceptance criteria:

- the combat area keeps exactly three active spawn points;
- the west and east points continue to spawn young wolves;
- the northern point spawns one forest boar instead of a third wolf;
- all three enemies remain inside the existing hostile territory and outside the peaceful village.

## Task 3 - Preserve Existing Rewards And Quest Rules

Acceptance criteria:

- the wolf-tail quest advances only from wolves;
- the boar does not use the wolf loot table or wolf-tail chance;
- killing the boar uses the Sprint 013 level-difference experience formula;
- wolf loot, trophies, corpse cleanup, and respawn behavior remain unchanged.

## Task 4 - Keep The Feature Verifiable

Acceptance criteria:

- the starter-village builder reproduces the boar material, prefab, and mixed spawners;
- an editor validator checks boar identity, level, reward, stats, loot exclusion, and scene population;
- the project compiles and the validator passes in Unity batch mode.

## Initial Prototype Tuning

- forest boar: level 2;
- maximum health: 60;
- attack / defense: 10 / 2;
- base experience: 30;
- level 1 hero reward: 33 experience;
- level 2 hero reward: 30 experience;
- level 3 hero reward: 22 experience after whole-number rounding.

## Not In This Sprint

- boar-specific loot, quests, skills, charge attacks, sounds, or animations;
- automatic stat scaling from enemy level;
- packs, assists, social aggro, factions, elites, bosses, or randomized variants;
- additional spawn points or higher total enemy density;
- final models, textures, effects, or server-authoritative spawning.
