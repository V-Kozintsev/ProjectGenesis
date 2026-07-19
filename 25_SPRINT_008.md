# Sprint 008 - Quest System Foundation

## Sprint Goal

Turn the single quest prototype into a reusable player-facing quest foundation with a journal, persistent display data, abandonment, progress feedback, and deterministic state validation.

## Task 1 - Persist Quest Display Data

Acceptance criteria:

- accepted quest progress stores its title, description, objective label, and quest-giver name;
- the journal reads from `QuestLog` instead of directly depending on a scene NPC;
- display data survives Play mode restarts through the existing local profile;
- existing objective count, reward, ready-to-turn-in, and completed behavior remains intact.

## Task 2 - Add A Quest Journal

Acceptance criteria:

- `J` and an on-screen button open and close the journal;
- the journal has active and completed tabs;
- a quest list supports more than one stored quest without requiring another window implementation;
- the selected quest shows title, state, description, objective progress, quest giver, and reward;
- empty tabs show a clear empty state;
- the journal fits the current desktop Game view without overlapping its own controls.

## Task 3 - Add Quest Abandonment

Acceptance criteria:

- active and ready-to-turn-in quests can be abandoned from the journal;
- abandonment removes stored objective progress and returns the quest to `NotStarted`;
- the tracker hides after abandonment;
- the Village Elder offers the abandoned quest again from zero progress;
- completed quests cannot be abandoned.

## Task 4 - Add Objective Progress Feedback

Acceptance criteria:

- a successful wolf-tail roll shows a short non-blocking notification;
- the notification includes the quest title and capped objective progress;
- failed rolls, inactive quests, restored profiles, and completed objectives do not show false progress notifications;
- the notification disappears automatically and does not require a click.

## Task 5 - Add Deterministic State Validation

Acceptance criteria:

- an editor command validates acceptance, progress, the `5 / 5` cap, ready state, turn-in, and abandonment;
- validation confirms progress cannot advance before acceptance or after the objective is ready;
- validation reports one clear success message or throws a useful failure exception.

## Preserve Existing Behavior

- one click selects and a second click acts;
- movement, camera, combat, respawn, regular loot, inventory, equipment, tracker, progression, and automatic persistence continue to work;
- quest trophies remain abstract progress and never become world or inventory items;
- `.vsconfig` remains outside project work.

## Not In This Sprint

- additional quests, quest chains, prerequisites, or branching dialogue;
- map markers, navigation arrows, shared party progress, or timed quests;
- server-authoritative quest validation;
- final UI art, animation, audio, localization tables, or controller navigation;
- a general ScriptableObject quest authoring pipeline.
