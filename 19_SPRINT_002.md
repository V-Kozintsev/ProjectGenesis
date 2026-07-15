# Sprint 002 - Starter Village Blockout

## Sprint Goal

Turn the starter scene into a small playable village blockout with obstacle-aware movement and clear prototype boundaries.

## Tasks

### Task 1 - Document Sprint Scope

Create this sprint document before implementation.

Acceptance criteria:

- sprint goal is written down;
- acceptance criteria are clear;
- exclusions protect the project from starting later systems too early.

### Task 2 - Build Starter Village Blockout

Create a simple greybox village layout in `StarterVillage`.

Acceptance criteria:

- the scene contains a larger ground area;
- the village has simple building and prop blockers;
- the layout has visible boundaries around the playable area;
- the player starts inside the village.

### Task 3 - Add NavMesh Movement

Replace direct click movement with NavMesh-based movement.

Acceptance criteria:

- left-click movement routes around obstacles;
- clicking an unreachable point does not move the player;
- the destination marker appears only for accepted destinations;
- WASD or arrow-key fallback movement remains available.

### Task 4 - Protect Camera From Obstacles

Prevent the follow camera from passing through walls and props.

Acceptance criteria:

- right mouse button still rotates the camera;
- mouse wheel still zooms in and out;
- camera distance shortens when an obstacle blocks the view;
- camera returns to the requested distance when the path is clear.

### Task 5 - Add Minimal UI Placeholder

Add a small non-interactive UI label for the current prototype state.

Acceptance criteria:

- UI is visible in Play mode;
- UI does not introduce inventory, quests, combat, or dialogue.

### Task 6 - Update Project Notes

Update the changelog and README controls/status after implementation.

Acceptance criteria:

- documentation mentions Sprint 002;
- prototype controls remain easy to find.

## Not In This Sprint

- combat;
- quests;
- NPC interaction;
- inventory;
- character creation;
- multiplayer;
- polished art pass.
