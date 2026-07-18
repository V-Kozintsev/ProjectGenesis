# Project Genesis

Project Genesis is a small-scope online RPG prototype built in Unity. The goal is not to copy a giant MMORPG at full scale, but to create a focused, playable foundation: character creation, a starter village, exploration, combat, progression, loot, quests, and a clean architecture that can grow without collapsing.

## Current Stage

Sprint 005 first reward is implemented: the prototype now includes a peaceful village gate, a separate northern combat area, one hostile wolf, a visible weapon drop, an eight-slot inventory, and a main-hand equipment bonus.

## Prototype Controls

- Open `Assets/ProjectGenesis/Scenes/StarterVillage.unity` before entering Play mode.
- Left-click the ground to move the character. This is the primary control style, inspired by classic point-and-click MMORPGs such as Lineage 2.
- Click movement uses NavMesh navigation, so the character routes around simple obstacles and ignores unreachable points.
- Hold the right mouse button and move the mouse to rotate the camera horizontally and vertically. A short right-click returns it behind the character.
- Use the mouse wheel to zoom the camera in and out. Click the wheel to switch between the rear and front views.
- WASD or arrow keys are optional fallback controls and cancel the current click destination.
- Leave the village through the north gate to reach the first combat area. The wolf returns home before it can chase the player back into the village.
- Click the grey wolf in the northern combat area once to select it. Click the selected wolf again at any time to run into range and attack automatically; a quick double-click performs both steps.
- Clicking the ground or using WASD stops the current approach or attack but keeps the selected target.
- Press `Esc`, click the `X` in the target panel, or select another creature to clear or switch the current target.
- The combat HUD shows player health, selected-enemy health, level, and experience.
- Defeating the wolf awards 20 experience. If the player dies, the character briefly disappears and returns at the village spawn point with full health.
- Defeating the wolf also drops a visible `Rusty Sword`. The wolf body disappears after six seconds, while the loot remains available; click the drop to approach it and collect it.
- Press `I` or click `Инвентарь [I]` to open the eight-slot inventory.
- Click `Надеть` to equip the sword in the main-hand slot and increase attack power from 14 to 18. Click `Снять` to remove the bonus without deleting the item.
- The camera shortens its distance when a wall or prop blocks the view, then returns to the requested zoom when clear.
- Click the Village Elder once to select him and show his target panel.
- Click the selected Village Elder again at any time to talk; a quick double-click performs both steps. If the player is far away, the character runs closer and opens dialogue only when almost touching the NPC.
- NPC interaction can be tuned on the player object in the `PlayerInteractionController` component:
  - `Interaction Radius` controls how close the player must be for dialogue to open and stay open;
  - `Approach Distance` controls how close the player tries to stand after clicking an NPC;
  - `Click Approach Max Distance` controls how far away an NPC can be clicked for auto-approach.
- `E` is still available as a temporary fallback while standing near an NPC.
- Accepting the first quest changes it from `NotStarted` to `Active`; defeating the wolf does not complete that quest yet.
- Armor, additional equipment slots, enemy respawn, trading, saving, combat skills, and quest completion are intentionally outside Sprint 005.

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
- [CHANGELOG.md](CHANGELOG.md) - change history.

## Development Principle

Every task should be small enough to finish, test, and understand. Large ideas must be split into clear implementation steps before code is written.
