# Sprint 020 - Inventory Rearrangement And Weapon Variety

## Sprint Goal

Make the eight inventory cells represent persistent positions, support direct rearrangement, and verify exact-item equipment with a second weapon definition.

## Task 1 - Preserve Inventory Slot Positions

Acceptance criteria:

- the inventory owns eight stable positions that may contain an item or remain empty;
- adding loot uses the first available empty position;
- occupied count remains separate from capacity;
- profile version 5 stores the position of every item instance;
- profiles from versions 1-4 migrate their existing item order into consecutive positions;
- empty gaps and reordered items survive automatic save and restore.

## Task 2 - Move And Swap Items

Acceptance criteria:

- an occupied slot can be dragged onto another inventory slot;
- dropping onto an empty position moves the exact instance there;
- dropping onto an occupied position swaps the two exact instances;
- the selected item remains selected after moving;
- the equipped instance remains equipped after moving;
- empty source slots cannot start a move and invalid indices are rejected.

## Task 3 - Add One Contrasting Weapon

Acceptance criteria:

- a reusable Worn Axe definition exists with a stable id and a visibly different attack bonus;
- the Rusty Sword remains `+4` attack and the Worn Axe provides `+7` attack;
- boars can drop the Worn Axe through a reusable loot table;
- selecting and equipping either weapon updates the displayed attack power to the correct total;
- a Play Mode development command can add one sword and one axe without relying on random drops.

## Task 4 - Keep The Change Verifiable

Acceptance criteria:

- the starter-village builder authors the axe, boar loot, catalog references, and drag handlers;
- an editor validator checks fixed positions, moving, swapping, exact equipment, profile migration, item data, loot data, prefab defaults, and scene wiring;
- the project compiles and the Sprint 020 validator passes in Unity batch mode;
- movement, targeting, combat, skills, dialogue, recovery, death, experience, wolf loot, quests, character entry, stats, movable windows, and saving remain functional.

## Not In This Sprint

- additional equipment slots, armor, consumables, stacks, splitting, sorting buttons, vendors, currency, selling, trading, crafting, or item destruction;
- random affixes, durability, binding, item levels, rarity art, icons, or item tooltips;
- new enemies, NPCs, quests, zones, models, animations, VFX, sound, or final UI art;
- server, multiplayer, or networked persistence.
