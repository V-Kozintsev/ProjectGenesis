# Sprint 028 - First Elite Encounter

## Sprint Goal

Add one reusable elite-enemy encounter to the connected starter region, giving the player a readable threat, a movement-based response, and a worthwhile reward without introducing a boss, quest, or new scene.

## Task 1 - Add An Elite Enemy Rank

Acceptance criteria:

- enemies have an explicit `Common`, `Elite`, or `Boss` rank;
- existing wolves and the forest boar remain common;
- the first elite is the level-3 `Вожак стаи`;
- the target panel clearly labels an elite without final UI art;
- rank does not silently determine health, damage, experience, loot, or behavior.

## Task 2 - Add A Readable Special Attack

Acceptance criteria:

- the Wolf Alpha has one optional reusable telegraphed attack component;
- `Мощный укус` has an authored initial delay, cooldown, windup, range, and attack-power multiplier;
- the enemy stops and changes color during the 1.25-second warning;
- moving outside the hit range before resolution avoids all special damage;
- remaining in range takes defense-aware damage using the shared combat formula;
- warning, hit, and avoided results appear in the local combat feed;
- normal enemies continue to use their existing basic attack behavior.

## Task 3 - Add A Side Clearing

Acceptance criteria:

- the existing north combat area gains a walkable east-side opening;
- a small connected clearing provides enough room to react and move away;
- ordinary wolves and the boar keep their existing shared territory and spawners;
- the Wolf Alpha uses a separate territory, cannot pursue into the village, and returns to its clearing;
- the clearing is another volume of the existing `Северные окрестности` combat zone;
- no new scene, loading transition, cave, village, or decorative world overhaul is introduced.

## Task 4 - Add A Useful Reward

Acceptance criteria:

- defeating the Wolf Alpha grants 90 base experience;
- the elite respawns after 45 seconds;
- one existing useful item always drops: Worn Axe 50%, Worn Leather Armor 30%, or Minor Healing Potion 20%;
- loot remains data-driven through its own validated loot-table asset;
- the elite does not advance the existing wolf or boar quests.

## Task 5 - Validate Regressions

Acceptance criteria:

- a deterministic editor validator checks rank, stats, special-attack data, exact hit damage, successful avoidance, loot, clearing, territory, zone, and spawner wiring;
- the starter-village builder recreates the elite prefab, loot asset, connected clearing, zone references, and spawn deterministically;
- older validators accept additional intentional combat volumes and elite spawners while retaining exact regular-enemy checks;
- the relevant Sprint 008 through Sprint 028 regression suite passes;
- the user verifies appearance, target rank, warning readability, hit, avoidance, return behavior, loot, and respawn in Play Mode.

## Not In This Sprint

- a quest for the elite, new NPC, new item, unique equipment, currency, vendor, or economy;
- summons, phases, area attacks, crowd control, boss health UI, or raid mechanics;
- final character models, animations, VFX, audio, icons, or UI art;
- a cave, separate location, additive loading, world streaming, or decorative region rebuild;
- server authority, multiplayer synchronization, shared respawn state, or network loot.
