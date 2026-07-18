# Roadmap

## Version 0.1 - First Breath

Status: Sprint 002 starter village blockout implemented on 2026-07-15.

Goal: the player exists in the world.

Features:

- Unity project created;
- starter scene;
- player movement;
- third-person camera;
- basic environment;
- simple UI placeholder.

Sprint 002 additions:

- starter village blockout;
- obstacle-aware NavMesh movement;
- playable-area boundaries;
- camera obstacle protection.

## Version 0.2 - First Contact

Status: Sprint 003 first contact implemented on 2026-07-15.

Goal: the player can interact.

Features:

- interactable NPC;
- dialogue window;
- first quest accepted;
- quest log state.

Sprint 003 exclusions:

- no enemies;
- no combat objective progress;
- no rewards;
- no inventory.

## Version 0.3 - First Fight

Status: Sprint 004 first fight implemented on 2026-07-18.

Goal: the player can fight and win.

Features:

- health system;
- basic enemy AI;
- basic attack;
- enemy death;
- experience reward.

Sprint 004 additions:

- one hostile wolf on the north road;
- click selection, automatic approach, and basic attacks;
- enemy idle, chase, attack, return, and death states;
- player death and village respawn;
- combat HUD for health, target health, level, and experience.

Sprint 004 exclusions:

- no loot, inventory, or equipment;
- no quest objective progress or completion;
- no combat skills or multiple enemies;
- no enemy respawn or save/load.

## Version 0.4 - First Reward

Status: Sprint 005 first reward implemented on 2026-07-18.

Goal: the player receives meaningful progress.

Features:

- inventory;
- loot drop;
- equipment slot;
- stat update from equipment.

Sprint 005 additions:

- north village gate and a separate prototype combat area;
- wolf leash that prevents pursuit into the village;
- authored `Rusty Sword` item definition;
- visible click-to-collect wolf loot;
- eight-slot player inventory;
- one main-hand equipment slot;
- attack power increase from equipped weapon;
- compact inventory window opened with `I` or an on-screen button.

Sprint 005 exclusions:

- no general peace-zone or PvP-zone rules;
- no armor or additional equipment slots;
- no trading, currency, vendors, or crafting;
- no enemy respawn or save/load;
- no quest completion or quest rewards.

## Version 0.5 - First Loop

Status: Sprint 006 first loop implemented on 2026-07-18.

Goal: the first complete RPG loop works.

Features:

- kill quest;
- quest completion;
- reward;
- level up;
- automatic persistence.

Sprint 006 additions:

- one complete accept, kill, return, and turn-in quest flow;
- tracked wolf objective progress and one-time quest reward;
- experience thresholds, level up, and health and attack growth;
- automatic persistence for position, progression, quest state, inventory, and equipment;
- replaceable persistence interface with local JSON as a temporary offline stand-in for the future authoritative server;
- development-only command for clearing the local prototype profile.

Sprint 006 exclusions:

- no player-facing save/load button or save slots;
- no authentication, account service, database, or real multiplayer backend;
- no enemy respawn, world-state persistence, multiple quests, or dialogue trees;
- no additional items, equipment slots, skills, or character creation.

## Version 0.6 - First Zone

Goal: the starter area feels like a small world.

Features:

- village polish;
- enemy zones;
- cave entrance;
- more items;
- more NPCs.

## Version 1.0 - Vertical Slice

Goal: a polished small RPG slice.

Features:

- character creation;
- several enemy types;
- several quests;
- first boss;
- UI pass;
- audio pass;
- save/load stable;
- performance acceptable.
