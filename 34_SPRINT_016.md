# Sprint 016 - Character Identity

Status: completed and accepted in Play Mode on 2026-07-20.

## Sprint Goal

Add minimal reusable and persistent character identity data to the existing player without building character creation yet.

## Task 1 - Add Authored Race And Class Data

Acceptance criteria:

- race and class use separate reusable ScriptableObject definitions with stable ids and display names;
- the prototype contains one human race asset and one warrior class asset;
- data validation rejects missing ids or display names;
- no additional races, classes, class stats, skill trees, or visual variants are added.

## Task 2 - Add Runtime Player Identity

Acceptance criteria:

- the player prefab contains one reusable `PlayerIdentity` component;
- the current prototype identity is `Путник`, `Человек`, and `Воин`;
- the identity exposes a change event for future creation and selection UI;
- invalid or missing restored values fall back to the authored prefab defaults.

## Task 3 - Persist Stable Identity Values

Acceptance criteria:

- the local profile stores the character name, race id, and class id;
- profile version 2 remains able to load existing version-1 saves;
- only stable ids and primitive values are stored, never Unity object references;
- inventory, equipment, progression, quests, and saved position continue to restore normally.

## Task 4 - Show Minimal Identity Feedback

Acceptance criteria:

- the existing inventory window shows the current name, race, and class;
- the display is temporary blockout UI and does not become a character-creation screen;
- an editor validator checks data assets, prefab wiring, profile compatibility, and scene UI wiring.

## Not In This Sprint

- character creation or character selection screens;
- editable player names in normal gameplay;
- additional races or classes;
- class stat bonuses or weapon-based skill scaling;
- models, equipment visuals, animations, VFX, sound, or final UI art;
- server accounts, multiplayer, or networked persistence.
