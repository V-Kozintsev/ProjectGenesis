# Sprint 027 - Zone Rules Foundation

## Sprint Goal

Give the starter village and its northern wilderness explicit reusable gameplay rules so peaceful protection, combat permission, transition feedback, and safe resurrection no longer depend only on enemy placement.

## Task 1 - Add Reusable Zone Data

Acceptance criteria:

- a `WorldZoneDefinition` asset owns a stable id, display name, zone type, enter message, and peaceful-combat refusal message;
- `Peaceful` and `Combat` are explicit rule types rather than names inferred from scene objects;
- invalid ids, names, messages, or peaceful-zone refusal feedback are rejected;
- the starter village and north wilds use separate Inspector-authored assets;
- faction, PvP, dungeon, weather, music, level, and server rules remain future extensions.

## Task 2 - Resolve The Player's Current Zone

Acceptance criteria:

- reusable scene volumes resolve the zone from world position without physics-trigger timing;
- overlapping volumes use an explicit priority so a small peaceful location can override a larger combat region later;
- the starter village volume has higher priority than the north combat volume;
- a configured combat fallback prevents unmapped wilderness gaps from becoming accidental safe areas;
- entering either authored zone publishes one short system message to the existing local feed.

## Task 3 - Enforce Peaceful Protection

Acceptance criteria:

- one click may still select an enemy while the player is in the village;
- starting normal auto-attack from the peaceful zone is refused without clearing the selected target;
- Heavy Strike is refused in the peaceful zone before approach, damage, or cooldown starts;
- an active normal attack stops if the player crosses into a peaceful zone;
- enemies stop aggression and return instead of chasing or damaging a protected player;
- the existing combat flow remains unchanged in the north wilds.

## Task 4 - Keep Resurrection Safe

Acceptance criteria:

- the existing explicit resurrection choice remains unchanged;
- the configured village spawn point is inside the peaceful volume and outside the north combat volume;
- the player's zone is refreshed immediately after resurrection;
- no profile migration or new death penalty is introduced.

## Task 5 - Validate Regressions

Acceptance criteria:

- an editor validator checks both zone assets, invalid data rejection, overlap priority, combat authorization, transition feedback, volumes, player prefab, entry gate, and peaceful respawn wiring;
- the starter-village builder recreates both assets, volumes, prefab wiring, and scene references deterministically;
- the existing scene is migrated without a full-scene file-id rewrite;
- the relevant Sprint 008 through Sprint 027 regression suite passes;
- the user verifies transitions, peaceful combat refusal, wilderness combat, enemy return, and resurrection in Play Mode.

## Not In This Sprint

- an elite enemy, boss, new ordinary enemy, NPC, quest, item, reward, building, cave, or world expansion;
- final zone borders, minimap overlays, art, VFX, music, ambient audio, or loading transitions;
- PvP, duels, factions, guards, crime, reputation, sanctuary skills, or player-owned areas;
- additive scene loading, world streaming, server authority, multiplayer synchronization, or network persistence.
