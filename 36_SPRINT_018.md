# Sprint 018 - Character Stats

## Sprint Goal

Create one readable source of player combat-stat composition and make Heavy Strike scale from the same attack power as basic attacks.

## Task 1 - Author The Warrior Stat Identity

Acceptance criteria:

- the warrior class definition contains explicit maximum-health and attack-power bonuses;
- level-1 totals remain 100 maximum health, 14 attack power, and 3 defense;
- the level-1 totals are composed from 90 base health plus 10 warrior health and 12 base attack plus 2 warrior attack;
- the existing per-level growth remains 10 health and 2 attack power.

## Task 2 - Use One Attack-Power Formula

Acceptance criteria:

- total attack power is the sum of base, class, level, and equipped-weapon contributions;
- basic damage remains total attack power minus target defense, clamped to at least 1;
- Heavy Strike uses 170% of total attack power minus target defense, clamped to at least 1;
- against the current 2-defense wolf, Heavy Strike deals 22 damage without a weapon and 29 with the Rusty Sword;
- after Heavy Strike, basic autoattacks continue against the same living target.

## Task 3 - Show Readable Character Stats

Acceptance criteria:

- a temporary character-stats window opens from an on-screen button or the `C` key;
- the window shows identity, level, experience, health, attack, defense, attack interval, and Heavy Strike power;
- health and attack show readable class, level, and equipment bonuses without exposing internal base-value arithmetic;
- displayed values refresh immediately after equipping a weapon or changing level;
- character stats, inventory, and quest-journal windows can be dragged by their headers and remain reachable on screen;
- hovering Heavy Strike shows a temporary data-driven description, damage, range, and cooldown tooltip;
- the window uses blockout UI and introduces no final art.

## Task 4 - Keep The Change Verifiable

Acceptance criteria:

- the starter-village builder authors warrior bonuses, the Heavy Strike coefficient and description, player base values, movable windows, tooltip, and stats-window wiring;
- an editor validator checks class data, exact stat composition, exact basic and Heavy Strike damage, prefab defaults, movable windows, tooltip, and scene UI;
- the project compiles and the Sprint 018 validator passes in Unity batch mode;
- movement, targeting, autoattack, skill approach and cooldown, NPC dialogue, recovery, death, experience, loot, quests, creation, selection, and saving are preserved.

## Not In This Sprint

- attributes, distributable points, critical hits, random damage, resistances, armor types, or new equipment slots;
- mana, rage, stamina, or another class resource;
- additional classes, races, skills, weapons, enemies, items, quests, or zones;
- final character models, equipment visuals, animation, VFX, sound, icons, or UI art;
- server, multiplayer, or networked persistence.
