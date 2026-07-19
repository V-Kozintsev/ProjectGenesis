# Sprint 011 - Enemy Territory

## Sprint Goal

Make wolves feel present in the world while keeping their movement predictable and safely outside the peaceful village.

## Task 1 - Add Explicit Enemy Territory

Acceptance criteria:

- the northern combat area has one reusable `EnemyTerritory` component;
- territory dimensions are editable in the Inspector and visible as a selected Scene gizmo;
- wolves can detect and pursue the player only while the player is inside their territory;
- crossing into the peaceful village makes an engaged wolf return home;
- territory checks do not change player movement or camera controls.

## Task 2 - Add Idle Roaming

Acceptance criteria:

- an unengaged wolf alternates between idle pauses and short NavMesh walks;
- roaming radius and minimum/maximum idle delay are editable on the enemy prefab;
- every roaming destination is inside both the wolf's home radius and its assigned territory;
- unreachable random points are rejected instead of producing partial paths;
- detecting or receiving an attack from the player immediately interrupts roaming.

## Task 3 - Preserve Combat And Recovery

Acceptance criteria:

- click selection, approach, attacks, health, loot, quests, corpse cleanup, and respawn remain unchanged;
- Chase, Attack, Return, Dead, and delayed regeneration continue to follow Sprint 010 rules;
- a wolf returning home cannot roam until the return completes;
- a respawned wolf receives the same territory and roaming configuration.

## Task 4 - Keep The Feature Verifiable

Acceptance criteria:

- the starter-village builder reproduces the territory and prefab defaults;
- an editor command validates the wolf roaming values, territory dimensions, and all three spawner references;
- the project compiles and the validator passes in Unity batch mode.

## Initial Prototype Tuning

- detection radius: 2.2 metres;
- leash radius: 6 metres from home, additionally limited by the enemy territory;
- roaming radius: 2.4 metres from home;
- idle delay: random value from 0.5 to 1.5 seconds;
- territory center: northern combat-area center;
- territory size: 15.6 by 9.2 metres with 0.2-metre inner padding;
- recovery, corpse lifetime, and respawn delay remain unchanged.

## Not In This Sprint

- waypoint patrol routes, packs, formations, fleeing, or coordinated attacks;
- additional enemies, animations, audio, final effects, or difficulty scaling;
- player death experience loss or level loss;
- general PvP, guards, crime, faction, or server-authoritative zone rules;
- final spawn population balancing.
