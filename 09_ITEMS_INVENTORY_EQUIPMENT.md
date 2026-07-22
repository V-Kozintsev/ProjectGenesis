# Items, Inventory, And Equipment

## Item Types

Implemented prototype item types:

- weapon;
- armor;
- consumable;
- quest item remains objective progress rather than normal inventory content;
- material later.

## Equipment Slots

Current slots:

- main hand;
- chest;

Add later:

- legs optional;
- boots optional.
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

The current wolf table gives the `Rusty Sword` a 10% chance and the `Minor Healing Potion` a 20% chance. The boar table gives the `Worn Axe` a 20% chance and `Worn Leather Armor` a 15% chance. One cumulative roll still produces at most one regular item. Quest trophies are not regular loot-table entries: they remain conditional objective progress and never become world or inventory items.

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

The current prototype stores every collected copy as a stable `ItemInstance` in one of eight persistent positions. The player can select, move, or swap exact copies. Main-hand equipment accepts only weapons; body equipment accepts only armor. The Worn Leather Armor contributes `+3` equipment defense without changing base defense.

The Minor Healing Potion restores `30` health and is removed only after successful use. Full health, death, an invalid instance, or a non-consumable category rejects the action without deleting anything. Potions remain separate instances in this sprint; stacking and quickbar use are later systems.
