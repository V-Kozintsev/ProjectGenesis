# Changelog

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
