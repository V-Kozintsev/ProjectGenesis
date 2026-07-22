# Project Genesis

Project Genesis is a small-scope online RPG prototype built in Unity. The goal is not to copy a giant MMORPG at full scale, but to create a focused, playable foundation: character creation, a starter village, exploration, combat, progression, loot, quests, and a clean architecture that can grow without collapsing.

## Current Stage

Sprint 024 Local Message Feed Foundation is complete and accepted after the deterministic Sprint 008-024 regression suite and the user's Play Mode check. Gameplay publishes typed session-local system, loot, combat, chat, and announcement events into a movable, filterable, scrollable lower-left feed with focused-input blocking and local prototype chat. Sprint 025 Reusable Quest Content Definitions is the next proposed step; real player chat and server-authenticated global announcements remain later network features.

The Russian development sequence, seamless-world direction, lore and visual-production gates, and Codex task-handoff rules are summarized in [32_DEVELOPMENT_PLAN_RU.md](32_DEVELOPMENT_PLAN_RU.md).

## Prototype Controls

- Open `Assets/ProjectGenesis/Scenes/StarterVillage.unity` before entering Play mode.
- A new local profile opens character creation first. Enter a name, press `Создать персонажа`, review the created human warrior, then press `Играть`; later sessions begin at character selection.
- Left-click the ground to move the character. This is the primary control style, inspired by classic point-and-click MMORPGs such as Lineage 2.
- Click movement uses NavMesh navigation, so the character routes around simple obstacles and ignores unreachable points.
- Hold the right mouse button and move the mouse to rotate the camera horizontally and vertically. A short right-click returns it behind the character.
- Use the mouse wheel to zoom the camera in and out. Click the wheel to switch between the rear and front views.
- WASD or arrow keys are optional fallback controls and cancel the current click destination.
- Leave the village through the north gate to reach the first combat area. Its three wolves are spread across the zone and return home before they can chase the player back into the village.
- Idle wolves alternate between short pauses and small NavMesh walks around their spawn points. Their roaming radius and pause range are editable on the wolf prefab.
- Click a grey wolf once to select it. Click the selected wolf again at any time to run into range and attack automatically; a quick double-click performs both steps.
- Click `Heavy Strike` on the temporary hotbar to use the first warrior skill on the selected enemy. If the enemy is out of skill range, the character approaches automatically; after the skill, basic autoattacks continue against the same living target. The button shows cooldown seconds while unavailable.
- Press `C` or click `Характеристики [C]` to inspect health, attack, defense, experience, attack timing, and Heavy Strike power. Equipping the Rusty Sword raises level-1 attack from 14 to 18 and Heavy Strike damage against the current wolf from 22 to 29. Hover Heavy Strike for its description, damage, range, and cooldown.
- Drag the title area of the character-stats, inventory, or quest-journal window to move it; the window is kept within reachable screen bounds.
- Clicking the ground or using WASD stops the current approach or attack but keeps the selected target.
- Press `Esc`, click the `X` in the target panel, or select another creature to clear or switch the current target.
- The combat HUD shows player health, selected-enemy health, level, and experience.
- Player and selected-enemy health bars visibly shrink on damage and grow during recovery.
- A level 1 hero receives the young wolf's full 20 base experience. The same level 1 wolf awards 15 experience at player level 2, 10 at level 3, 5 at level 4, and 2 at level 5. Its body disappears after six seconds and its spawner creates a new wolf after twelve seconds.
- Selected enemies show their authored level beside their name. Enemy level and base experience are editable on `EnemyBrain`; weaker-enemy penalties and stronger-enemy bonuses are editable on `PlayerProgression`.
- The northern spawn point creates a level-2 `Forest Boar` with 60 health and 30 base experience. It does not drop wolf loot or advance the wolf-tail quest.
- If the player dies, the character loses 10% of the current level requirement with a minimum loss of 10 experience, stays at the death position, and shows a centered resurrection window. Click `Воскреснуть в деревне` to return to the village spawn point with full health.
- Death-loss values are editable on `PlayerProgression` in the player prefab. Loss can cross into the previous level, but the character never falls below level 1; inventory, equipment, and quests are preserved.
- A retreating wolf keeps its remaining health. Its leash measures how far the wolf itself has travelled from home, so it visibly pursues before returning. It can be re-engaged while returning by approaching inside its leash. At home it waits five seconds, then restores 3 health every second. It still attacks nearby players while recovering, and every new hit restarts the five-second healing delay.
- After active combat ends, the player waits eight seconds and then restores 2 health every second. New damage restarts the delay, while confirmed village resurrection restores full health independently of regeneration.
- Recovery values are editable through `HealthRegeneration` on the player and wolf prefabs. Use `Project Genesis > Sprint 010 > Validate Combat Recovery` to verify saved defaults and health boundaries.
- The selected `Zone_NorthCombat` object shows its enemy-territory boundary as a green Scene gizmo. Use `Project Genesis > Sprint 011 > Validate Enemy Territory` to verify the zone, spawners, and roaming defaults.
- Each wolf has a 10% chance to drop a visible `Rusty Sword` and a 20% chance to drop a `Minor Healing Potion`; one kill produces at most one regular item. Click the drop to approach it and collect it.
- Regular wolf loot is configured in `Assets/ProjectGenesis/Data/LootTables/LT_Wolf.asset`. Select that asset and edit an entry's `Drop Chance` value in the Inspector; `0.1` means 10%.
- Rebuilding the starter village preserves edits to an existing loot-table asset. Use `Project Genesis > Sprint 009 > Validate Wolf Loot Table` to check the table and run a fixed-seed 100,000-roll simulation.
- While the Village Elder's quest is active and incomplete, each wolf also has a 70% chance to add a `Wolf Tail` directly to objective progress. Quest trophies do not appear on the ground or occupy normal inventory slots, and progress stops at `5 / 5`.
- Press `I` or click `Инвентарь [I]` to open one movable window containing the eight-slot bag plus separate `Правая рука` and `Тело` equipment slots.
- Click an occupied inventory slot to select that exact item. `Надеть` transfers a weapon or armor from the bag into its character slot; the equipped item no longer occupies one of the eight bag positions.
- Drag an occupied slot onto an empty slot to move it, or onto another occupied slot to swap the two exact item instances. Reordered positions persist after restarting Play Mode.
- Boars have a 20% chance to drop a `Потёртый топор` with `+7` attack; the Rusty Sword remains `+4` attack.
- Boars also have a 15% chance to drop `Потёртая кожаная броня`. Select it and click `Надеть` to transfer it into the body slot and add `+3` defense without replacing the equipped weapon.
- Select `Малое лечебное зелье` and click `Использовать` to restore 30 health. It is consumed only when healing succeeds; full health and death show a refusal and preserve the item.
- During Sprint 023 Play Mode testing, use `Project Genesis > Sprint 023 > Prepare Play Mode Test` to add a sword, axe, and armor without waiting for random drops.
- Select a weapon or armor in the bag, then select the compatible equipment slot on the right to see its bonus comparison. Use `Надеть`, `Заменить`, or `Снять`; a removed item returns to the bag, and a full bag refuses removal without losing it.
- `Выбросить` transfers the selected exact item to a collectible object beside the character. Click `Корзина` or drag a bag slot onto it to open a centered confirmation; `Удалить` permanently removes only that item and `Отмена` preserves it.
- During Sprint 020 Play Mode testing, use `Project Genesis > Sprint 020 > Add Sword And Axe In Play Mode` to add both contrasting weapons immediately.
- During Sprint 019 Play Mode testing, use `Project Genesis > Sprint 019 > Add Two Rusty Swords In Play Mode` to create two distinct copies without waiting for random drops.
- Equipping the sword in the main-hand slot increases attack power from 14 to 18. Removing it from the equipment side of the inventory returns the exact item to the bag and removes the bonus.
- The camera shortens its distance when a wall or prop blocks the view, then returns to the requested zoom when clear.
- Click the Village Elder once to select him and show his target panel.
- Click the selected Village Elder again at any time to talk; a quick double-click performs both steps. If the player is far away, the character runs closer and opens dialogue only when almost touching the NPC.
- NPC interaction can be tuned on the player object in the `PlayerInteractionController` component:
  - `Interaction Radius` controls how close the player must be for dialogue to open and stay open;
  - `Approach Distance` controls how close the player tries to stand after clicking an NPC;
  - `Click Approach Max Distance` controls how far away an NPC can be clicked for auto-approach.
