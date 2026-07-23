# Mobs, AI, And Bosses

## Enemy Tiers

Every enemy has an authored display name, level, and base experience reward. Level is shown in the selected-target panel and participates in kill-experience calculation; it does not silently rewrite health, damage, loot, or quest data.

### Common

Simple enemies:

- wolf;
- forest boar;
- bandit later.

The starter combat area currently uses two level-1 young wolves and one level-2 forest boar. The boar reuses the shared enemy state machine but has independently authored health, combat stats, movement, recovery, and experience. It intentionally has no wolf loot or wolf-tail quest progress.

Behavior:

- idle;
- roam near home;
- detect player;
- chase;
- attack;
- return home if too far.

### Elite

Stronger enemies with better rewards. The first implementation is the level-3 `Вожак стаи` in its own connected clearing. It is visibly larger and darker, has an explicit Elite rank, 160 health, and a slower 45-second respawn.

Its optional `TelegraphedEnemyAttack` adds `Мощный укус`: the enemy pauses, changes to a warning color for 1.25 seconds, then deals a defense-aware 2x attack-power hit only if the player remains in range. Moving away during the warning avoids all special damage. The same component can later be authored on other elite or boss prefabs without adding special cases to the common enemy state machine.

The Wolf Alpha has its own bounded territory and cannot be dragged into the village. Its data-driven loot table always selects one existing useful reward and intentionally does not advance the current wolf or boar quests.

Behavior additions:

- larger aggro range;
- more health;
- stronger damage;
- one readable special attack where authored.

### Boss

Bosses should be rare and readable.

First boss idea:

- Greyfang, old wolf leader.

Mechanics:

- high health;
- stronger bite;
- howl to summon one weak wolf later;
- respawn timer later.

## AI States

Initial AI states:

- Idle;
- Roam;
- Aggro;
- Chase;
- Attack;
- Return;
- Dead.

## Spawn Rules

Early spawn system:

- fixed spawn points;
- respawn after delay;
- enemies stay near home area;
- shared territory bounds keep enemies outside peaceful spaces.

## Boss Rule

Do not add raid-style complexity before normal combat, health, loot, and respawn work reliably.
