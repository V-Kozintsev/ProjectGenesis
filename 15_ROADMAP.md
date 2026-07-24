# Roadmap

## Current Direction

**Sprint 032: Merchant Shop Foundation** is complete and accepted. **Sprint 033: NPC Interaction Hub** is in implementation so quest and trade features open from explicit NPC action choices.

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

Sprint 018 delivered additions:

- authored warrior health and attack bonuses while preserving existing level-1 totals;
- explicit base, class, level, and equipment attack contributions;
- Heavy Strike scaling from current attack power and target defense;
- a temporary character-stats window with health, attack, defense, experience, timing, and skill power;
- deterministic validation of exact stat composition, damage results, prefab defaults, and scene wiring.
- movable character, inventory, and quest windows plus a data-driven Heavy Strike hover tooltip.

Sprint 018 exclusions:

- no attributes, distributable points, critical hits, random damage, resistances, or armor types;
- no mana, rage, stamina, additional classes, skills, equipment slots, enemies, items, quests, or zones;
- no final models, animations, VFX, sound, icons, UI art, server, multiplayer, or networked persistence.

Sprint 019 delivered additions:

- stable runtime identity for every collected item copy;
- profile version 4 with exact item-instance and equipped-instance persistence;
- migration support for version-1 through version-3 profiles;
- eight explicit inventory slot buttons with concrete-item selection;
- deterministic validation of duplicate rejection, migration, exact-instance equipment, prefab defaults, and scene wiring.

Sprint 019 exclusions:

- no additional equipment slots, armor, consumables, vendors, currency, selling, trading, crafting, stacks, sorting, or drag-and-drop;
- no random affixes, durability, binding, item levels, new content, final UI art, server, multiplayer, or networked persistence.

Sprint 020 delivered additions:

- eight persistent inventory positions, including empty gaps;
- drag-and-drop movement into an empty position and swapping between occupied positions;
- profile version 5 with slot-index persistence and version-1 through version-4 migration;
- a +7 Worn Axe definition and a reusable 20% boar loot table;
- deterministic validation of movement, swaps, exact equipment, migration, data, prefabs, and scene wiring.

Sprint 020 exclusions:

- no additional equipment slots, armor, consumables, stacks, sorting buttons, vendors, currency, selling, trading, crafting, or item destruction;
- no random affixes, durability, binding, item levels, new quests or zones, final UI art, server, multiplayer, or networked persistence.

Sprint 021 delivered additions:

- weapon, body-armor, and consumable item categories in one reusable definition;
- one persistent body-equipment slot and equipment defense bonus;
- a +3 Worn Leather Armor and a 30-health Minor Healing Potion;
- successful-use consumption plus clear refusal without item loss;
- profile version 6 with body-instance persistence and version-1 through version-5 migration;
- deterministic validation of data, equipment restrictions, defense, healing, removal, migration, loot, prefabs, and scene wiring.

Sprint 021 exclusions:

- no further equipment slots, two-handed rules, class restrictions, stacks, quickbar consumables, cooldowns, buffs, mana, vendors, currency, selling, trading, crafting, durability, binding, random affixes, final UI art, server, multiplayer, or networked persistence.

Sprint 022 delivered additions:

- preserve the corpse state until the player confirms resurrection;
- block movement, combat, skills, NPC interaction, loot pickup, consumables, and regeneration while dead;
- apply the existing experience penalty once on death;
- show a centered temporary death window with `Воскреснуть в деревне`;
- restore full health and move to the configured village spawn point only after confirmation;
- deterministic validation of one-time penalty, no automatic movement, resurrection, prefab wiring, scene UI, and relevant regressions.

Sprint 023 delivered additions:

- equipped weapon and body armor are exact instances stored outside the eight bag positions;
- safe equip replacement returns the previous item to the released bag position;
- unequip returns an item only when the bag has space and otherwise preserves equipment;
- one movable inventory window presents the bag, main-hand and body slots, and a simple selected-item comparison;
- selected bag items can be dropped into the world as the same exact instance or dragged onto the trash target for centered permanent-removal confirmation;
- profile version 7 separates bag and equipment records while migrating version-1 through version-6 profiles;
- deterministic validation covers transfers, replacement, full-bag refusal, exact-instance drop and collection, permanent removal, migration, scene wiring, and relevant regressions.

Post-Sprint 023 proposed sequence:

- Sprint 024 Local Message Feed Foundation: completed and accepted;
- Sprint 025 Reusable Quest Content Definitions: completed and accepted;
- Sprint 026 Parallel Quests And Second Quest: completed and accepted;
- Sprint 027 Zone Rules Foundation: completed and accepted with authored peaceful/combat definitions, prioritized volumes, shared combat authorization, transition feedback, and safe resurrection wiring.
- Sprint 028 First Elite Encounter: in implementation with one level-3 Wolf Alpha, separate clearing territory, telegraphed powerful bite, guaranteed existing-item reward, and deterministic validation.

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
4. Explicit player-death state and a confirmed respawn choice.
5. Separate character-equipment presentation from bag storage.
6. Typed local message-feed foundation before network chat.
7. Reusable quest content definitions and a second short quest.
8. A complete starter-region pass with explicit zone rules and one elite encounter.
9. World-history brief and visual style guide before major content or art production.
10. Seamless-world technical slice using streamed or additive neighboring areas.
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
