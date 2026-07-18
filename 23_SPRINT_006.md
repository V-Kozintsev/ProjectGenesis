# Sprint 006 - First Loop

## Sprint Goal

Complete the first RPG loop: accept the Village Elder's request, defeat the wolf, return for a reward, reach level 2, and automatically restore persistent character state on the next session.

## Tasks

### Task 1 - Document Sprint Scope

Acceptance criteria:

- quest progress, progression, rewards, and persistence have clear ownership;
- persistence is described as an automatic stand-in for a future online backend;
- no manual save/load menu is introduced.

### Task 2 - Complete The First Kill Quest

Connect the existing wolf defeat to the Village Elder quest.

Acceptance criteria:

- the quest asks for one wolf in the current one-enemy prototype;
- accepting the quest changes it to `Active` with progress `0 / 1`;
- defeating the wolf while the quest is active changes it to `ReadyToTurnIn`;
- defeating the wolf before accepting the quest does not retroactively complete it;
- returning to the Village Elder allows the quest to be completed once.

### Task 3 - Add Quest Reward And Level Up

Reward the complete loop with visible progression.

Acceptance criteria:

- the wolf still grants 20 experience;
- turning in the quest grants 80 experience;
- 100 total experience raises the player from level 1 to level 2;
- level 2 increases base health and attack power;
- the HUD shows experience toward the next level.

### Task 4 - Add Automatic Character Persistence

Persist stable player state without presenting a single-player save system.

Acceptance criteria:

- a replaceable persistence interface owns load and save operations;
- the current prototype uses local JSON as a temporary backend substitute;
- position, level, experience, quest state/progress, inventory item IDs, and equipped main-hand item ID are stored;
- data is restored automatically when Play Mode starts;
- the restored position is validated against NavMesh;
- state is saved periodically and when Play Mode or the application stops;
- scene objects and transient combat state are not serialized.

### Task 5 - Add Development Reset

Allow clean repeatable testing without adding a player-facing save menu.

Acceptance criteria:

- an Editor menu command clears only the local prototype profile;
- regular players do not see save/load/reset controls in the game UI.

### Task 6 - Update Project Notes

Acceptance criteria:

- README explains the complete quest loop and automatic persistence;
- architecture notes distinguish local prototype persistence from future server persistence;
- roadmap records Sprint 006 implementation status;
- changelog records the first complete RPG loop.

## Not In This Sprint

- accounts, authentication, character selection, or a real server database;
- manual save/load slots or save buttons;
- multiple quests or objective types;
- quest markers, quest tracker panel, or polished quest UI;
- multiple wolves or enemy respawn;
- gold, vendors, trading, or crafting;
- player or enemy regeneration;
- combat skills;
- multiplayer networking.