- `E` is still available as a temporary fallback while standing near an NPC.
- Accept the Village Elder's quest, collect five wolf tails beyond the north gate, then return to the elder and choose `Завершить поручение`.
- The compact tracker in the upper-right corner shows current tail progress and tells the player when it is time to return to the elder.
- Press `J` or click `Задания [J]` to open the quest journal. Its active and completed tabs are generated from the quest log rather than being hardcoded for one quest.
- Select a journal entry to see its description, objective, quest giver, state, and experience reward.
- An active or ready-to-turn-in quest can be abandoned with a two-step confirmation. Its tracker disappears immediately, and accepting it again starts objective progress from zero.
- Real objective progress briefly appears as a notification near the top of the screen. Loading an existing profile does not replay old notifications.
- Defeating the wolf grants 20 experience. Turning in the quest grants another 80 experience, raises the player to level 2, increases maximum health to 110, and increases base attack power by 2.
- Player position, level, experience, quest state, exact inventory positions, equipped weapon, equipped body armor, and remaining potions are saved automatically. There is intentionally no save button.
- For the current offline prototype, persistence uses a local JSON file behind a replaceable interface. In the future online version, the authoritative server will store this state and return the character near the last valid position after login.
- To start a fresh development playthrough, use `Project Genesis > Development > Clear Local Prototype Profile` while outside Play mode.
- Corpse resurrection, resurrection items or skills, item or quest loss on death, server-authoritative combat, and a real multiplayer backend remain outside the current prototype.

