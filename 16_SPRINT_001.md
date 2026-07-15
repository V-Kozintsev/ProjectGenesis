# Sprint 001 - Foundation

## Sprint Goal

Prepare the repository and first Unity foundation without overbuilding.

## Tasks

### Task 1 - Create Unity Project

Create the Unity project in this repository using the chosen Unity LTS version.

Acceptance criteria:

- Unity project opens cleanly;
- standard folders exist;
- project can enter play mode without errors.

### Task 2 - Create Starter Folder Structure

Create the folder structure described in [12_TECH_ARCHITECTURE.md](12_TECH_ARCHITECTURE.md).

Acceptance criteria:

- `Assets/ProjectGenesis/` exists;
- data, prefabs, scenes, scripts, UI, and settings folders exist.

### Task 3 - Add Starter Scene

Create a simple starter scene.

Acceptance criteria:

- ground plane or terrain exists;
- player spawn point exists;
- basic lighting exists;
- scene is saved in the project scenes folder.

### Task 4 - Add Player Movement Prototype

Add basic third-person movement.

Acceptance criteria:

- player can move by left-clicking the ground;
- keyboard movement remains available as an optional fallback;
- movement is smooth enough for prototype;
- camera follows player;
- code is separated from future combat logic.

### Task 5 - Document What Was Built

Update changelog and roadmap status.

Acceptance criteria:

- [CHANGELOG.md](CHANGELOG.md) mentions completed setup;
- any architecture deviations are documented.

## Not In This Sprint

- multiplayer;
- inventory;
- full combat;
- character creation UI;
- skill trees;
- crafting;
- boss systems.
