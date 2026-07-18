# Sprint 005 - First Reward

## Sprint Goal

Complete the first meaningful reward loop: defeat the wolf outside the village, collect one dropped weapon, equip it, and see the player's attack power increase.

## Tasks

### Task 1 - Document Sprint Scope

Acceptance criteria:

- the reward loop is small and visually testable;
- item data, inventory state, equipment state, and UI responsibilities remain separate;
- exclusions prevent trading, crafting, and save/load from entering this sprint.

### Task 2 - Separate Village And Combat Space

Move the wolf beyond the peaceful village blockout.

Acceptance criteria:

- the north village boundary has a clear gate;
- a small combat area is visible beyond the gate;
- the wolf starts in the combat area;
- the wolf returns home before it can chase the player into the village;
- this is a spatial prototype, not a general peace-zone system.

### Task 3 - Add Item And Inventory Data

Add the first authored item and a small player inventory.

Acceptance criteria:

- `Rusty Sword` is represented by an item definition asset;
- the player inventory has a visible capacity of eight slots;
- adding an item reports an inventory change;
- full inventory rejects additional items without deleting the world drop.

### Task 4 - Add Wolf Loot Drop

Drop the sword into the world after the wolf dies.

Acceptance criteria:

- the wolf drops exactly one visible sword pickup;
- the defeated wolf body disappears after a short adjustable delay without removing its loot;
- clicking the pickup makes the player approach it using NavMesh;
- the pickup enters the inventory only when the player is close enough;
- repeated clicks cannot duplicate the item.

### Task 5 - Add Main-Hand Equipment

Allow the sword to be equipped and removed.

Acceptance criteria:

- the inventory window shows the collected sword;
- the sword can be equipped into one main-hand slot;
- equipping adds the sword attack bonus to player attack power;
- unequipping removes the bonus and keeps the sword in the inventory;
- no armor or additional equipment slots are implemented.

### Task 6 - Add Minimal Inventory UI

Provide a compact interface for testing the reward.

Acceptance criteria:

- `I` and an on-screen inventory button open and close the window;
- the window shows capacity, current attack power, main-hand equipment, and inventory contents;
- item actions have immediate visible feedback;
- the window can be closed with its close button or `I`.

### Task 7 - Update Project Notes

Acceptance criteria:

- README explains the loot and inventory controls;
- roadmap records Sprint 005 implementation status;
- changelog records the first reward loop.

## Not In This Sprint

- general peace-zone or PvP-zone rules;
- item stacks, drag and drop, or item sorting;
- armor and additional equipment slots;
- vendors, currency, trading, or crafting;
- quest objective completion or quest rewards;
- enemy respawn;
- save/load;
- final art, item icons, or combat animations;
- multiplayer.
