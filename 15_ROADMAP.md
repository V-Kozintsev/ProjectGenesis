# Roadmap

## Current Direction

Sprint 017 Character Creation And Selection is complete. The next proposed implementation is **Character Stats**.

This file preserves completed milestone history and the broad delivery order. The user-facing Russian plan and current dependency decisions live in [32_DEVELOPMENT_PLAN_RU.md](32_DEVELOPMENT_PLAN_RU.md). A future feature listed here is not permission to implement it before its dependencies or current priority.

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

Status: Sprint 014 first enemy variety implemented on 2026-07-19. Version 0.6 remains in progress.

Goal: the starter area feels like a small world.

Features:

- reusable enemy zones and peaceful-village boundaries;
- multiple enemy species and level-aware rewards;
- player skills and clearer class identity;
- stronger item, equipment, and quest authoring foundations;
- a complete and readable starter-region gameplay path.

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

Sprint 010 additions:

- reusable delayed, interval-based health regeneration;
- returning wolves that preserve damage instead of restoring full health immediately;
- recovery that starts after reaching home, does not disable enemy reactions, and is delayed again by renewed damage;
- leash distance measured from the wolf itself so pursuit is visible before return;
- gradual out-of-combat player recovery with damage-delay resets;
- Inspector-authored player and wolf recovery values;
- deterministic validation of healing boundaries and prefab defaults.

Sprint 010 exclusions:

- no potions, food, healing skills, buffs, mana recovery, or status effects;
- no combat log, floating numbers, recovery icons, animation, audio, or final effects;
- no new enemies, attacks, difficulty scaling, or boss mechanics;
- no death penalties, resurrection services, corpse running, or PvP rules;
- no server-authoritative combat state or multiplayer synchronization.

Sprint 011 additions:

- reusable rectangular enemy territory with Inspector tuning and Scene gizmo;
- short random NavMesh roaming around each wolf's home point;
- configurable roaming radius and idle-delay range on the wolf prefab;
- territory-aware detection and return that keep wolves outside the peaceful village;
- independently tunable detection, pursuit, roaming, idle-delay, and territory values;
- territory assignment preserved through enemy respawn;
- deterministic validation of territory dimensions, roaming defaults, and all three spawners.

Sprint 011 exclusions:

- no waypoint patrol routes, packs, formations, fleeing, or coordinated attacks;
- no additional enemy types, animations, audio, or final effects;
- no player death experience or level loss;
- no general PvP, guards, crime, faction, or server-authoritative zone rules;
- no final enemy population balancing.

Sprint 012 additions:

- configurable percentage and minimum experience loss on player death;
- experience loss that can cross a level boundary without falling below level 1;
- level bonus recalculation followed by the existing full-health village respawn;
- persistence through the existing level and experience profile fields;
- deterministic validation of tuning, boundary crossing, the level-1 floor, and disabled penalties.

Sprint 012 exclusions:

- no item, equipment, currency, durability, quest, or quest-progress loss;
- no corpse running, resurrection service, death window, animation, audio, or final effects;
- no PvP-specific or server-authoritative death rules;
- no final live-economy penalty balancing.

Sprint 013 additions:

- Inspector-authored enemy display name, level, and base experience;
- enemy level displayed in the selected-target panel;
- configurable weaker-enemy penalties and stronger-enemy bonuses;
- fixed quest rewards kept separate from scaled combat rewards;
- deterministic validation of prefab defaults and reward boundaries.

Sprint 013 exclusions:

- no automatic enemy stat scaling, randomized levels, elites, or new enemy species;
- no party sharing, contribution tracking, rested experience, boosters, or mentoring;
- no experience notifications, combat log, animation, audio, or final effects;
- no server-authoritative reward calculation or anti-cheat validation.

Sprint 014 additions:

- first reusable level-2 forest-boar prefab with distinct blockout and authored stats;
- one northern boar spawn replacing the third wolf without increasing total enemy density;
- shared combat, AI, territory, recovery, death, cleanup, respawn, and level-scaled experience;
- explicit exclusion from wolf loot and wolf-tail quest progress;
- deterministic validation of the boar prefab and mixed scene population.

Sprint 014 exclusions:

- no boar loot, quests, unique skills, charge attack, sounds, or animations;
- no automatic stat scaling, packs, social aggro, elites, bosses, or random variants;
- no additional spawn points, final assets, or server-authoritative spawning.

Sprint 015 planned additions:

- reusable ScriptableObject skill definitions with future class compatibility;
- one warrior active skill, Heavy Strike;
- selected-enemy skill application with NavMesh approach into range;
- cooldown state and temporary hotbar feedback;
- deterministic validation of skill data, player wiring, hotbar presence, and preserved enemy population.

Sprint 015 exclusions:

- no cave, new zones, decorative world rebuild, new NPCs, quests, enemies, or items;
- no full skill tree, multi-class kits, mana, or other resources;
- no final models, animations, VFX, sound, icons, or UI art;
- no character creation, server, multiplayer, or networked persistence.

Sprint 016 planned additions:

- separate reusable race and class definitions with stable ids;
- one human race, one warrior class, and a default prototype character name;
- runtime player identity with versioned local persistence and version-1 save compatibility;
- minimal identity feedback in the existing inventory window;
- deterministic validation of identity data, prefab wiring, save compatibility, and scene UI.

Sprint 016 exclusions:

- no character creation or selection screen, editable names, additional races, or additional classes;
- no class stat bonuses, weapon-based skill scaling, or new combat content;
- no final character models, equipment visuals, animation, sound, or UI art;
- no accounts, server, multiplayer, or networked persistence.

Sprint 017 planned additions:

- version-3 local profile lifecycle with explicit first-character creation state;
- one editable character name with the currently authored human race and warrior class;
- a blocking creation overlay for new profiles and a selection overlay for existing profiles;
- disabled gameplay input until the player selects the character and presses `Играть`;
- deterministic validation of lifecycle compatibility, name rules, scene wiring, and preserved systems.

Sprint 017 exclusions:

- no multiple character slots, deletion, additional races, classes, or appearance customization;
- no class bonuses, character-stat screen, or weapon-based Heavy Strike scaling;
- no separate account or character scenes, final UI art, server, multiplayer, or networked persistence.

Post-Sprint 009 backlog decisions:

- add explicit inventory-slot selection when different copies can have meaningfully different properties;
- introduce stable item instances before random stats, durability, binding, selling, or trading;
- move enemy population and respawn timing into reusable spawn data instead of relying on scene-only tuning;
- allow sub-10% and sub-1% loot rates, but balance them from expected kills per reward, time-to-kill, quest rewards, economy, and player frustration rather than treating 10% as a minimum;
- consider guaranteed starter rewards or a pity rule separately from ordinary rare drops.

## Planned Delivery Order

The exact sprint numbers after Sprint 015 are provisional. Dependencies and test results decide the final split.

1. Skills foundation and the first active class skill.
2. Character identity data, creation, selection, and readable stats.
3. Stable item instances, explicit inventory selection, equipment expansion, and one consumable.
4. Reusable quest content definitions and a second short quest.
5. A complete starter-region pass with explicit zone rules and one elite encounter.
6. World-history brief and visual style guide before major content or art production.
7. Seamless-world technical slice using streamed or additive neighboring areas.
8. First real cave route and boss only after world-transition, combat, and reward dependencies are ready.
9. Online account, authoritative persistence, and synchronization after the local vertical slice is stable.

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
