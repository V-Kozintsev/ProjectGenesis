# World Building

## Working Setting

The world is an old frontier region where former kingdoms, abandoned ruins, wild creatures, and small settlements exist beside each other.

The first region is intentionally modest: one village on the edge of dangerous land.

## Starter Region

Working name: Ashvale Frontier.

Key areas:

- Ashvale Village - safe starting hub.
- Old North Road - low-risk combat area.
- Greywood Edge - stronger forest enemies.
- Stonefall Cave - first dangerous enclosed area.
- Forgotten Shrine - future elite or boss location.

## Tone

The tone is grounded fantasy:

- mysterious, but not abstract;
- heroic, but not exaggerated;
- dangerous, but not hopeless;
- readable for players who want clear goals.

## Factions

Initial factions can remain simple:

- Village Guard - protects Ashvale.
- Frontier Traders - economy, supplies, item hooks.
- Old Circle - future magic/lore faction.
- Wild Clans - future orc/dark faction hooks.

## Lore Rule

Lore should support gameplay. If a lore idea does not create a quest, enemy, item, place, or character motivation, it goes to backlog.

Before expanding beyond the starter region, create a short lore bible that defines:

- the important past events and current conflict;
- why the frontier is being settled or defended;
- how the initial factions relate to each other;
- why creatures and ruins are becoming dangerous;
- what role Ashvale Village and the player have in that conflict.

Individual quests, enemies, and locations should grow from this shared history instead of inventing unrelated explanations one at a time.

## Seamless World Direction

The preferred player experience is a connected world:

- outdoor areas connect through physical roads and terrain;
- a cave entrance should lead into the cave without a visible loading screen when technically reasonable;
- position, NavMesh behavior, combat state, and return paths must remain coherent across streamed areas;
- additive scene loading or another chunk-streaming approach should be prototyped before building the first real cave.

Teleports are allowed for deliberate long-distance travel, magical travel, return services, and unstuck recovery. They are not the default replacement for walking into a neighboring area.

Separate loading or instancing remains an acceptable exception when performance, multiplayer encounter ownership, or alternate dungeon states require it. This decision must be made explicitly, not inherited accidentally from scene structure.
