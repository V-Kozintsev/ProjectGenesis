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

Stronger enemies with better rewards.

Behavior additions:

- larger aggro range;
- more health;
- stronger damage;
- simple special attack later.

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
