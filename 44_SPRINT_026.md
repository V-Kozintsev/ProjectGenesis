# Sprint 026 - Parallel Quests And Second Quest

## Sprint Goal

Prove that the reusable quest foundation supports more than one NPC and more than one active quest by adding one short boar-hunt quest without changing profile format or duplicating quest logic.

## Task 1 - Add A Second Quest Giver

Acceptance criteria:

- the starter village contains a simple Guard Captain prototype using the existing NPC selection, approach, and dialogue systems;
- the Guard Captain owns the data-driven `Boar Threat` quest while the Village Elder keeps `Wolf Trophies`;
- both NPCs can be selected with one click and interacted with by clicking again or using the existing fallback interaction;
- no final model, animation, voice, or dialogue tree is introduced.

## Task 2 - Add The Boar Hunt

Acceptance criteria:

- `SO_Quest_BoarHunt` uses the shared `QuestDefinition` data model;
- the objective requires two enemies with the existing stable target id `boar`;
- the authored reward is 100 experience;
- the boar prefab advances only the boar objective and wolves keep their independent trophy objective;
- the quest can be accepted before the wolf quest is completed.

## Task 3 - Support Parallel Active Quests

Acceptance criteria:

- the quest log accepts and progresses the wolf and boar quests at the same time;
- changing one objective does not change the other objective;
- either ready quest can be turned in first without completing the other;
- both quest states and counters restore together through the existing profile quest list with no profile-version migration;
- the compact tracker shows both active quests and the journal continues to list their full details.

## Task 4 - Restore Target Close Feedback

Acceptance criteria:

- the selected-target panel shows a clearly readable close symbol;
- the close button keeps its existing target-clear behavior;
- deterministic scene validation checks the symbol so a later rebuild cannot silently remove it.

## Task 5 - Validate Regressions

Acceptance criteria:

- a Sprint 026 editor validator checks both quest assets, unique ids, boar target wiring, simultaneous acceptance, independent progress, persistence, independent turn-in, reward totals, both NPC assignments, shared tracker presence, and the close symbol;
- the existing scene is migrated without a full-scene file-id rewrite;
- the relevant Sprint 008 through Sprint 026 regression suite passes;
- the user verifies both NPCs, two simultaneous quests, independent progress, journal entries, tracker text, saving, and the visible close symbol in Play Mode.

## Not In This Sprint

- a third quest, another village, a quest chain, prerequisites, branching, choices, repeatable quests, or daily quests;
- multiple quests offered by one NPC in a dedicated selection UI;
- new enemy types, items, equipment, currency, reputation, vendors, or zones;
- map markers, navigation assistance, final NPC models, animations, VFX, audio, or UI art;
- server authority, party-shared progress, multiplayer synchronization, or network persistence.
