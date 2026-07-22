# Sprint 025 - Reusable Quest Content Definitions

## Sprint Goal

Move concrete quest content out of the village elder component into reusable, Inspector-authored data without changing the accepted first quest flow or breaking existing saved quest progress.

## Task 1 - Add Reusable Quest Data

Acceptance criteria:

- a `QuestDefinition` asset owns stable identity, display name, description, dialogue text, objective data, and reward data;
- the first objective type is explicit `DefeatTarget` rather than an unlabelled target string;
- objective target, player-facing progress label, return instruction, and required count are Inspector-authored;
- the first reward definition stores non-negative experience;
- deterministic validation rejects missing ids, text, objectives, targets, counts, or rewards;
- item rewards, objective chains, prerequisites, and branching remain future extensions rather than fake incomplete behavior.

## Task 2 - Decouple The Quest Giver

Acceptance criteria:

- `InteractableNpc` references one `QuestDefinition` instead of serializing the concrete wolf quest fields;
- dialogue state text and quest presentation resolve from that definition;
- `QuestLog` accepts a valid definition through a reusable API;
- the existing field-based acceptance API remains available for regression and save compatibility;
- the quest tracker uses saved or definition-authored objective labels instead of hardcoded wolf text.

## Task 3 - Preserve The Existing Quest And Saves

Acceptance criteria:

- the starter-village builder creates one `SO_Quest_WolfTrophies` asset and assigns it to the village elder;
- quest id, target id, required count, experience reward, dialogue, journal, progress toast, and turn-in behavior remain unchanged;
- already-saved active, ready, completed, or abandoned quest states continue to load without a profile-version migration;
- accepting the data-driven quest still copies self-contained presentation and reward data into saved progress.

## Task 4 - Validate Regressions

Acceptance criteria:

- a Sprint 025 editor validator checks asset validity, exact authored data, definition-based acceptance, progress, turn-in reward, restored legacy progress, NPC assignment, and scene wiring;
- the starter-village builder recreates the asset, NPC reference, prefab, and scene deterministically;
- the relevant Sprint 008 through Sprint 025 regression suite passes;
- the user verifies the unchanged quest flow in Play Mode.

## Not In This Sprint

- a second quest, new NPC, enemy, zone, item, or visual redesign;
- item, currency, reputation, choice, random, or scaled quest rewards;
- multiple simultaneous objectives, collection counters, escort, exploration, dialogue, timed, repeatable, daily, party, or account-wide objectives;
- prerequisites, quest chains, branching, failure states, level gates, class gates, map markers, or navigation assistance;
- server authority, shared party progress, multiplayer synchronization, or network persistence.
