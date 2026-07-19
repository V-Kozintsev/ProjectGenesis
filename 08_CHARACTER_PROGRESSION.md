# Character Progression

## Progression Goals

Progression should make the player feel stronger without breaking the early game.

## Leveling

Initial max level for prototype: 5.

Each level can grant:

- more health;
- more attack power;
- optional stat point later;
- skill unlocks at specific levels later.

## Experience Sources

Initial sources:

- enemy kills;
- quest completion.

Later sources:

- exploration discovery;
- crafting;
- achievements.

## Class Growth

Each class should have a simple growth identity:

- Warrior: health and melee power.
- Mage: magic power and mana.
- Rogue: attack speed and critical chance.
- Tank: health and defense.

## Progression Rule

The player should notice level 2 quickly. Early feedback matters more than long-term grind balance.

## Death Penalty

The local prototype removes 10% of the current level's experience requirement on death, with a minimum loss of 10 experience. Both values are editable on `PlayerProgression`.

If current-level experience is insufficient, the remaining loss continues into the previous level. Reaching exactly zero keeps the current level; only a later loss crosses that boundary. Level 1 and zero experience are absolute floors. Death does not remove items, equipment, quests, or quest progress.
