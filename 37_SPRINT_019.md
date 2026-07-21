# Sprint 019 - Item Instances And Inventory Selection

## Sprint Goal

Give every collected item a stable identity and let the player explicitly select which inventory slot an action applies to.

## Task 1 - Introduce Stable Item Instances

Acceptance criteria:

- `ItemDefinition` remains shared authored data and is not treated as one physical item;
- every collected inventory item receives a non-empty unique instance id;
- two Rusty Swords can coexist as distinct inventory entries;
- inventory capacity and duplicate-instance validation remain deterministic;
- no random affixes, durability, binding, stacks, or item levels are introduced.

## Task 2 - Preserve Exact Items In Saves

Acceptance criteria:

- the local profile format advances to version 4 and stores instance id plus definition id for every inventory entry;
- the equipped main hand is stored by instance id rather than only by definition id;
- profiles from versions 1, 2, and 3 still load;
- legacy item-id lists migrate to unique instances and preserve the first matching equipped weapon;
- the next automatic save writes the migrated version-4 representation.

## Task 3 - Select A Concrete Inventory Slot

Acceptance criteria:

- the temporary inventory exposes eight stable slot buttons;
- clicking an occupied slot selects that exact item instance;
- the selected slot is clearly highlighted and its details drive the action button;
- equipping or removing a weapon acts on the selected instance, including when two copies share one definition;
- empty slots cannot be selected or equipped;
- selection remains stable while the window is open and inventory or equipment state refreshes.

## Task 4 - Keep The Change Verifiable

Acceptance criteria:

- the starter-village builder authors the eight-slot UI and all runtime references;
- an editor validator checks unique ids, duplicate rejection, legacy migration, exact-instance equipment, profile compatibility, prefab defaults, and scene wiring;
- a Play Mode development command can add two separate Rusty Swords without relying on random loot;
- the project compiles and the Sprint 019 validator passes in Unity batch mode;
- combat, skills, movement, targeting, dialogue, recovery, death, experience, loot, quests, character entry, stats, movable windows, and saving remain functional.

## Not In This Sprint

- additional equipment slots, armor, consumables, vendors, currency, selling, trading, crafting, or item destruction;
- stacks, splitting, sorting, drag-and-drop, random affixes, durability, binding, rarity art, or tooltips;
- new items, enemies, NPCs, quests, zones, models, animations, VFX, sound, icons, or final UI art;
- server, multiplayer, or networked persistence.