## Documentation Map

- [00_PROJECT_MANIFESTO.md](00_PROJECT_MANIFESTO.md) - why the project exists and what decisions are considered correct.
- [01_MASTER_PROMPT.md](01_MASTER_PROMPT.md) - main instruction file for Codex.
- [02_GAME_VISION.md](02_GAME_VISION.md) - target feeling, player fantasy, and design pillars.
- [03_GAME_DESIGN_DOCUMENT.md](03_GAME_DESIGN_DOCUMENT.md) - high-level game design document.
- [04_WORLD_BUILDING.md](04_WORLD_BUILDING.md) - world, tone, regions, factions, and lore direction.
- [05_RACES_AND_CLASSES.md](05_RACES_AND_CLASSES.md) - first playable races and classes.
- [06_GAMEPLAY_SYSTEMS.md](06_GAMEPLAY_SYSTEMS.md) - core systems and how they connect.
- [07_COMBAT_SYSTEM.md](07_COMBAT_SYSTEM.md) - combat loop, stats, skills, enemies, and encounter rules.
- [08_CHARACTER_PROGRESSION.md](08_CHARACTER_PROGRESSION.md) - leveling, attributes, skills, and long-term growth.
- [09_ITEMS_INVENTORY_EQUIPMENT.md](09_ITEMS_INVENTORY_EQUIPMENT.md) - item, inventory, equipment, and loot rules.
- [10_NPC_QUEST_SYSTEM.md](10_NPC_QUEST_SYSTEM.md) - NPCs, dialogue, quests, and rewards.
- [11_MOBS_AI_AND_BOSSES.md](11_MOBS_AI_AND_BOSSES.md) - enemy types, AI, elites, and bosses.
- [12_TECH_ARCHITECTURE.md](12_TECH_ARCHITECTURE.md) - Unity architecture and system boundaries.
- [13_UNITY_DEVELOPMENT_RULES.md](13_UNITY_DEVELOPMENT_RULES.md) - project structure, prefabs, scenes, assets, and naming.
- [14_CODEX_WORKING_RULES.md](14_CODEX_WORKING_RULES.md) - how Codex should work inside this repo.
- [15_ROADMAP.md](15_ROADMAP.md) - milestones from prototype to version 1.0.
- [16_SPRINT_001.md](16_SPRINT_001.md) - first actionable development sprint.
- [17_BACKLOG.md](17_BACKLOG.md) - ideas that are useful but not current.
- [18_CODEX_FIRST_MESSAGE.md](18_CODEX_FIRST_MESSAGE.md) - first message to give Codex when starting a new task.
- [19_SPRINT_002.md](19_SPRINT_002.md) - starter village blockout sprint.
- [20_SPRINT_003.md](20_SPRINT_003.md) - first NPC, dialogue, and quest state sprint.
- [21_SPRINT_004.md](21_SPRINT_004.md) - first enemy, combat, health, death, and experience sprint.
- [22_SPRINT_005.md](22_SPRINT_005.md) - first loot, inventory, and equipment reward sprint.
- [23_SPRINT_006.md](23_SPRINT_006.md) - first complete quest, progression, and automatic persistence loop sprint.
- [24_SPRINT_007.md](24_SPRINT_007.md) - first-zone enemy population, respawn, chance loot, trophy quest, and tracker sprint.
- [25_SPRINT_008.md](25_SPRINT_008.md) - persistent quest journal, reusable quest metadata, abandon flow, notifications, and quest-state validation sprint.
- [26_SPRINT_009.md](26_SPRINT_009.md) - data-driven loot tables, wolf drop rebalancing, Inspector tuning, and probability-validation sprint.
- [27_SPRINT_010.md](27_SPRINT_010.md) - delayed player and enemy recovery, preserved retreat health, tunable settings, and validation sprint.
- [28_SPRINT_011.md](28_SPRINT_011.md) - bounded enemy territory, idle roaming, peaceful-village protection, and validation sprint.
- [29_SPRINT_012.md](29_SPRINT_012.md) - configurable player death experience and level-loss sprint.
- [30_SPRINT_013.md](30_SPRINT_013.md) - enemy levels, target display, and level-difference experience scaling sprint.
- [31_SPRINT_014.md](31_SPRINT_014.md) - first forest-boar enemy, mixed population, and shared-system validation sprint.
- [32_DEVELOPMENT_PLAN_RU.md](32_DEVELOPMENT_PLAN_RU.md) - current Russian development plan, priorities, seamless-world direction, and task-handoff policy.
- [33_SPRINT_015.md](33_SPRINT_015.md) - skills foundation, Heavy Strike, hotbar, cooldown, and validation sprint.
- [34_SPRINT_016.md](34_SPRINT_016.md) - persistent character name, race, class, and compatibility-validation sprint.
- [35_SPRINT_017.md](35_SPRINT_017.md) - first-character creation, local selection, gameplay gating, and lifecycle-validation sprint.
- [36_SPRINT_018.md](36_SPRINT_018.md) - unified character stats, warrior bonuses, weapon-scaled Heavy Strike, stats UI, and validation sprint.
- [37_SPRINT_019.md](37_SPRINT_019.md) - stable item instances, profile migration, explicit inventory selection, and validation sprint.
- [38_SPRINT_020.md](38_SPRINT_020.md) - persistent inventory positions, drag-and-drop swapping, weapon variety, and validation sprint.
- [39_SPRINT_021.md](39_SPRINT_021.md) - body equipment, defense bonuses, one healing consumable, persistence, and validation sprint.
- [40_SPRINT_022.md](40_SPRINT_022.md) - explicit player death state, resurrection choice, input lock, and validation sprint.
- [41_SPRINT_023.md](41_SPRINT_023.md) - separate equipment storage, character equipment view, profile migration, and validation sprint.
- [CHANGELOG.md](CHANGELOG.md) - change history.

## Development Principle

Every task should be small enough to finish, test, and understand. Large ideas must be split into clear implementation steps before code is written.
