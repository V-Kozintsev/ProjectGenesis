# Sprint 024 - Local Message Feed Foundation

## Sprint Goal

Create one reusable, typed, session-local message stream and a movable prototype feed for useful player-facing events. Gameplay systems publish data without knowing how the UI renders it, while real player chat and global server announcements remain explicitly outside the local prototype.

## Task 1 - Add A Typed Message Stream

Acceptance criteria:

- messages use explicit `System`, `Loot`, `Combat`, `LocalChat`, or `Announcement` categories;
- every accepted message receives a monotonic session sequence number;
- blank messages are rejected;
- history has an Inspector-configurable bounded capacity;
- consumers can subscribe to published and cleared events without referencing UI code;
- message history is session-local and is not added to the character save profile.

## Task 2 - Publish Useful Gameplay Events

Acceptance criteria:

- successful pickup, world drop, and permanent item destruction publish clear messages;
- equipping, replacing, and unequipping weapon or armor publish system messages;
- successful potion use reports the actual restored health;
- Heavy Strike success and refusal feedback also enters the combat feed while the existing hotbar feedback remains intact;
- every successful ordinary player attack and enemy attack reports the actual applied damage and the enemy name;
- player death reports the experience loss and village resurrection reports success;
- messages are published only after the related gameplay operation succeeds.

## Task 3 - Add The Prototype Feed UI

Acceptance criteria:

- a lower-left panel shows the most recent matching messages in chronological order;
- the panel can be dragged by its header and remains reachable on screen;
- `Все`, `Система`, `Лут`, `Бой`, `Общий`, and `Важно` filters are available;
- category labels are visually distinguishable without final UI art;
- overflowing history can be reviewed with the mouse wheel or a visible draggable scrollbar, and new events automatically scroll to the latest message;
- the panel can be hidden and restored with its reopen button or `L`;
- the character-entry overlay gates the feed with the other gameplay UI;
- a length-limited local prototype input has a visible blinking caret and publishes the current character's messages into `Общий`;
- while text input is focused, gameplay movement, interaction, window hotkeys, selection shortcuts, and camera controls do not consume typed keys or chat scrolling.

## Task 4 - Validate Regressions

Acceptance criteria:

- an editor validator checks typed filtering, bounded ordered history, blank rejection, clear behavior, prefab wiring, scene wiring, filters, draggable header, draggable scrolling, and a visible constrained local input;
- the starter-village builder creates the stream and feed deterministically;
- the relevant Sprint 008 through Sprint 024 regression suite passes;
- the user performs the ordinary Play Mode visual check.

## Not In This Sprint

- network transport, real general, nearby, party, clan, trade, or private player chat;
- server-authenticated maintenance or exceptional-world-event broadcasts;
- moderation, mute, block, report, rate limits, links, channels, chat commands, or chat persistence;
- resizable window, timestamps, unread counters, copy, final fonts, icons, sounds, or UI art.
