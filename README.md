# Project Genesis

Project Genesis is a small-scope online RPG prototype built in Unity. The goal is not to copy a giant MMORPG at full scale, but to create a focused, playable foundation: character creation, a starter village, exploration, combat, progression, loot, quests, and a clean architecture that can grow without collapsing.

## Current Stage

Sprint 001 foundation is implemented: the Unity project has a starter scene, a prototype player, a following camera, and basic movement.

## Prototype Controls

- Open `Assets/ProjectGenesis/Scenes/StarterVillage.unity` before entering Play mode.
- Left-click the ground to move the character. This is the primary control style, inspired by classic point-and-click MMORPGs such as Lineage 2.
- Hold the right mouse button and move the mouse to rotate the camera horizontally and vertically.
- Use the mouse wheel to zoom the camera in and out.
- WASD or arrow keys are optional fallback controls and cancel the current click destination.
- The current prototype walks directly toward the clicked point. Obstacle-aware navigation will be added when the world has obstacles that require it.

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
- [CHANGELOG.md](CHANGELOG.md) - change history.

## Development Principle

Every task should be small enough to finish, test, and understand. Large ideas must be split into clear implementation steps before code is written.
