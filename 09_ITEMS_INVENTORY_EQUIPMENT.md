# Items, Inventory, And Equipment

## Item Types

Initial item types:

- weapon;
- armor;
- consumable;
- quest item;
- material later.

## Equipment Slots

Start with:

- main hand;
- chest;
- legs optional;
- boots optional.

Add later:

- off hand;
- helmet;
- gloves;
- rings;
- amulet.

## Item Rarity

Initial rarity:

- Common;
- Uncommon;
- Rare.

Later:

- Epic;
- Legendary.

## Loot Philosophy

Early loot should be sparse but meaningful.

The player should understand:

- why an item is better;
- whether it fits the class;
- what changed after equipping it.

## Loot Tables

Regular enemy drops are authored in `LootTableDefinition` assets. Each entry contains an item, a rarity category, and a direct chance from `0` to `1`. Entries are evaluated as one cumulative roll, so one enemy death produces at most one regular item and any unused probability means no regular drop.

The current wolf table gives the `Rusty Sword` a 10% chance. Quest trophies are not regular loot-table entries: they remain conditional objective progress and never become world or inventory items.

Ten percent is starter-zone prototype tuning, not a technical or design minimum. Loot-table entries support any value from `0` to `1`; for example, `0.001` means 0.1%. Final rates must be balanced later against item value, enemy kill time, player population, trading, and any guaranteed-reward or pity rules instead of being derived from the rarity label alone.

Existing loot-table assets are not rewritten by scene rebuilding. Designers tune them directly in the Inspector and can run the Sprint 009 validator to check probability boundaries and a fixed-seed simulation.

## Inventory Rules

Prototype inventory can be slot-based.

Needed operations:

- add item;
- remove item;
- move item;
- equip item;
- unequip item;
- check if inventory has space;
- serialize inventory for saving.

The current prototype can store repeated references to the same item definition, but its compact UI intentionally operates on the first item only. Identical `Rusty Sword` copies currently have identical stats, so choosing between them would not change combat. A later inventory pass must add selectable slots and stable item instances before supporting unique rolls, durability, binding, selling, or trading.
