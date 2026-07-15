# Master Prompt For Codex

You are working on Project Genesis, a Unity fantasy online RPG prototype. Before changing code, read the project documentation in this repository.

## Primary Objective

Build the game step by step, starting from a small playable RPG loop. Do not jump ahead into large MMO systems until the local prototype is stable.

## Required Working Style

When starting any task:

1. Read the relevant documentation.
2. Inspect the existing Unity project structure.
3. Identify current systems and naming conventions.
4. Propose the smallest safe implementation plan if the task is broad.
5. Implement only the agreed task.
6. Keep changes scoped.
7. Run available tests or Unity validation steps where possible.
8. Update documentation when behavior or architecture changes.

## Architecture Priorities

Prefer:

- data-driven configuration;
- plain C# systems where possible;
- ScriptableObjects for authored game data;
- MonoBehaviours as Unity-facing adapters;
- clear separation between UI, gameplay, data, saving, and input;
- small scenes with explicit responsibility.

Avoid:

- giant manager classes;
- direct cross-scene references;
- hidden dependencies;
- hardcoded item, quest, enemy, and class values inside combat logic;
- implementing future systems before they are needed.

## Current Development Philosophy

The project should grow through milestones:

1. Character movement and camera.
2. Starter scene.
3. Basic interaction.
4. Basic combat.
5. Experience and leveling.
6. Inventory and equipment.
7. Quest completion.
8. Save/load.
9. Content expansion.
10. Multiplayer exploration only after the local game loop is strong.

## Response Format For Development Tasks

For each task, Codex should summarize:

- what was changed;
- which files were touched;
- how to test it;
- what remains for later.

