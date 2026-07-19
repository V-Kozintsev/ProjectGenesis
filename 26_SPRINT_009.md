# Sprint 009 - Loot Tables And Balance

## Sprint Goal

Replace the wolf's hardcoded regular-item roll with a reusable, Inspector-tunable loot table and establish a safer baseline for future item balancing.

## Task 1 - Add A Data-Driven Loot Table

Acceptance criteria:

- regular enemy loot is described by a `LootTableDefinition` asset;
- each entry stores an item, rarity category, and direct drop chance from `0` to `1`;
- the unused probability is treated as a no-drop result;
- one table roll produces at most one regular item;
- invalid tables with missing items or a total chance above 100% can be detected clearly.

## Task 2 - Rebalance Wolf Weapon Drops

Acceptance criteria:

- the wolf uses `LT_Wolf` instead of a hardcoded item and chance;
- the initial `Rusty Sword` chance is reduced from 35% to 10%;
- the chance is editable on the loot-table asset in the Inspector;
- rebuilding the starter village preserves later Inspector tuning instead of restoring 35%;
- all wolf spawners continue to use the same shared table.

## Task 3 - Preserve Quest Loot Separation

Acceptance criteria:

- wolf-tail progress remains a separate conditional quest roll;
- changing regular loot chances does not affect quest progress;
- regular equipment still appears in the world and uses the existing pickup flow;
- quest trophies still never appear in the world or normal inventory.

## Task 4 - Add Deterministic Loot Validation

Acceptance criteria:

- an editor command validates the saved wolf loot-table asset;
- boundary checks confirm a 10% entry drops only inside its probability range;
- a fixed-seed simulation reports the observed rate across many rolls;
- validation fails with a useful message if the asset, item, chance, or total probability is wrong.

## Preserve Existing Behavior

- combat, corpse cleanup, respawn, quest progress, inventory, equipment, saving, and UI continue to work;
- the sword remains a visible click-to-collect world object;
- `.vsconfig` remains outside project work.

## Not In This Sprint

- a guaranteed first weapon, pity counter, or per-player loot ownership;
- currency, vendors, trading, item selling, or a server economy;
- randomized item stats, affixes, crafting, rarity colors, or final loot visuals;
- selectable inventory slots and individual identities for duplicate item copies;
- final drop-rate balancing, including sub-1% rare rewards and rarity-to-chance rules;
- centralized enemy-spawn data and final respawn/population balancing;
- server-authoritative random rolls or multiplayer synchronization.
