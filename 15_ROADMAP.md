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

Status: Sprint 009 loot-table foundation implemented on 2026-07-19.

Goal: the starter area feels like a small world.

Features:

- village polish;
- enemy zones;
- cave entrance;
- more items;
- more NPCs.

Sprint 007 additions:

- three wolf spawners distributed across the northern combat area;
- six-second corpse cleanup and twelve-second enemy respawn;
- independent 35% sword drop and 70% active-quest trophy progress chances;
- five-tail collection quest with progress saved through the existing profile;
- compact quest tracker for active and ready-to-turn-in states;
- quest trophies applied directly to capped objective progress instead of becoming world or inventory items.

Sprint 007 exclusions:

- no general-purpose loot tables, rarity tiers, or shared world-spawn service;
- no additional enemy species, caves, bosses, NPCs, or quests;
- no polished models, animation, audio, or final UI art;
- no server-authoritative spawning, loot, or quest validation.

Sprint 008 additions:

- persistent display metadata stored with each accepted quest;
- `J` quest journal with dynamic active and completed lists;
- detailed objective, giver, state, and reward display;
- two-step quest abandon confirmation and clean retake from zero;
- short notifications for real objective progress;
- deterministic editor validation of the quest state lifecycle.

Sprint 008 exclusions:

- no additional quests, quest chains, dialogue trees, or map markers;
- no ScriptableObject quest-authoring pipeline or content editor;
- no polished UI art, animation, or audio;
- no server-authoritative quest service or multiplayer synchronization.

Sprint 009 additions:

- reusable ScriptableObject loot tables with direct per-entry chances and a no-drop remainder;
- shared wolf loot configuration editable from the Inspector;
- rusty-sword drop chance reduced from 35% to 10%;
- regular equipment rolls kept separate from conditional quest-trophy progress;
- deterministic boundary validation and a fixed-seed 100,000-roll simulation;
- scene rebuilding that preserves existing loot-table tuning.

Sprint 009 exclusions:

- no guaranteed first weapon, pity counter, or personal loot ownership;
- no currency, vendors, trading, selling, or server economy;
- no random affixes, item levels, crafting, rarity presentation, or final loot art;
- no selectable inventory grid or individual item-instance identity;
- no final rarity-to-drop-rate rules or final economy balancing;
- no centralized spawn authoring or final respawn/population balancing;
- no server-authoritative loot rolls or multiplayer synchronization.

Post-Sprint 009 backlog decisions:

- add explicit inventory-slot selection when different copies can have meaningfully different properties;
- introduce stable item instances before random stats, durability, binding, selling, or trading;
- move enemy population and respawn timing into reusable spawn data instead of relying on scene-only tuning;
- allow sub-10% and sub-1% loot rates, but balance them from expected kills per reward, time-to-kill, quest rewards, economy, and player frustration rather than treating 10% as a minimum;
- consider guaranteed starter rewards or a pity rule separately from ordinary rare drops.

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
