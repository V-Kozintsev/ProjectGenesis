# Sprint 021 - Equipment Slots And First Consumable

## Sprint Goal

Extend the reusable item foundation with one additional equipment slot and one consumable action while preserving exact item instances, inventory positions, combat stats, and save compatibility.

## Task 1 - Generalize Item Data

Acceptance criteria:

- item definitions distinguish weapons, body armor, and consumables;
- each category exposes only the gameplay values it needs: attack, defense, or healing;
- a Worn Leather Armor definition has a stable id and provides `+3` defense;
- a Minor Healing Potion definition has a stable id and restores `30` health;
- invalid or non-positive authored values are rejected by deterministic validation.

## Task 2 - Add A Body Equipment Slot

Acceptance criteria:

- the existing main-hand slot continues to accept only weapons;
- a body slot accepts only armor and keeps the exact selected item instance equipped;
- equipping or removing body armor updates total defense without changing base defense;
- moving equipped items between inventory positions does not unequip them;
- the inventory and character-stat windows show the current body armor and defense bonus clearly.

## Task 3 - Add One Consumable Action

Acceptance criteria:

- selecting a healing potion offers `Use` instead of `Equip`;
- successful use heals without exceeding maximum health and removes exactly one potion instance;
- use at full health, while dead, or with an invalid item is rejected without consuming the item;
- the inventory gives short success or refusal feedback;
- a Play Mode development command adds the armor and two separate potions, then safely damages the player for a deterministic success-and-refusal check.

## Task 4 - Preserve And Verify State

Acceptance criteria:

- profile version 6 stores the equipped body-instance id;
- profiles from versions 1-5 remain supported and restore with an empty body slot;
- current profiles restore item positions, main hand, body armor, and remaining consumables exactly;
- the starter-village builder authors both new items, loot-table entries, catalog references, gameplay components, and UI wiring;
- an editor validator checks data, category restrictions, defense changes, consumable success and refusal, exact-instance removal, migration, prefab defaults, loot data, and scene wiring;
- the project compiles and Sprint 021 plus relevant regression validators pass in Unity batch mode.

## Not In This Sprint

- helmet, gloves, boots, jewelry, off-hand rules, two-handed weapons, armor sets, or class restrictions;
- consumable stacks, quickbar binding, cooldowns, buffs, mana, resurrection, food, or multiple potion tiers;
- vendors, currency, selling, trading, crafting, item destruction, durability, binding, or random affixes;
- new enemies, NPCs, quests, zones, models, animations, VFX, sound, icons, tooltips, or final UI art;
- server, multiplayer, or networked persistence.
