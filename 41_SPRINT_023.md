# Sprint 023 - Character Equipment View

## Sprint Goal

Separate equipped items from bag storage in both the runtime model and the temporary UI. Opening the inventory must show the bag and exact equipped weapon and body-armor instances together, support safe equip, replacement, unequip, drop, and permanent-destroy actions, and preserve older local profiles through migration.

## Task 1 - Separate Equipment From Bag Storage

Acceptance criteria:

- an equipped weapon or body armor no longer occupies one of the eight bag positions;
- equipping an item transfers that exact instance from the bag into its compatible equipment slot;
- replacing equipment returns the previous exact instance to the bag position released by the new item;
- unequipping returns the exact instance to a free bag position;
- unequipping is refused without item loss when the bag is full;
- weapon and armor slots continue to reject incompatible item categories.

## Task 2 - Add Character Equipment Presentation

Acceptance criteria:

- opening `Инвентарь [I]` shows the bag and equipment in one movable window;
- the equipment side contains separate `Правая рука` and `Тело` slots;
- each slot clearly shows whether it is empty and displays the equipped item plus its relevant bonus;
- selecting a compatible bag item shows a simple comparison with the currently equipped item;
- the inventory window can equip the selected bag item, replace current equipment, or unequip the selected equipment slot;
- refusal because of a full bag is shown as short feedback;
- the inventory window remains a bag with eight stable positions and no longer presents equipped items as bag contents.

## Task 3 - Free Bag Space Safely

Acceptance criteria:

- `Выбросить` transfers the selected exact bag instance into a collectible world pickup near the character;
- collecting that pickup restores the same instance rather than creating a replacement copy;
- clicking `Корзина` or dragging a bag slot onto it opens a centered confirmation window;
- confirming permanently removes only the selected bag item, while cancellation preserves it;
- changing selection cancels pending trash confirmation;
- equipped items cannot be dropped or destroyed until they are unequipped into the bag;
- drop and permanent destruction are unavailable while the character is dead.

## Task 4 - Preserve Local Profiles

Acceptance criteria:

- profile version 7 stores bag instances and equipped instances separately;
- version-1 through version-6 profiles remain loadable;
- version-6 equipped references migrate by transferring the referenced exact instances out of their old bag positions;
- a version-7 save and restore preserves instance ids, bag positions, main hand, body armor, attack, and defense bonuses.

## Task 5 - Validate Regressions

Acceptance criteria:

- the starter-village builder creates and wires the combined inventory and equipment view plus item-drop controller;
- an editor validator checks transfer, replacement, full-bag refusal, compatibility, drop identity, permanent removal, migration, prefab data, and scene UI;
- relevant validators for item instances, inventory movement, consumables, character identity and stats, death, combat, skills, loot, quests, and saving still pass;
- the user performs the ordinary Play Mode visual check.

## Not In This Sprint

- helmet, gloves, boots, jewelry, off-hand, two-handed, ammunition, cosmetic, or class-restricted slots;
- item icons, character portrait or 3D paper doll, final art, drag preview, drop highlighting, or equip animation;
- drag-and-drop between bag and equipment slots;
- item comparison tooltips, requirements, durability, binding, random affixes, vendors, trading, or crafting;
- server authority, multiplayer inventory, or networked persistence.
