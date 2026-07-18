# Changelog

## 2026-07-18

- Documented Sprint 004 scope and acceptance criteria.
- Added reusable health, combat stats, distance checks, and player progression components.
- Added click-to-target combat: the player approaches a selected enemy and performs timed basic attacks automatically.
- Unified mouse interaction: one click selects an NPC or enemy, while a double-click starts dialogue or combat with that selected target.
- Kept selected targets while moving away, separated stopping an action from clearing a target, and added `Esc` plus a target-panel close button for explicit deselection.
- Removed click-timing dependence: clicking an already selected NPC or enemy performs its action regardless of how long ago it was selected.
- Reused the top target panel for selected NPCs as well as enemies.
- Expanded the classic MMORPG camera controls: mouse-wheel click toggles the front/rear view, while a short right-click returns the camera behind the character.
- Added a first wolf enemy with idle, chase, attack, return, and death states.
- Added player death, delayed respawn at the village spawn point, and full-health restoration.
- Added a combat HUD for player health, selected-enemy health, level, and experience.
- Added 20 experience as the first combat reward.
- Added a blockout wolf and combat target marker to the starter village scene.
- Kept Sprint 004 scoped away from loot, inventory, equipment, quest completion, skills, and save/load.
- Documented Sprint 005 scope and acceptance criteria.
- Opened a north village gate and added a visually distinct combat area beyond it.
- Moved the wolf outside the village and shortened its leash so it returns before entering the peaceful area.
- Added an authored `Rusty Sword` item definition and a visible wolf loot drop.
- Added an adjustable corpse lifetime so the defeated wolf disappears after six seconds without removing its loot.
- Added click-to-approach loot collection using the existing NavMesh movement.
- Added an eight-slot player inventory and one main-hand equipment slot.
- Added a compact inventory window opened by `I` or its on-screen button.
- Made the equipped sword increase attack power from 14 to 18, with immediate equip and unequip feedback.
- Kept Sprint 005 scoped away from general zone rules, armor, trading, crafting, quest rewards, enemy respawn, and save/load.

## 2026-07-15

- Added initial project documentation foundation.
- Added game vision, design direction, technical architecture, roadmap, sprint plan, and Codex working rules.
- Completed Sprint 001 foundation setup: created `Assets/ProjectGenesis/` folder structure, starter scene, prototype player prefab, third-person camera follow, and basic movement.
- Made left-click movement the primary control style and kept keyboard movement as an optional fallback.
- Added independent right-mouse camera orbit and mouse-wheel zoom.
- Kept Sprint 001 scoped to foundation work only; combat, quests, inventory, multiplayer, and character creation UI were not started.
- Documented Sprint 002 scope and acceptance criteria.
- Built a starter village blockout with simple houses, props, road shapes, playable-area boundaries, and a minimal UI placeholder.
- Replaced direct click movement with NavMeshAgent movement that routes around obstacles and rejects unreachable destinations.
- Added runtime NavMesh building for the prototype scene so the scene stays text-serializable and easy to review.
- Added camera collision protection so the follow camera does not pass through village blockers.
- Kept Sprint 002 scoped away from combat, quests, NPC interaction, and inventory.
- Documented Sprint 003 scope and acceptance criteria.
- Added the first Village Elder NPC prototype to the starter village.
- Added range-limited click interaction with the Village Elder, a simple dialogue window, and an interaction prompt.
- Kept `E` as a temporary fallback while making mouse interaction the primary NPC flow.
- Added click-to-approach NPC interaction: single-click moves toward the NPC, while double-click opens dialogue when close enough.
- Exposed interaction range tuning through `PlayerInteractionController` inspector fields.
- Delayed enabling `NavMeshAgent` until after the runtime NavMesh is built to avoid startup NavMesh errors.
- Split NPC interaction tuning into close dialogue radius, approach distance, maximum click-to-approach distance, and double-click timing.
- Added minimal quest state handling for `Wolves Near The Road`, including `NotStarted` and `Active`.
- Kept Sprint 003 scoped away from combat, enemies, quest completion, rewards, and inventory.
