# Sprint 013 - Enemy Levels And Experience Scaling

## Sprint Goal

Make enemy level meaningful by showing it to the player and using it to calculate combat experience rewards.

## Task 1 - Author Enemy Identity And Level

Acceptance criteria:

- every `EnemyBrain` has an Inspector-authored display name and level;
- the young wolf defaults to level 1 with 20 base experience;
- the selected-target panel shows both the enemy name and level;
- enemy level changes do not automatically rewrite health, damage, loot, or quest rules.

## Task 2 - Scale Experience By Level Difference

Acceptance criteria:

- equal-level enemies award 100% of their base experience;
- each enemy level below the player removes 25 percentage points down to a 10% minimum;
- each enemy level above the player adds 10 percentage points up to a 150% maximum;
- the final reward is rounded to a whole number and cannot be negative;
- quest rewards continue to use their authored fixed experience and are not scaled as enemy kills.

## Task 3 - Keep Tuning Editable

Acceptance criteria:

- lower-level penalty, minimum multiplier, higher-level bonus, and maximum multiplier are editable on `PlayerProgression`;
- enemy level and base experience remain independently editable on the enemy prefab;
- the starter-village builder reproduces all prototype defaults.

## Task 4 - Keep The Feature Verifiable

Acceptance criteria:

- an editor validator checks player and wolf prefab defaults;
- the validator checks equal, weaker, stronger, minimum, maximum, and zero-reward boundaries;
- the project compiles and the validator passes in Unity batch mode.

## Initial Prototype Tuning

- young wolf: level 1, 20 base experience;
- level 1 player versus young wolf: 20 experience;
- level 2 player versus young wolf: 15 experience;
- level 3 player versus young wolf: 10 experience;
- level 4 player versus young wolf: 5 experience;
- level 5 player versus young wolf: 2 experience.

## Not In This Sprint

- automatic enemy stat growth, randomized levels, elite modifiers, or level ranges;
- additional enemy species, zones, encounters, quests, items, or loot changes;
- party contribution, shared kills, rested experience, boosters, or mentor systems;
- experience notifications, combat log entries, animation, audio, or final effects;
- server-authoritative reward calculation or anti-cheat validation.
