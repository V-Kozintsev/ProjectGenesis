# Sprint 017 - Character Creation And Selection

## Sprint Goal

Add a minimal local flow for creating and selecting the first character on top of the persistent identity foundation.

## Task 1 - Add Character Lifecycle State

Acceptance criteria:

- local profile version 3 records whether the first character has been created;
- existing version-1 and version-2 profiles are treated as already-created characters;
- new profiles remain in creation state until a valid name is submitted;
- inventory, equipment, progression, quests, position, race, and class continue to restore.

## Task 2 - Create The First Character

Acceptance criteria:

- a new local profile opens a blocking character-creation overlay;
- the player can enter a trimmed name up to 24 characters;
- blank names are rejected with short feedback;
- the only current authored choices are shown as `Человек` and `Воин`;
- successful creation persists the character and switches to selection mode.

## Task 3 - Select And Enter Gameplay

Acceptance criteria:

- an existing character opens a selection overlay showing name, race, class, and level;
- pressing `Играть` closes the overlay and enables normal gameplay input;
- movement, combat, skills, NPC interaction, loot, inventory, and quest input stay disabled until selection;
- no gameplay progress is reset by entering through selection.

## Task 4 - Keep The Flow Verifiable

Acceptance criteria:

- the starter-village builder creates and wires the creation and selection overlay;
- an editor validator checks profile-version compatibility, lifecycle rules, scene wiring, and preserved Sprint 015/016 UI;
- the project compiles and the Sprint 017 validator passes in Unity batch mode.

## Not In This Sprint

- multiple character slots or deleting characters;
- additional races, classes, appearance choices, or body customization;
- class bonuses, stat allocation, or weapon-based skill scaling;
- separate login, account, bootstrap, or character-creation scenes;
- final models, animation, VFX, sound, or UI art;
- server, multiplayer, or networked persistence.
