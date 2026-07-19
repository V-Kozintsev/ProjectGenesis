# Sprint 010 - Combat Recovery And Leash

## Sprint Goal

Make combat disengagement predictable: enemies retain damage while returning home, recovery starts only after a delay, and the player slowly recovers after leaving combat.

## Task 1 - Add Reusable Health Regeneration

Acceptance criteria:

- regeneration is a reusable component driven by Inspector values;
- delay, healed amount, and tick interval are configurable;
- taking damage restarts the recovery delay;
- regeneration never revives a dead character and never exceeds maximum health;
- health changes continue to update the existing HUD.

## Task 2 - Preserve Enemy Damage During Return

Acceptance criteria:

- a wolf that reaches its own leash boundary enters the existing `Return` state, so a fleeing player does not cancel pursuit before the wolf has actually chased;
- the wolf keeps its current health while travelling home;
- the wolf cannot attack or regenerate while returning;
- approaching the returning wolf again inside its leash can resume combat;
- the home point uses the wolf's actual NavMesh placement so the `Return` state cannot wait on an unreachable offset;
- reaching home starts a fresh recovery delay instead of restoring full health immediately;
- an idle wolf at home does not restart `Return` merely because the player remains outside its leash;
- a recovering wolf still detects, pursues, and attacks a nearby player;
- proximity alone does not pause recovery;
- every new hit restarts the wolf's recovery delay, so sustained attacks prevent healing.

## Task 3 - Add Player Out-Of-Combat Recovery

Acceptance criteria:

- the player does not regenerate while actively attacking a selected enemy;
- ending combat or losing the target starts a fresh recovery delay;
- taking new damage restarts that delay;
- after the delay, health returns gradually in visible steps;
- death and village respawn still restore full health through the existing flow.

## Task 4 - Keep Recovery Tunable And Verifiable

Acceptance criteria:

- the player and wolf prefabs expose their recovery settings in the Inspector;
- detection radius, leash radius, corpse lifetime, and spawner respawn delay remain independently editable;
- an editor command validates health boundaries and the saved prefab settings;
- the starter-village builder reproduces the intended defaults.

## Initial Prototype Tuning

- wolf recovery: 5-second delay, 3 health per 1-second tick;
- player recovery: 8-second delay, 2 health per 1-second tick;
- wolf leash radius remains 3.5 metres and is measured from the wolf's current position to its home point;
- corpse lifetime remains 6 seconds;
- wolf respawn delay remains 12 seconds.

## Preserve Existing Behavior

- click targeting and double-click actions remain unchanged;
- movement, NavMesh avoidance, loot, quests, progression, inventory, equipment, saving, and UI continue to work;
- `.vsconfig` remains outside project work.

## Not In This Sprint

- health or mana potions, food, buffs, or healing skills;
- combat log UI, floating numbers, status icons, or final effects;
- enemy difficulty scaling, new attacks, new enemies, or boss mechanics;
- death penalties, corpse running, resurrection services, or PvP rules;
- server-authoritative combat state or multiplayer synchronization.
